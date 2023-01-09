using UnityEngine;

namespace Player.PlayerActions.Harvest
{
    public interface IHarvestable
    {
        void OnHarvested(GameObject _harvester);
    }
}