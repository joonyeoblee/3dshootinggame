using UnityEngine;
using UnityEngine.UI;
public class UI_Main : MonoBehaviour
{
    public static UI_Main Instance;

    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

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
}
