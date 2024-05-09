using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

// Output the build size or a failure depending on BuildPlayer.
public class TeamCityBuilder : MonoBehaviour
{
    //젠킨스
    [MenuItem("Tools/Build Android")]
    public static void BuildAndroid()
    {
        // 어드레서블 프로파일 변경
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string profileID = settings.profileSettings.GetProfileId(Define.EBuildType.Remote.ToString());
        settings.activeProfileId = profileID;        
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[]
            { "Assets/@Scenes/TitleScene.unity", "Assets/@Scenes/LobbyScene.unity", "Assets/@Scenes/GameScene.unity" };
        string date = DateTime.Now.ToString("yyyyMMdd"); // "yyyyMMdd" 형식으로 날짜를 문자열로 변환합니다.
        buildPlayerOptions.locationPathName = $"./Builds/{date}_M1.apk"; // 날짜를 포함한 파일명을 설정합니다.
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        PlayerSettings.Android.keystorePass = "rookiss";
        PlayerSettings.Android.keyaliasName = "rookiss";
        PlayerSettings.Android.keyaliasPass = "rookiss";

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }


}