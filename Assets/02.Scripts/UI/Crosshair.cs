using UnityEngine;
public class Crosshair : MonoBehaviour
{
    [SerializeField] private float _min = 10f;
    [SerializeField] private float _max = 35f;

    [SerializeField] private RectTransform _crosshairUp;
    [SerializeField] private RectTransform _crosshairDown;
    [SerializeField] private RectTransform _crosshairLeft;
    [SerializeField] private RectTransform _crosshairRight;

    [SerializeField] private float _recoilStep;
    [SerializeField] private float _recoverySpeed;

    private float _spreadAmount;

    private void Start()
    {
        _spreadAmount = _min;
        ApplySpread();
    }

    private void Update()
    {
        // 조준선 회복
        if (_spreadAmount > _min)
        {
            _spreadAmount -= _recoverySpeed * Time.deltaTime;
            _spreadAmount = Mathf.Max(_spreadAmount, _min);
            ApplySpread();
        }
    }

    public void Recoil()
    {
        _spreadAmount += _recoilStep;
        _spreadAmount = Mathf.Min(_spreadAmount, _max);
        ApplySpread();
    }

    private void ApplySpread()
    {
        _crosshairUp.anchoredPosition = new Vector2(0, _spreadAmount);
        _crosshairDown.anchoredPosition = new Vector2(0, -_spreadAmount);
        _crosshairLeft.anchoredPosition = new Vector2(-_spreadAmount, 0);
        _crosshairRight.anchoredPosition = new Vector2(_spreadAmount, 0);
    }
}
