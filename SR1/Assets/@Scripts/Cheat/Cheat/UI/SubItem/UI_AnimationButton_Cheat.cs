public class UI_AnimationButton_Cheat : UI_Base
{
    enum Buttons
    {
        AnimationButton,
    }

    enum Texts
    {
        AnimationText,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.AnimationButton).gameObject.BindEvent(OnClickAnimationButton);

        return true;
    }


    public void SetInfo(string AnimationName)
    {
        GetText((int)Texts.AnimationText).text = AnimationName;
    }

    void OnClickAnimationButton()
    {
        (Managers.Scene.CurrentScene as ArtTestScene).Creature_Cheat.SetAnimation(GetText((int)Texts.AnimationText).text);
    }
}
