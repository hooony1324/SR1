public class UI_ChallengeItem : UI_Base
{
    enum GameObjects
    {
        TicketArea,
        RewardSlot1Obj,
        RewardSlot2Obj,
        RewardSlot3Obj,
    }

    enum Images
    {
        TicketImage,
        ChallengeImage,
        Reward1Image,
        Reward2Image,
        Reward3Image,
    }

    enum Texts
    {
        ChallengeNameText,
        TicketCountText,
    }

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent(OnClickGameObject);

        return true;
    }

    public void SetInfo()
    {

    }

    void Refresh()
    {

    }

    void OnClickGameObject()
    {

    }
}
