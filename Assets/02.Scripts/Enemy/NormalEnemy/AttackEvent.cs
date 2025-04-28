using UnityEngine;
public class AttackEvent : MonoBehaviour
{
    private Enemy _enemy;
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
}
