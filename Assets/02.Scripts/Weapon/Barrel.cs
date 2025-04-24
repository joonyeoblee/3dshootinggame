using System.Collections;
using UnityEngine;
public class Barrel : MonoBehaviour, IDamageable
{
    public int Health { get; set; }

    private Explore _explore;
    private readonly float _deleteTime = 7f;
    private bool _isDead;

    public GameObject BeforeExploreObject;
    public GameObject AfterExploreObject;

    private Rigidbody _rigidbody;

    public float explosionForce = 2f;

    private void Start()
    {
        Health = 100;
        _explore = GetComponent<Explore>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void TakeDamage(Damage damage)
    {
        if (_isDead)
        {
            return;
        }
        Debug.Log(damage);
        Health -= damage.Value;
        if (Health <= 0)
        {
            OnDeath();
        }
    }
    public void OnDeathExplode()
    {
        StartCoroutine(Delete());

        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            1f,
            Random.Range(-1f, 1f)
        ).normalized;

        _rigidbody.AddForce(randomDir * explosionForce, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(
            1f,
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * 10f;

        _rigidbody.AddTorque(randomTorque, ForceMode.Impulse);

    }
    private void OnDeath()
    {
        _isDead = true;

        _explore.Explode();

        BeforeExploreObject.SetActive(false);
        AfterExploreObject.SetActive(true);

        OnDeathExplode();

    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(_deleteTime);
        Destroy(gameObject);
    }
}
