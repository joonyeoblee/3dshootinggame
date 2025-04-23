using UnityEngine;
public struct Damage
{
    public int Value;
    public int WeaponPower;
    public GameObject DamageFrom;

    public Damage(int value, int weaponPower, GameObject damageFrom)
    {
        Value = value;
        WeaponPower = weaponPower;
        DamageFrom = damageFrom;
    }
}
