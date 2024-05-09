public class UI_HeroInfo_AbilityItem : UI_SubItem
{
    enum Images
    {
        LockImage,
    }

    enum Texts
    {
        LevelText,
        AbilityText,
    }


    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        Refresh();

        return true;
    }

    //Ability Data
    public void SetInfo()
    {


        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;


    }
}
