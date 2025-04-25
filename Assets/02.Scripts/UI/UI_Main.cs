using System;
using System.Collections;
using DG.Tweening;
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

    [SerializeField] private Image _attackImage;
    public Crosshair Crosshair;

    public Action OnHit;
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

    public void ActiveAttackImage()
    {
        Color color = _attackImage.color;
        color.a = 1f;
        _attackImage.color = color;

        // _attackImage.DOFade(0f, 1f);
        StartCoroutine(Fade_Coroutine(1f));
    }

    private IEnumerator Fade_Coroutine(float duration)
    {
        float elapsed = 0f;
        Color color = _attackImage.color;
        float startAlpha = color.a;
        float endAlpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            color.a = alpha;
            _attackImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        _attackImage.color = color;
    }
}
