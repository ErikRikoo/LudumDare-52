using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDamageTrigger : MonoBehaviour, IDamageable
{
    public float Health { get; set; }
    public System.Guid id;
    public event InformAttackersAboutDeath InformAboutDeath;
    public LandPlot LandPlot { get; set; }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0.1f)
        {
            InformAboutDeath?.Invoke(gameObject);
            LandPlot.PlantDestroyed(this.id);
        }
    }
}
