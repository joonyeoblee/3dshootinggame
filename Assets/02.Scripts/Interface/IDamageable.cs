﻿public interface IDamageable
{
    int Health { get; set; }
    void TakeDamage(Damage damage);
}
