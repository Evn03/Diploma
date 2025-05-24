using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject hamsterPrefab;
    public float spawnInterval = 2f;
    private int enemiesToSpawn;
    private int enemiesAlive;

private void Start()
{
    enemiesToSpawn = MapParser.Instance.TotalEnemies;
    Debug.Log("Всего хомяков для спавна: " + enemiesToSpawn);

    StartCoroutine(SpawnRoutine());
}

IEnumerator SpawnRoutine()
{
    while (enemiesToSpawn > 0)
    {
        Debug.Log("Спавним хомяка...");
        SpawnHamster();
        enemiesToSpawn--;
        enemiesAlive++;
        yield return new WaitForSeconds(spawnInterval);
    }
}

void SpawnHamster()
{
    TileData startTile = MapParser.Instance.entryTile;
    if (startTile == null)
    {
        Debug.LogError("entryTile не найден! Проверь карту.");
        return;
    }

    GameObject hamster = Instantiate(hamsterPrefab, startTile.worldPosition, Quaternion.identity);
    Debug.Log("Хомяк заспавнен по координатам: " + startTile.worldPosition);

    EnemyMover mover = hamster.GetComponent<EnemyMover>();
    if (mover != null)
    {
        mover.OnDeath += HandleHamsterDeath;
        List<TileData> path = new Pathfinding(MapParser.Instance.GetGrid(), MapParser.Instance.Width, MapParser.Instance.Height)
            .FindPath(MapParser.Instance.entryTile, MapParser.Instance.exitTile);

        mover.SetPath(path);
    }
}

    void HandleHamsterDeath()
    {
        enemiesAlive--;
        if (enemiesAlive == 0 && enemiesToSpawn == 0)
        {
            Debug.Log("Победа! Все хомяки повержены!");
        }
    }
}