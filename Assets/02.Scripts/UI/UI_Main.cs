using System.Collections;
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

    [SerializeField] private float _waitTime = 1f;
    [SerializeField] private GameObject _gameState;
    [SerializeField] private TMP_Text _gameStateText;

    [SerializeField] private Image _sniperImage;

    [SerializeField] private Image _weaponImage;
    [SerializeField] private Sprite[] _weaponSprite;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(GameState_Coroutine());
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

        // dotween 사용시..
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

    private IEnumerator GameState_Coroutine()
    {

        string[] messages =
        {
            "Ready.",
            "Ready..",
            "Ready...",
            "Start!"
        };

        foreach(string message in messages)
        {
            _gameStateText.text = message;
            yield return new WaitForSeconds(_waitTime);
        }
        GameManager.Instance.IsPlaying = true;
        _gameState.SetActive(false);
    }

    public void EndGame()
    {
        _gameState.SetActive(true);
        _gameStateText.text = "Game Over!";
    }

    public void ToggleSniperImage()
    {
        // false true
        bool isActive = _sniperImage.gameObject.activeSelf;
        Crosshair.gameObject.SetActive(isActive);
        _sniperImage.gameObject.SetActive(!isActive);
    }

    public void ChangeWeaponSprite(int index)
    {
        _weaponImage.sprite = _weaponSprite[index];
    }
}
