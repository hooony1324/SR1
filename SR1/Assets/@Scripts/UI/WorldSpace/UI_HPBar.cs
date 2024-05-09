using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    private Creature _owner;
    private Slider _slider;
    private GameObject _timer;
    private Image _timerImage;
    private enum GameObjects
    {
        HPBar,
        Timer,
        TimerImage,
    }

    private enum Images
    {
        TimerImage,
    }

    
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        
        GetComponent<Canvas>().sortingOrder = SortingLayers.HERO + 1;
        _slider = GetObject((int)GameObjects.HPBar).GetComponent<Slider>();
        _timer = GetObject((int)GameObjects.Timer);
        _timerImage = GetImage((int)Images.TimerImage);

        _timerImage.fillAmount = 0;
        _slider.value = 1;

        return true;
    }

    public void SetInfo(Creature owner)
    {
        _owner = owner;
        _slider.value = 1;

        transform.localPosition = Vector3.up * (_owner.GetSpineHeight() * 1.1f);
        Refresh(1);
    }

    public void Refresh(float ratio)
    {
        if (_owner.CreatureState == Define.ECreatureState.Dead)
        {
            _slider.gameObject.SetActive(false);
            _timer.gameObject.SetActive(true);
            _timerImage.fillAmount = ratio;
        }
        else
        {
            _slider.gameObject.SetActive(true);
            _timer.gameObject.SetActive(false);
            _slider.value = ratio;
        }
    }
}
