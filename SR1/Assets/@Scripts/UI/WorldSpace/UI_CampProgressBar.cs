using UnityEngine;
using UnityEngine.UI;

public class UI_CampProgressBar : UI_Base
{
    private InteractionObject _owner;
    private Slider _slider;
    private GameObject _timer;
    private Image _timerImage;
    private enum GameObjects
    {
        HPBar,
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
        _slider.value = 1;

        return true;
    }

    public void SetInfo(InteractionObject owner, float time)
    {
        _owner = owner;
        _slider.maxValue = time;
        _slider.value = 0;

        // transform.localPosition = Vector3.up * (_owner.GetSpineHeight() * 1.1f);
        Refresh(0);
    }

    public void Refresh(float value)
    {
        _slider.gameObject.SetActive(true);
        _slider.value = value;
    }
}
