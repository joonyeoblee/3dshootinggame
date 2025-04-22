using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Main : MonoBehaviour
{
    public static UI_Main Instance;

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _bombGaugeSlider;

    [SerializeField] private TMP_Text _BulletText;
    [SerializeField] private TMP_Text _bombText;

    void Awake()
    {
        Instance = this;
    }

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

    }
}
