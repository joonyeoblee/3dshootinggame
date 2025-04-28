using UnityEngine;
public class Player : MonoBehaviour, IDamageable
{
    public PlayerSO PlayerData;

    public GameObject Model;

    public Animator Animator;

    public int Health { get; set; }

    public bool GunMode;
    public bool KnifeMode;

    // 추가: 상체 본
    private Transform spineBone;
    private Transform chestBone;

    private void Start()
    {
        Health = PlayerData.MaxHealth;

        Animator = Model.GetComponent<Animator>();

        GunMode = true;
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

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GunMode = true;
            KnifeMode = false;
            Animator.SetTrigger("Gun");

            // 총 모드 전환 시, Model 회전 및 위치 보정
            SetModelRotation();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GunMode = false;
            KnifeMode = true;
            Animator.SetTrigger("Knife");
        }

        Animator.SetLayerWeight(2, 1 - Health / PlayerData.MaxHealth - 0.5f);

    }

    private void SetModelRotation()
    {
        Model.transform.localRotation = Quaternion.Euler(0f, 40f, 0f);
        Model.transform.localPosition = new Vector3(0.058f, -1f, -0.02f);
    }




    private void Die()
    {
        GameManager.Instance.EndGame();
    }
}
