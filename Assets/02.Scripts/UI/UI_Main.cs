using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Main : Singleton<UI_Main>
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _bombGaugeSlider;
    [SerializeField] private Slider _reloadSlider;

    [SerializeField] private TMP_Text _BulletText;
    [SerializeField] private TMP_Text _bombText;

    public void RefreshHealthSlider(float value)
    {
        _healthSlider.value = value;
    }
    public void RefreshStaminaSlider(float value)
    {
        _staminaSlider.value = value;
    }
    public void RefreshBombGaugeSlider(float value)
    {
        _bombGaugeSlider.value = value;
    }

    public void RefreshBulletText(string text)
    {
        _BulletText.text = text;
    }

    public void RefreshBombText(string text)
    {
        _bombText.text = text;
    }

    public void ActiveReloadSlider()
    {
        _reloadSlider.gameObject.SetActive(true);
    }
    public void DeactiveReloadSlider()
    {
        _reloadSlider.gameObject.SetActive(false);
    }
    public void RefreshReloadSlider(float value)
    {
        _reloadSlider.value = value;
    }
}
