using DG.Tweening;
using UnityEngine;
public class Item : MonoBehaviour
{
    public float FlyDistance = 5f;
    private bool _isFlying;

    private void Update()
    {
        if (_isFlying) return;

        Vector3 playerPos = GameManager.Instance.Player.transform.position;
        float dist = Vector3.Distance(transform.position, playerPos);

        if (dist <= FlyDistance)
        {
            _isFlying = true;

            Vector3 start = transform.position;
            Vector3 end = playerPos;
            Vector3 control1 = start + Vector3.up * 2f;
            Vector3 control2 = end + Vector3.up * 2f;

            // DOTween이 start는 자동으로 넣어주므로, 아래 배열에 control1, control2, end만 넣는다
            Vector3[] bezierPoints =
            {
                control1,
                control2,
                end
            };

            transform.DOPath(bezierPoints, 0.8f, PathType.CubicBezier)
                .SetEase(Ease.InOutSine)
                .SetLookAt(0.1f)
                .OnComplete(() =>
                {
                    // 예: 아이템 획득 처리
                    Destroy(gameObject);
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
