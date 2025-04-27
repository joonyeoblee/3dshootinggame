using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public PlayerSO PlayerData;

    public GameObject Model;

    public Animator Animator;

    public int Health { get; set; }

    public bool GunMode;
    public bool KnifeMode;

    private void Start()
    {
        Health = PlayerData.MaxHealth;

        Animator = Model.GetComponent<Animator>();
    }

    public void TakeDamage(Damage damage)
    {
        Health -= damage.Value;
        UI_Main.Instance.RefreshHealthSlider(Health);
        Debug.Log($"{name} damage dealt from {damage.Value} to {Health}");
        UI_Main.Instance.ActiveAttackImage();

        if (Health <= 0)
        {
            Die();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GunMode = true;
            KnifeMode = false;
            Animator.SetTrigger("Gun");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GunMode = false;
            KnifeMode = true;
            Animator.SetTrigger("Knife");
        }
    }

    private void Die()
    {
        GameManager.Instance.EndGame();
    }
}
