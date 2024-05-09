using UnityEngine;
using Spine.Unity;
using Data;

public class UI_AddHeroes_HeroItem_Cheat : UI_Base
{
    enum Buttons
    {
        HeroButton,
    }

    [SerializeField]
    SkeletonGraphic _spine;
    
    HeroData _heroData;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);

        Refresh();

        return true;
    }

    public void SetInfo(HeroData heroData)
    {
        _heroData = heroData;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_heroData == null)
            return;

        _spine.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(_heroData.SkeletonDataID);
        _spine.Initialize(true);
    }
    
    void OnClickHeroButton()
    {
        if (_heroData == null)
            return;

        Managers.Hero.AcquireHeroCard(_heroData.TemplateId, 10);
    }
}
