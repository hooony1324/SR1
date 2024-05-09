using System.Collections.Generic;
using UnityEngine;

public class UI_WorldmapPopup : UI_Popup
{
    #region enum

    enum GameObjects
    {
    }

    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        RawMapImage,
        PlayerImage
    }

    #endregion

    public RectTransform _heroMarker;

    public List<UI_WorldmapItem> _npcMarker;

    public RectTransform _mapImage;
    public Vector2 _offset;

    private Vector3 _playerPos; //player
    private Vector3[] _waypoints; //player
    private Vector3[] _portals; //player
    private Vector2 _imageDimentions;
    private SpriteRenderer _mapbound;

    const int MAX_ITEM_COUNT = 15;
    List<UI_WorldmapItem> _worldItems = new List<UI_WorldmapItem>();
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        var parent = _mapImage.transform;
        for (int i = 0; i < MAX_ITEM_COUNT; i++)
        {
            UI_WorldmapItem item = Managers.UI.MakeSubItem<UI_WorldmapItem>(parent);
            _worldItems.Add(item);
        }
        
        return true;
    }

    public void SetInfo()
    {
        _npcMarker.Clear();
        _mapbound = Managers.Map.StageTransition.MapBound;
        _playerPos = Managers.Game.Leader.Position;
        _heroMarker = GetImage((int)Images.PlayerImage).rectTransform;
        _mapImage = GetImage(((int)Images.RawMapImage)).rectTransform;
        _imageDimentions = new Vector2(_mapImage.sizeDelta.x, _mapImage.sizeDelta.y);

        List<Npc> npcs = new List<Npc>();
        // 웨이포인트
        foreach (var npc in Managers.Object.Npcs)
        {
            switch (npc.Data.NpcType)
            {
                case Define.ENpcType.StartPosition:
                    continue;
                case Define.ENpcType.Portal:
                    PortalInteraction pi = (PortalInteraction)npc.Interaction;
                    if (pi.IsTownPortal == false)
                        npcs.Add(npc);
                    break;
                case Define.ENpcType.Waypoint:
                    npcs.Add(npc);
                    break;
            }
        }
        
        for (int i = 0; i <MAX_ITEM_COUNT; i++)
        {
            if (i < npcs.Count)
            {
                Npc npc = npcs[i];
                _worldItems[i].gameObject.SetActive(true);
                _worldItems[i].SetInfo(npc.Data.NpcType, npc.Position, Managers.Data.NpcDic[npc.TemplateId].IconSpriteName, OnClickCloseButton );
                _worldItems[i].MapIndex = npc.SpawnStage.StageIndex;
                _npcMarker.Add(_worldItems[i]);
            }
            else
            {
                _worldItems[i].gameObject.SetActive(false);
            }
        }

        Refresh();
    }
    
    private void Refresh()
    {
        if (_init == false)
            return;
        
        _playerPos = Managers.Game.Leader.Position;

        Vector3 distance = _playerPos - _mapbound.bounds.min;
        Vector2 coordinates = new Vector2(distance.x / _mapbound.size.x, distance.y / _mapbound.size.y);
        _heroMarker.anchoredPosition =
            new Vector2(coordinates.x * _imageDimentions.x, coordinates.y * _imageDimentions.y) + _offset;

        foreach (var npc in _npcMarker)
        {
            Vector3 dist = npc.GetWorldPosition() - _mapbound.bounds.min;
            Vector2 coord = new Vector2(dist.x / _mapbound.size.x, dist.y / _mapbound.size.y);
            npc.SetAnchoredPosition(new Vector2(coord.x * _imageDimentions.x, coord.y * _imageDimentions.y) + _offset);
        }
    }

    private void OnClickCloseButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}