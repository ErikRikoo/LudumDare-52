using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private EnemyWaveHolder enemyWaves;
        [SerializeField] private GameState gameState;
        [SerializeField] private float spawnRadius;
        [SerializeField] private float spawnArea;
        [SerializeField] private GameObject silo;
    
        [SerializeField] private int currentWave = 0;

        private Vector3 RandomPointOnCircleEdge(float minRadius, float maxRadius)
        {
            var radius = Random.Range(minRadius, maxRadius);
            var vector2 = Random.insideUnitCircle.normalized * radius;
            return new Vector3(vector2.x, 0, vector2.y);
        }


        private IEnumerator SpawnEnemyPod(SpawnTypeData pod)
        {
            yield return new WaitForSeconds(pod.delay);
            Vector3 siloPosition = silo.transform.position;
            for (var i = 0; i < pod.numberOfSpaws; i++)
            {
                Vector3 spawnPoint = RandomPointOnCircleEdge(spawnRadius - spawnArea / 2, spawnRadius + spawnArea / 2);
                Debug.Log(spawnPoint);
                float angle = Vector3.Angle(spawnPoint, siloPosition);
                Instantiate(pod.enemyPrefab, spawnPoint, Quaternion.AngleAxis(angle, Vector3.up));
                gameState.numberOfEnemiesAlive++;
            }
        }
    
        private void SpawnAllEnemiesInWave(Wave wave)
        {
            foreach (var spawnPod in wave.waveData)
            {
                StartCoroutine(SpawnEnemyPod(spawnPod));
            }
        }
    
        [ContextMenu("Spawn Wave")]
        public void SpawnWave()
        {

            var waveToSpawn = enemyWaves.waves[currentWave];
            SpawnAllEnemiesInWave(waveToSpawn);
        
            currentWave++;
        }
    
    }
}
