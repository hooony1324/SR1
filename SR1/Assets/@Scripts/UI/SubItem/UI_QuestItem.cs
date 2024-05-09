using System;
using Data;
using UnityEngine;
using static Define;

public class UI_QuestItem : UI_SubItem
{
    enum Texts
    {
        RewardCountText,
        MoveToQuestText,                 //현지화 용
        CompletedText,                   //현지화 용
        CompletedRewardText,             //현지화 용
        QuestDescriptionText,
    }

    enum Buttons
    {
        MoveToQuestButton,
        CompletedButton,
        CompletedRewardButton,
    }

    enum Images
    {
        RewardImage,
    }

    enum Sliders
    {
        ProgressSlider,
    }

    private Quest _quest;
    private QuestTask _questTask;
    private Action OnUpdateItem;
    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindSlider(typeof(Sliders));
        BindImage(typeof(Images));

        GetButton((int)Buttons.CompletedButton).gameObject.BindEvent(OnClickCompleteButton);
        GetButton((int)Buttons.MoveToQuestButton).gameObject.BindEvent(OnClickMoveToQuestButton);

        Refresh();

        return true;
    }

    public void SetInfo(int templateId, Action action)
    {
        _quest = Managers.Quest.GetQuest(templateId);
        OnUpdateItem = action;
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        if (_quest == null)
            return;

        QuestData questData = _quest.QuestData;
        _questTask = _quest.GetCurrentTask();

        GetText((int)Texts.QuestDescriptionText).text = $"{ Managers.GetText(_questTask.TaskData.DescriptionTextId)}";
        GetText((int)Texts.RewardCountText).text = $"{_questTask.TaskData.RewardCount}";
        
        GetSlider((int)Sliders.ProgressSlider).value = (float)_questTask.Count / _questTask.TaskData.ObjectiveCount;
        
        GetImage((int)Images.RewardImage).sprite = Managers.Resource.Load<Sprite>(_questTask.TaskData.RewardIcon);
        
        if (_quest.State == EQuestState.Completed)
        {
            GetButton((int)Buttons.CompletedRewardButton).gameObject.SetActive(false);
            GetButton((int)Buttons.CompletedButton).gameObject.SetActive(true);
            GetButton((int)Buttons.MoveToQuestButton).gameObject.SetActive(false);
        }
        else if(_quest.State == EQuestState.Rewarded)
        {
            GetButton((int)Buttons.CompletedRewardButton).gameObject.SetActive(true);
            GetButton((int)Buttons.CompletedButton).gameObject.SetActive(false);
            GetButton((int)Buttons.MoveToQuestButton).gameObject.SetActive(false);
        }
        else
        {
            GetButton((int)Buttons.CompletedRewardButton).gameObject.SetActive(false);
            GetButton((int)Buttons.CompletedButton).gameObject.SetActive(false);
            GetButton((int)Buttons.MoveToQuestButton).gameObject.SetActive(true);
        }
    }

    void OnClickCompleteButton()
    {
        //Dragon : 버튼을 받았을때만 GetReward
        if(_quest.GetCurrentTask().IsCompleted())
            _quest.GetCurrentTask().GiveReward();

        Refresh();
        OnUpdateItem?.Invoke();
    }

    void OnClickMoveToQuestButton()
    {
        // TODO : TimeScale = 0f로 만들고 퀘스트 목적지로 카메라 이동시키기
        Debug.Log("On Click Move To Quest Button");
    }


}
