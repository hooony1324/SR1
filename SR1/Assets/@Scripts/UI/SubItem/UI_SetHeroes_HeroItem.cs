using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_SetHeroes_HeroItem : UI_SubItem
{
    enum Buttons
    {
        HeroButton,
    }

    enum Images
    {
        HeroImage,
        SetHeroImage,
    }

    enum Texts
    {
        BattlePowerText,
    }

    int _heroDataId = -1;
    bool _isSet = false;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetButton((int)Buttons.HeroButton).gameObject.BindEvent(OnClickHeroButton);
        GetImage((int)Images.SetHeroImage).gameObject.SetActive(false);

        Refresh();

        return true;
    }

    //영웅 Index 받아서 처리
    public void SetInfo(int heroDataId, bool isSet)
    {
        _heroDataId = heroDataId;
        _isSet = isSet;

        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_heroDataId < 0)
            return;

        GetImage((int)Images.SetHeroImage).gameObject.SetActive(_isSet);
        GetImage((int)Images.HeroImage).sprite = Managers.Resource.Load<Sprite>(Managers.Data.HeroDic[_heroDataId].IconImage);

        //임시
        GetText((int)Texts.BattlePowerText).text = $"{Managers.Data.HeroDic[_heroDataId].DescriptionTextID}";
    }

    void OnClickHeroButton()
    {
        if (_isSet)
        {
            List<Hero> heroes = Managers.Object.Heroes.ToList();

            if (heroes.Count <= 1)
                return;

            Hero hero = heroes.Find(hero => hero.TemplateId == _heroDataId);// && !hero.IsLeader);
            if (hero == null)
                return;

            if(hero.IsLeader)
            {
                Hero newLeader = hero != heroes[0] ? heroes[0] : heroes[1];
                Managers.Game.Leader = newLeader;
                for(int i=0; i< heroes.Count; i++)
                {
                    heroes[i].MyLeader = newLeader;
                }
            }

            Managers.Hero.UnpickHero(_heroDataId);

            _isSet = false;
        }
        else
        {
            Managers.Hero.PickHero(_heroDataId, Vector3Int.zero);

            _isSet = true;
        }

        GetImage((int)Images.SetHeroImage).gameObject.SetActive(_isSet);
    }

    private void OnDisable()
    {
        _heroDataId = -1;
    }
}
