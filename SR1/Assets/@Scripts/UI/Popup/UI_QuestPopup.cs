using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_QuestPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        QuestList,
    }

    enum Texts
    {
        RewardPointText,
    }

    enum Images
    {
        // Reward20Image,
        // Reward40Image,
        // Reward60Image,
        // Reward80Image,
        // Reward100Image,
    }

    enum Toggles
    {
        DailyToggle,
        WeeklyToggle,
    }

    enum Buttons
    {
        CloseButton,
    }

    enum Sliders
    {
        RewardSlider,
    }
    
    List<UI_QuestItem> _questItems = new List<UI_QuestItem>();
    private Toggle _dailyToggle;
    private Toggle _weeklyToggle;
    const int MAX_ITEM_COUNT = 30;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindToggle(typeof(Toggles));
        BindButton(typeof(Buttons));
        BindSlider(typeof(Sliders));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        GetSlider((int)Sliders.RewardSlider).minValue = 0;
        GetSlider((int)Sliders.RewardSlider).maxValue = 100;
        
        _dailyToggle = GetToggle((int)Toggles.DailyToggle);
        _weeklyToggle = GetToggle((int)Toggles.WeeklyToggle);
        
        _dailyToggle.gameObject.BindEvent(OnClickDailyToggle);
        _weeklyToggle.gameObject.BindEvent(OnClickWeeklyToggle);
        
        GetObject((int)GameObjects.QuestList).DestroyChilds();
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_QuestItem item = Managers.UI.MakeSubItem<UI_QuestItem>(GetObject((int)GameObjects.QuestList).transform);
            _questItems.Add(item);
        }
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
        
        //TODO 퀘스트 정렬
        //
        
        if(_dailyToggle.isOn)
        {
            int score = Managers.Quest.GetQuestScore(EQuestRewardType.DailyScore);
            GetText((int)Texts.RewardPointText).text = $"{score}";
            GetSlider((int)Sliders.RewardSlider).value = score;

            List<Quest> quests = Managers.Quest.GetDailyQuests();
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < quests.Count)
                {
                    _questItems[i].gameObject.SetActive(true);
                    _questItems[i].SetInfo(quests[i].TemplateId, Refresh);
                }
                else
                {
                    _questItems[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            int score = Managers.Quest.GetQuestScore(EQuestRewardType.DailyScore);
            GetText((int)Texts.RewardPointText).text = $"{score}";
            GetSlider((int)Sliders.RewardSlider).value = score;
            List<Quest> quests = Managers.Quest.GetWeeklyQuests();
            
            for (int i = 0; i < MAX_ITEM_COUNT; i++)
            {
                if (i < quests.Count)
                {
                    _questItems[i].gameObject.SetActive(true);
                    _questItems[i].SetInfo(quests[i].TemplateId, Refresh);
                }
                else
                {
                    _questItems[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void OnClickCloseArea()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickReward20Image()
    {
        Debug.Log("On Click Reward20Image");
    }

    void OnClickReward40Image()
    {
        Debug.Log("On Click Reward40Image");
    }

    void OnClickReward60Image()
    {
        Debug.Log("On Click Reward60Image");
    }

    void OnClickReward80Image()
    {
        Debug.Log("On Click Reward80Image");
    }

    void OnClickReward100Image()
    {
        Debug.Log("On Click Reward100Image");
    }

    void OnClickDailyToggle()
    {
        Debug.Log("On Click DailyToggle");
        Refresh();
    }

    void OnClickWeeklyToggle()
    {
        Debug.Log("On Click WeeklyToggle");
        Refresh();
    }

}