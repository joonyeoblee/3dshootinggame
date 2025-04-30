using DG.Tweening;
using UnityEngine;
public class Player : MonoBehaviour, IDamageable
{
    public PlayerSO PlayerData;

    public GameObject Model;

    public Animator Animator;

    public int Health { get; set; }

    private int _weaponIndex;
    public bool GunMode;
    public bool KnifeMode;
    public bool BombMode;

    // 추가: 상체 본
    private Transform spineBone;
    private Transform chestBone;


    [SerializeField] private GameObject _knife;
    private void Start()
    {
        Health = PlayerData.MaxHealth;

        Animator = Model.GetComponent<Animator>();

        GunMode = true;

        UI_Main.Instance.ChangeWeaponSprite(_weaponIndex);
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
        Animator.SetLayerWeight(2, 1 - Health / PlayerData.MaxHealth - 0.5f);

        // 마우스 휠로 _weaponIndex 변경
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            _weaponIndex = (_weaponIndex + 1) % 3;
            ApplyWeaponMode();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            _weaponIndex = (_weaponIndex - 1 + 3) % 3;
            ApplyWeaponMode();
        }

        // 숫자 키로 모드 변경
        if (Input.GetKeyDown(KeyCode.Alpha1) && !GunMode)
        {
            _weaponIndex = 0;

            ApplyWeaponMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !KnifeMode)
        {
            _weaponIndex = 1;
            ApplyWeaponMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !BombMode)
        {
            _weaponIndex = 2;
            ApplyWeaponMode();
        }
    }

    private void ApplyWeaponMode()
    {
        switch (_weaponIndex)
        {
            case 0: // 총
                if (!GunMode)
                {
                    GunMode = true;
                    KnifeMode = false;
                    BombMode = false;

                    _knife.SetActive(false);
                    SetModelRotation();

                    DOTween.To(
                        () => Animator.GetFloat("GunKnife"),
                        x => Animator.SetFloat("GunKnife", x),
                        0f,
                        0.2f
                    );
                }
                break;

            case 1: // 칼
                if (!KnifeMode)
                {
                    GunMode = false;
                    KnifeMode = true;
                    BombMode = false;

                    _knife.SetActive(true);

                    DOTween.To(
                        () => Animator.GetFloat("GunKnife"),
                        x => Animator.SetFloat("GunKnife", x),
                        1f,
                        0.2f
                    );
                }
                break;

            case 2: // 폭탄
                if (!BombMode)
                {
                    GunMode = false;
                    KnifeMode = false;
                    BombMode = true;

                    _knife.SetActive(false);
                }
                break;
        }
        UI_Main.Instance.ChangeWeaponSprite(_weaponIndex);
    }


    public void SetModelRotation()
    {
        Model.transform.localRotation = Quaternion.Euler(0f, 40f, 0f);
        Model.transform.localPosition = new Vector3(0.058f, -1f, -0.02f);
    }




    private void Die()
    {
        GameManager.Instance.EndGame();
    }
}
