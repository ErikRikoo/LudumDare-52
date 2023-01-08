using System;
using Mono.Cecil.Cil;
using UnityEngine;

namespace General
{
    public delegate void InformAttackersAboutDeath(GameObject obj);

    public interface IDamageable
    {
        public void TakeDamage(float amount);
        event InformAttackersAboutDeath InformAboutDeath;
    }
}