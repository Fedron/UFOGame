using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour {
    [System.Serializable]
    private class Wave {
        public float startTime = default;
        public float spawnInterval = default;
        public int maxSpawnedEnemies = default;
        public GameObject[] enemies = default;
    }

    [SerializeField] WorldGenerator world = default;
    [SerializeField] float startDelay = 1f;
    [SerializeField] GameObject[] animals = default;
    [SerializeField] int maxAnimals = 10;
    [SerializeField] Wave[] waves = default;

    [HideInInspector] public int spawnedAnimals = 0;
    [HideInInspector] public int spawnedEnemies = 0;
    private int currentWave = 0;
    private float startTime = 0f;

    private void Awake() {
        startTime = Time.time;
    }

    private void Start() {
        StartCoroutine("SpawnAnimals");
        StartCoroutine("SpawnEnemies");
    }

    private IEnumerator SpawnEnemies() {
        yield return new WaitForSeconds(startDelay);
        while (true) {
            if (spawnedEnemies < waves[currentWave].maxSpawnedEnemies) {
                if (currentWave + 1 < waves.Length && Time.time - startTime > waves[currentWave + 1].startTime) currentWave++;

                GameObject[] enemies = waves[currentWave].enemies;
                Instantiate(enemies[Random.Range(0, enemies.Length)], world.GetRandomPosition(), Quaternion.identity);
                spawnedEnemies++;
                yield return new WaitForSeconds(waves[currentWave].spawnInterval);
            } else {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator SpawnAnimals() {
        while (true) {
            if (spawnedAnimals < maxAnimals) {
                Instantiate(animals[Random.Range(0, animals.Length)], world.GetRandomPosition(), Quaternion.identity);
                spawnedAnimals++;
                yield return new WaitForSeconds(Random.Range(0, 3f));
            } else {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
