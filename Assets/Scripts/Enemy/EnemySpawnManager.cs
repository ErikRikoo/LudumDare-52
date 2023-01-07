using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private EnemyWaveHolder waves;
    [SerializeField] private float spawnRadius;
    
    private int currentWave = 0;

    static int numberOfEnemiesOnScreen = 0;

    private Vector3 RandomPointOnCircleEdge(float minRadius, float maxRadius)
    {
        var radius = Random.Range(minRadius, maxRadius);
        var vector2 = Random.insideUnitCircle.normalized * radius;
        return new Vector3(vector2.x, 0, vector2.y);
    }
    
    private IEnumerator SpawnAllEnemiesInWave(Wave wave)
    {
        foreach (var spawnPod in wave.waveData)
        {
            yield return new WaitForSeconds(spawnPod.delay);
            
            for (var i = 0; i < spawnPod.numberOfSpaws; i++)
            {
                Instantiate(spawnPod.enemyPrefab);
                numberOfEnemiesOnScreen++;
            }

        }
    }

    public void SpawnWave()
    {
        
        

        currentWave++;
    }
    
}
