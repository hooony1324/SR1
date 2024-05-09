using UnityEngine;

public class UI_ScalePopup_Cheat : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
    }

    enum Texts
    {
        CharacterScaleText,
        MonsterScaleText,
        CameraZoomInOutText,
    }

    enum Sliders
    {
        CharacterScaleSlider,
        MonsterScaleSlider,
        CameraZoomInOutSlider,
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindSlider(typeof(Sliders));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);

        GetSlider((int)Sliders.CharacterScaleSlider).onValueChanged.AddListener((value) =>
        {
            OnCharacterSliderValueChaged(value);
        });

        GetSlider((int)Sliders.MonsterScaleSlider).onValueChanged.AddListener((value) =>
        {
            OnMonsterSliderValuChanged(value);
        });

        GetSlider((int)Sliders.CameraZoomInOutSlider).onValueChanged.AddListener((value) =>
        {
            OnCameraSliderValueChanged(value);
        });

        return true;
    }


    public void SetInfo()
    {

    }

    void Refresh()
    {

    }

    void OnCharacterSliderValueChaged(float value)
    {
        GetText((int)Texts.CharacterScaleText).text = value.ToString("F2");
        foreach(Hero hero in Managers.Object.Heroes)
        {
            hero.transform.localScale = value * Vector3.one;
        }
    }

    void OnMonsterSliderValuChanged(float value)
    {
        GetText((int)Texts.MonsterScaleText).text = value.ToString("F2");
        foreach (Monster monster in Managers.Object.Monsters)
        {
            monster.transform.localScale = value * Vector3.one;
        }
    }

    void OnCameraSliderValueChanged(float value)
    {
        GetText((int)Texts.CameraZoomInOutText).text = value.ToString("F2");

        Camera.main.orthographicSize = value;
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }
}


