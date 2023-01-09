using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private EnemyWaveHolder enemyWaves;
        [SerializeField] private GameState gameState;
        [SerializeField] private float spawnRadius;
        [SerializeField] private float spawnArea;
        [SerializeField] private GameObject silo;
    
        [ShowInInspector] private int currentWave = 0;
        [SerializeField] private float timeToNextWave = 5f;

        private Coroutine countdown = null;

        private int numberOfPodsToSpawnInCurrentWave;
        private int numberOfCurrentlySpawnedPods;
        
        private void Start()
        {
            gameState.numberOfEnemiesAlive = 0;
            gameState.timeToNextWave = 0;
            currentWave = 0;
            SpawnWave();
            GameEvents.OnEnemyKilled += CheckWaveStatus;
        }

        private void IncrementCurrentWave()
        {
            currentWave++;
        }

        private Vector3 RandomPointOnCircleEdge(float minRadius, float maxRadius)
        {
            var radius = Random.Range(minRadius, maxRadius);
            var vector2 = Random.insideUnitCircle.normalized * radius;
            return new Vector3(vector2.x, 0, vector2.y);
        }

        // private void Update()
        // {
        //     Debug.Log(podIsSpawned.Count);
        // }


        private IEnumerator SpawnEnemyPod(SpawnTypeData pod, int index)
        {
            yield return new WaitForSeconds(pod.delay);
            for (var i = 0; i < pod.numberOfSpaws; i++)
            {
                Vector3 spawnPoint = RandomPointOnCircleEdge(spawnRadius - spawnArea / 2, spawnRadius + spawnArea / 2);
                Instantiate(pod.enemyPrefab, spawnPoint, Quaternion.identity);
            }

            numberOfCurrentlySpawnedPods++;

        }
        
    
        public void SpawnWave()
        {
            numberOfPodsToSpawnInCurrentWave = enemyWaves.waves[currentWave].waveData.Length;
            numberOfCurrentlySpawnedPods = 0;

            var wave = enemyWaves.waves[currentWave];
            for (var i = 0; i < wave.waveData.Length; i++)
            {
                StartCoroutine(SpawnEnemyPod(wave.waveData[i], i));
                
            }
            
            gameState.waveIsActive = true;
            IncrementCurrentWave();
            GameEvents.OnWaveStart?.Invoke();
        }

        IEnumerator CountdownToNextWave(float time)
        {
            var countdownTime = time;
            while (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                gameState.timeToNextWave = countdownTime;
                yield return null;
            }
            
            SpawnWave();
            countdown = null;

        }


        public void CheckWaveStatus()
        {
            // Debug.Log($"Have all pods been spawned {podIsSpawned.Any(x => x != true)}");
            // Debug.Log($"Is the way not active {!gameState.waveIsActive}");
            // Debug.Log($"Is the number of enemies alive not equal zero {gameState.numberOfEnemiesAlive != 0}");

            
            if (gameState.numberOfEnemiesAlive != 0 || numberOfCurrentlySpawnedPods != numberOfPodsToSpawnInCurrentWave) return;
            
            gameState.waveIsActive = false;
            
            if (currentWave == enemyWaves.waves.Count)
            {
                Debug.Log("You win");
                GameEvents.OnGameWin?.Invoke();
            }
            else
            {
                gameState.timeToNextWave = timeToNextWave;
                GameEvents.OnWaveEnd?.Invoke();
                
                if (countdown != null)
                {
                    StopCoroutine(countdown);
                }
                countdown = StartCoroutine(CountdownToNextWave(timeToNextWave));
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, spawnRadius - spawnArea / 2);
            Gizmos.DrawWireSphere(transform.position, spawnRadius + spawnArea / 2);
        }
    }
}
