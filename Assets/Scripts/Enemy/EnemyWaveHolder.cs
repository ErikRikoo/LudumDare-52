using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class SpawnTypeData
{
    public GameObject enemyPrefab;
    public int numberOfSpaws = 1;
    public float delay = 0;

}

[Serializable]
public class Wave
{
    public SpawnTypeData[] waveData;

    public Wave()
    {
        waveData = new[] { new SpawnTypeData()};
    }


}

[CreateAssetMenu(fileName = "WaveHolder", menuName = "Enemies/WaveHolder")]
public class EnemyWaveHolder : ScriptableObject
{
    public List<Wave> waves;

}
