using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject hamsterPrefab;
    public float spawnInterval = 2f;

    private int enemiesToSpawn;
    private int enemiesAlive;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MapParser.Instance != null && MapParser.Instance.entryTile != null);

        enemiesToSpawn = MapParser.Instance.TotalEnemies;
        enemiesAlive = 0;

        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (enemiesToSpawn > 0)
        {
            SpawnHamster();
            enemiesToSpawn--;
            enemiesAlive++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnHamster()
    {
        var startTile = MapParser.Instance.entryTile;
        var hamster = Instantiate(hamsterPrefab, startTile.worldPosition, Quaternion.identity);

        var mover = hamster.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.OnDeath += HandleHamsterDeath;
            var path = new Pathfinding(MapParser.Instance.GetGrid(), MapParser.Instance.Width, MapParser.Instance.Height)
                .FindPath(MapParser.Instance.entryTile, MapParser.Instance.exitTile);
            mover.SetPath(path);
        }
    }

    private void HandleHamsterDeath()
    {
        enemiesAlive--;
        if (enemiesToSpawn == 0 && enemiesAlive == 0)
        {
            EndGameUI.Instance?.ShowVictory();
        }
    }
}