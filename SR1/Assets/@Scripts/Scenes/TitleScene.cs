using UnityEngine;
using UnityEngine.Rendering;

public class TitleScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Debug.Log("hi");



        //TitleUI


        return true;
    }

    private void Awake()
    {
        SceneType = Define.EScene.TitleScene;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;  
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 0.0f);
    }

    public override void Clear()
    {

    }

}
