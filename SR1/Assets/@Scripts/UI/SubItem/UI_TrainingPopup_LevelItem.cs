using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

public class UI_TrainingPopup_LevelItem : UI_Base
{
    enum Images
    {
        Stat1LineImage,
        Stat1Image,
        Stat1UnlockImage,
        
        Stat2LineImage,
        Stat2Image,
        Stat2UnlockImage,
        
        Stat3LineImage,
        Stat3Image,
        Stat3UnlockImage,
        
        AbilityImage,
        AbilityLineImage,
        AbilityUnlockImage
    }

    enum Texts
    {
        LevelText,
        Stat1ValueText,
        Stat2ValueText,
        Stat3ValueText,
        AbilityValueText,
    }

    enum Buttons
    {
        Stat1Button,
        Stat2Button,
        Stat3Button,
        AbilityButton,
    }

    int _level = -1;
    private List<TrainingData> _trainingDatas = new List<TrainingData>();
    private TrainingData _mainOptionData;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.Stat1Button).gameObject.BindEvent(OnClickStat1Button);
        GetButton((int)Buttons.Stat2Button).gameObject.BindEvent(OnClickStat2Button);
        GetButton((int)Buttons.Stat3Button).gameObject.BindEvent(OnClickStat3Button);
        GetButton((int)Buttons.AbilityButton).gameObject.BindEvent(OnClickAbilityButton);

        return true;
    }

    public void SetInfo(int level, bool isMaxItem)
    {
        _trainingDatas.Clear();
        _level = level;

        _trainingDatas = Managers.Data.TrainingDic.Values.Where(x => x.RequiredLevel == level).ToList();
        _mainOptionData = _trainingDatas.FirstOrDefault(x => x.isMainOption);
        
        HideLastItem(isMaxItem);
        
        Refresh();
    }

    void HideLastItem(bool isMaxItem)
    {
        GetImage((int)Images.Stat3LineImage).gameObject.SetActive(!isMaxItem);

        int maxMainId = Managers.Data.TrainingDic.Values
            .Where(x => x.isMainOption)
            .Max(x => x.TemplateId);

        bool showAbilityLineImage = true;
        if (_mainOptionData == null)
        {
            showAbilityLineImage = !isMaxItem;
        }
        else if (_mainOptionData.TemplateId == maxMainId)
        {
            showAbilityLineImage = false;
        }

        GetImage((int)Images.AbilityLineImage).gameObject.SetActive(showAbilityLineImage);
    }

    void Refresh()
    {
        if (_init == false)
            return;
        if(_level == 0)
            return;
        
        GetText((int)Texts.LevelText).text = $"{_level}";

        if(Managers.Data.TrainingDic.ContainsKey(_level+1) == false)
        {
            GetImage((int)Images.AbilityLineImage).gameObject.SetActive(false);
            GetImage((int)Images.Stat3LineImage).gameObject.SetActive(false);
        }

        //1. Option1 = Index0
        if (_trainingDatas[0] != null)
        {
            GetImage((int)Images.Stat1Image).sprite = Managers.Resource.Load<Sprite>(_trainingDatas[0].IconName);
            string str = Managers.GetText(_trainingDatas[0].CalcStatType.ToString());
            GetText((int)Texts.Stat1ValueText).text = $"{str} + {_trainingDatas[0].OptionValue}";
            bool isUnlock = Managers.Game.UnlockedTrainings.Contains(_trainingDatas[0].TemplateId);
            GetImage((int)Images.Stat1UnlockImage).gameObject.SetActive(isUnlock);
        }
        //2. Option2 = Index1
        if (_trainingDatas[1] != null)
        {
            GetImage((int)Images.Stat2Image).sprite = Managers.Resource.Load<Sprite>(_trainingDatas[1].IconName);
            string str = Managers.GetText(_trainingDatas[1].CalcStatType.ToString());
            GetText((int)Texts.Stat2ValueText).text = $"{str} + {_trainingDatas[1].OptionValue}";
            bool isUnlock = Managers.Game.UnlockedTrainings.Contains(_trainingDatas[1].TemplateId);
            GetImage((int)Images.Stat2UnlockImage).gameObject.SetActive(isUnlock);
            
        }
        //3. Option3 = Index2
        if (_trainingDatas[2] != null)
        {
            GetImage((int)Images.Stat3Image).sprite = Managers.Resource.Load<Sprite>(_trainingDatas[2].IconName);
            string str = Managers.GetText(_trainingDatas[2].CalcStatType.ToString());
            GetText((int)Texts.Stat3ValueText).text = $"{str} + {_trainingDatas[2].OptionValue}";
            bool isUnlock = Managers.Game.UnlockedTrainings.Contains(_trainingDatas[2].TemplateId);
            GetImage((int)Images.Stat3UnlockImage).gameObject.SetActive(isUnlock);
        }
        
        //MainOption이 있는경우
        if (_trainingDatas.Count < 4)
        {
            GetButton((int)Buttons.AbilityButton).gameObject.SetActive(false);
        }
        else
        {
            GetButton((int)Buttons.AbilityButton).gameObject.SetActive(true);
            GetImage((int)Images.AbilityImage).sprite = Managers.Resource.Load<Sprite>(_trainingDatas[3].IconName);

            if (_trainingDatas[3].CalcStatType == Define.ECalcStatType.None)
            {
                string str = Managers.GetText(_trainingDatas[3].DescTextId);
                GetText((int)Texts.AbilityValueText).text = $"{str}";
            }
            else
            {
                string str = Managers.GetText(_trainingDatas[3].CalcStatType.ToString());
                GetText((int)Texts.AbilityValueText).text = $"{str}+{_trainingDatas[3].OptionValue}%";
            }
            bool isUnlock = Managers.Game.UnlockedTrainings.Contains(_trainingDatas[3].TemplateId);
            GetImage((int)Images.AbilityUnlockImage).gameObject.SetActive(isUnlock);
        }
    }

    void OnClickStat1Button()
    {
        if (_trainingDatas[0] != null)
        {
            if(Managers.Inventory.SpendCurrency(_trainingDatas[0].currencyType, _trainingDatas[0].Price))
            {
                Managers.Game.UnLockTraining(_trainingDatas[0].TemplateId);
                Refresh();
            }
        }    
    }

    void OnClickStat2Button()
    {
        if (_trainingDatas[1] != null)
        {
            if(Managers.Inventory.SpendCurrency(_trainingDatas[1].currencyType, _trainingDatas[1].Price))
            {
                Managers.Game.UnLockTraining(_trainingDatas[1].TemplateId);
                Refresh();
            }
        }        
    }

    void OnClickStat3Button()
    {
        if (_trainingDatas[2] != null)
        {
            if(Managers.Inventory.SpendCurrency(_trainingDatas[2].currencyType, _trainingDatas[2].Price))
            {
                Managers.Game.UnLockTraining(_trainingDatas[2].TemplateId);
                Refresh();
            }
        }        
    }

    void OnClickAbilityButton()
    {
        if (_trainingDatas[3] != null)
        {
            if(Managers.Inventory.SpendCurrency(_trainingDatas[3].currencyType, _trainingDatas[3].Price))
            {
                Managers.Game.UnLockTraining(_trainingDatas[3].TemplateId);
                Refresh();
            }
        }
    }
}
