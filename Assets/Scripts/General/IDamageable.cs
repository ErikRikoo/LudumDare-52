using System;
using Mono.Cecil.Cil;

namespace General
{
    public delegate void InformAttackersAboutDeath();

    public interface IDamageable
    {
        public void TakeDamage(float amount);
        event InformAttackersAboutDeath InformAboutDeath;
    }
}