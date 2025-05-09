using UnityEngine;
public class AttackEvent : MonoBehaviour
{
    private Enemy _enemy;
    public GameObject Barrel;
    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }
    public void DealDamage()
    {
        Debug.Log("애니메이션 이벤트로 공격함!");

        if (_enemy.Player == null) return;

        Damage damage = new Damage(_enemy.Stat.AttackDamage, 0, gameObject);
        _enemy.Player.GetComponent<Player>().TakeDamage(damage);
    }

    public void ThrowBarrel()
    {
        _enemy.Barrel.SetActive(false);

        GameObject barrel = Instantiate(Barrel, _enemy.transform.position, Quaternion.identity);
        barrel.transform.position = _enemy.Muzzle.transform.position;

        // 플레이어 위치 주변 반경 20 내 랜덤 위치 생성
        Vector3 playerPos = GameManager.Instance.Player.transform.position;
        Vector2 randomCircle = Random.insideUnitCircle * 5f;
        Vector3 targetPos = playerPos + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // 발사 방향 계산
        Vector3 direction = (targetPos - _enemy.Muzzle.transform.position).normalized;

        Rigidbody bombRigidbody = barrel.GetComponent<Rigidbody>();
        bombRigidbody.AddForce(direction * 20f, ForceMode.Impulse);
        bombRigidbody.AddTorque(Vector3.one);

        barrel.GetComponent<Barrel>().IsThrow = true;
    }


}
