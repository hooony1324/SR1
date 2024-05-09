using System.Collections.Generic;
using UnityEngine;

public class UI_TrainingPopup : UI_Popup
{
    enum GameObjects
    {
        CloseArea,
        CommonContentsArea,
        CommonTrainingList,

        //SpecialContentsArea,
        PurchasedSlot1,
        UnPurchasedSlot1,
        PurchasedSlot2,
        UnPurchasedSlot2,
        PurchasedSlot3,
        UnPurchasedSlot3,
        PurchasedSlot4,
        UnPurchasedSlot4,
    }

    enum Buttons
    {
        /*        Purchase1Button,
                Purchase2Button,
                Purchase3Button,
                Purchase4Button,*/
        CloseButton,
    }

    enum Texts
    {
        PieceCount1Text,
        Purchase1Text,
        CostCount1Text,
        PieceCount2Text,
        Purchase2Text,
        CostCount2Text,
        PieceCount3Text,
        Purchase3Text,
        CostCount3Text,
        PieceCount4Text,
        Purchase4Text,
        CostCount4Text,
    }

    /*    enum Toggles
        {
            CommonToggle,
            SpecialToggle,
        }*/

    enum Sliders
    {
        RemainTime1Slider,
        RemainTime2Slider,
        RemainTime3Slider,
        RemainTime4Slider,
    }

    const int MAX_ITEM_COUNT = 100;
    List<UI_TrainingPopup_LevelItem> _levelItems = new List<UI_TrainingPopup_LevelItem>();

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        //BindToggle(typeof(Toggles));
        BindSlider(typeof(Sliders));

        GetObject((int)GameObjects.CloseArea).BindEvent(OnClickCloseArea);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
        /*        GetButton((int)Buttons.Purchase1Button).gameObject.BindEvent(OnClickPurchase1Button);
                GetButton((int)Buttons.Purchase2Button).gameObject.BindEvent(OnClickPurchase2Button);
                GetButton((int)Buttons.Purchase3Button).gameObject.BindEvent(OnClickPurchase3Button);
                GetButton((int)Buttons.Purchase4Button).gameObject.BindEvent(OnClickPurchase4Button);*/

        //GetToggle((int)Toggles.CommonToggle).gameObject.BindEvent(OnClickCommonToggle);
        //GetToggle((int)Toggles.SpecialToggle).gameObject.BindEvent(OnClickSpecialToggle);
        //
        //GetObject((int)GameObjects.SpecialContentsArea).SetActive(false);

        Transform parent = GetObject((int)GameObjects.CommonTrainingList).transform;
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_TrainingPopup_LevelItem item = Managers.UI.MakeSubItem<UI_TrainingPopup_LevelItem>(parent);
            _levelItems.Add(item);
        }

        return true;
    }

    public void SetInfo()
    {
        //OnClickCommonToggle();
        Refresh();
    }

    void Refresh()
    {
        if (_init == false)
            return;

        RectTransform rectTransform = GetObject((int)GameObjects.CommonTrainingList).GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;

        int maxId = Managers.Data.TrainingDic.Count;
        int maxLevel = Managers.Data.TrainingDic[maxId].RequiredLevel;

        int dataId = 1;

        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            //MaxLevel
            if (i <= maxLevel && i > 0)
            {
                _levelItems[i].gameObject.SetActive(true);
                _levelItems[i].SetInfo(i, i == maxLevel);
            }
            else
            {
                _levelItems[i].gameObject.SetActive(false);
            }
        }
    }

    void OnClickCloseArea()
    {
        ClosePopupUI();
    }

    void OnClickCloseButton()
    {
        ClosePopupUI();
    }

    void OnClickPurchase1Button()
    {
        Debug.Log("On Click Purchase1 Button");
    }

    void OnClickPurchase2Button()
    {
        Debug.Log("On Click Purchase2 Button");
    }

    void OnClickPurchase3Button()
    {
        Debug.Log("On Click Purchase3 Button");
    }

    void OnClickPurchase4Button()
    {
        Debug.Log("On Click Purchase4 Button");
    }

    /*    void OnClickCommonToggle()
        {
            if (GetObject((int)GameObjects.CommonContentsArea).activeSelf)
                return;

            GetObject((int)GameObjects.CommonContentsArea).SetActive(true);
            GetObject((int)GameObjects.SpecialContentsArea).SetActive(false);
        }

        void OnClickSpecialToggle()
        {
            if (GetObject((int)GameObjects.SpecialContentsArea).activeSelf)
                return;

            GetObject((int)GameObjects.CommonContentsArea).SetActive(false);
            GetObject((int)GameObjects.SpecialContentsArea).SetActive(true);
        }*/
}