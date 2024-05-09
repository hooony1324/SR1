using UnityEngine;

public class UI_SettingPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
    }

    enum Texts
    {
        UIDText,
    }

    enum Buttons
    {
        CloseButton,
        ConnectButton,
        SaveButton,
        UIDCopyButton,
    }

    //On, Off�� UI �����ο� ���� �������� ������ ����
    enum Toggles
    {
        BGMOnToggle,
        BGMOffToggle,
        EffectSoundOnToggle,
        EffectSoundOffToggle,
        VibrationOnToggle,
        VibrationOffToggle,
        DamageTextOnToggle,
        DamageTextOffToggle,
        SleepModeOnToggle,
        SleepModeOffToggle,
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)Buttons.ConnectButton).gameObject.BindEvent(OnClickConnectButton);
        GetButton((int)Buttons.SaveButton).gameObject.BindEvent(OnClickSaveButton);
        GetButton((int)Buttons.UIDCopyButton).gameObject.BindEvent(OnClickUIDCopyButton);

        GetToggle((int)Toggles.BGMOnToggle).gameObject.BindEvent(OnClickBGMOnToggle);
        GetToggle((int)Toggles.BGMOffToggle).gameObject.BindEvent(OnClickBGMOffToggle);
        GetToggle((int)Toggles.EffectSoundOnToggle).gameObject.BindEvent(OnClickEffectSoundOnToggle);
        GetToggle((int)Toggles.EffectSoundOffToggle).gameObject.BindEvent(OnClickEffectSoundOffToggle);
        GetToggle((int)Toggles.VibrationOnToggle).gameObject.BindEvent(OnClickVibrationOnToggle);
        GetToggle((int)Toggles.VibrationOffToggle).gameObject.BindEvent(OnClickVibrationOffToggle);
        GetToggle((int)Toggles.DamageTextOnToggle).gameObject.BindEvent(OnClickDamageTextOnToggle);
        GetToggle((int)Toggles.DamageTextOffToggle).gameObject.BindEvent(OnClickDamageTextOffToggle);
        GetToggle((int)Toggles.SleepModeOnToggle).gameObject.BindEvent(OnClickSleepModeOnToggle);
        GetToggle((int)Toggles.SleepModeOffToggle).gameObject.BindEvent(OnClickSleepModeOffToggle);

        Refresh();

        return true;
    }


    public void SetInfo()
    {

        Refresh();
    }


    void Refresh()
    {
        if (_init == false)
            return;


    }


    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickConnectButton()
    {
        Debug.Log("On Click ConnectButton");
    }

    void OnClickSaveButton()
    {
        Debug.Log("On Click SaveButton");
    }

    void OnClickUIDCopyButton()
    {
        Debug.Log("On Click UIDCopyButton");
    }

    void OnClickBGMOnToggle()
    {
        Debug.Log("On Click BGMOnToggle");
    }

    void OnClickBGMOffToggle()
    {
        Debug.Log("On Click BGMOffToggle");
    }

    void OnClickEffectSoundOnToggle()
    {
        Debug.Log("On Click EffectSoundOnToggle");
    }

    void OnClickEffectSoundOffToggle()
    {
        Debug.Log("On Click EffectSoundOffToggle");
    }

    void OnClickVibrationOnToggle()
    {
        Debug.Log("On Click VibrationOnToggle");
    }

    void OnClickVibrationOffToggle()
    {
        Debug.Log("On Click VibrationOffToggle");
    }

    void OnClickDamageTextOnToggle()
    {
        Debug.Log("On Click DamageTextOnToggle");
    }

    void OnClickDamageTextOffToggle()
    {
        Debug.Log("On Click DamageTextOffToggle");
    }

    void OnClickSleepModeOnToggle()
    {
        Debug.Log("On Click SleepModeOnToggle");
    }

    void OnClickSleepModeOffToggle()
    {
        Debug.Log("On Click SleepModeOffToggle");
    }
}
