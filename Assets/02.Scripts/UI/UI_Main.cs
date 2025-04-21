using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : MonoBehaviour
{
    public static UI_Main Instance;

    [SerializeField] Slider _stamina;

    void Awake()
    {
        Instance = this;
    }
    public void RefreshStamina(float value)
    {
        _stamina.value = value;
    }
}
