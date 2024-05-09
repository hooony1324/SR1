using System.Collections;
using System.Diagnostics;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EditorTools
{
    [MenuItem("Tools/Upload To S3")]
    public static void UploadToS3()
    {
        EditorUtils.SetAddressableProfile(Define.EBuildType.Remote);

        EditorCoroutineUtility.StartCoroutineOwnerless(WaitForAddressablesBuildAndUpload());
    }

    [MenuItem("Clear/Clear Addressables")]
    public static void ClearAddressableData()
    {
        Caching.ClearCache();
    }

    [MenuItem("Tools/EditorPlay _F5")]
    public static void EditorPlay()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(WaitForAddressablesBuildAndPlay());
    }

    private static IEnumerator WaitForAddressablesBuildAndUpload()
    {
        // AddressableAssetSettings.CleanPlayerContent();
        AddressablesPlayerBuildResult result;
        AddressableAssetSettings.BuildPlayerContent(out result);

        yield return new WaitForSeconds((float)result.Duration + 0.5f);
        // 빌드 완료 후 업로드 

        SetBackupFile();

        string awsCliPath = @"C:\Program Files\Amazon\AWSCLI\bin\aws"; // AWS CLI 설치 경로에 맞게 수정
        string arguments = "s3 cp ServerData s3://m1-unity/ --recursive";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = awsCliPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();
        Debug.Log(process.StandardOutput.ReadToEnd());
        process.WaitForExit();
        process.Close();
    }
    
    private static IEnumerator WaitForAddressablesBuildAndPlay()
    {
        // AddressableAssetSettings.CleanPlayerContent();
        AddressablesPlayerBuildResult result;
        AddressableAssetSettings.BuildPlayerContent(out result);

        yield return new WaitForSeconds((float)result.Duration + 0.5f);
        // 빌드 완료 후 Play 모드 시작
        EditorApplication.isPlaying = true;
    }

    private static void SetBackupFile()
    {
        string path = Application.dataPath;
        string sourcePath =
            Path.GetFullPath(Path.Combine(path,
                @"../ServerData/Backup/Android/addressables_content_state.bin")); // 기존 파일 경로
        if (File.Exists(sourcePath))
        {
            string newPath = Path.GetFullPath(Path.Combine(path,
                @$"../ServerData/Backup/Android/addressables_content_state_{PlayerSettings.bundleVersion}.bin")); // 기존 파일 경로
            // 파일 복사
            File.Copy(sourcePath, newPath, true);
            // 필요하다면 기존 파일 삭제
            File.Delete(sourcePath);
            // 에디터 상에서 새 파일을 감지하도록 에셋 데이터베이스 갱신
            AssetDatabase.Refresh();
        }
    }

    #region 보류
    // [MenuItem("Tools/Set Sprite Pivot")]
    static void SetSprite()
    {
        SetSpritePivot(SpriteAlignment.Custom);
        SetSpritePivot(SpriteAlignment.Center);
        SetSpritePivot(SpriteAlignment.BottomCenter);
    }

    static void SetSpritePivot(SpriteAlignment spriteAlignment)
    {
        string spriteFolder = $"@Resources/TileMaps/TileSprite/{spriteAlignment.ToString()}";
        string fullPath = $"{Application.dataPath}/{spriteFolder}";
        if (!System.IO.Directory.Exists(fullPath))
        {
            return;
        }

        var folders = new string[] { $"Assets/{spriteFolder}" };
        var guids = AssetDatabase.FindAssets("t:Sprite", folders);

        var newSprites = new Sprite[guids.Length];

        for (int i = 0; i < newSprites.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);

            settings.spriteAlignment = (int)spriteAlignment;

            if (spriteAlignment == SpriteAlignment.Custom)
            {
                newSprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                float height = 100 / newSprites[i].rect.height;
                settings.spritePivot = new Vector2(0.5f, height);
            }

            importer.SetTextureSettings(settings);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    

    #endregion

}