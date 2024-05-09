using System;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_WorldmapItem : UI_SubItem
{
    private ENpcType _npcType;
    private Vector3 _worldPos;
    private string _spriteName;
    public event Action OnCloseItem;
    public Image IconImage;
    public int MapIndex = -1;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        gameObject.BindEvent(OnClickStageItem);
        IconImage = gameObject.GetComponent<Image>();
        Refresh();

        return true;
    }

    public void SetInfo(ENpcType type, Vector3 worldPos, string spriteName, Action action)
    {
        _npcType = type;
        _worldPos = worldPos;
        IconImage.sprite = Managers.Resource.Load<Sprite>(spriteName);
        OnCloseItem = action;

        Refresh();
    }

    public Vector3 GetWorldPosition()
    {
        return _worldPos;
    }

    public void SetAnchoredPosition(Vector2 pos)
    {
        IconImage.rectTransform.anchoredPosition = pos;
    }

    void Refresh()
    {

    }

    void OnClickStageItem()
    {
        switch (_npcType)
        {
            case ENpcType.Waypoint:
                Managers.Map.StageTransition.OnMapChanged(MapIndex);
                Managers.Game.TeleportHeroes(Managers.Map.World2Cell(_worldPos));
                OnCloseItem?.Invoke();
                break;
        }

    }
}