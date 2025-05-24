using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public Tower selectedTowerPrefab;
    public static TowerPlacer Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TryPlaceTower(int x, int y)
    {
        TileData tile = MapParser.Instance.GetTileAt(x, y);

        if (tile == null || !tile.isBuildable || tile.HasTower)
        {
            Debug.Log("Нельзя построить башню на этой клетке.");
            return;
        }

        if (!UIManager.Instance.SpendCurrency(selectedTowerPrefab.cost))
        {
            Debug.Log("Недостаточно валюты.");
            return;
        }

        Vector3 position = tile.worldPosition;
        GameObject towerGO = Instantiate(selectedTowerPrefab.gameObject, position, Quaternion.identity);
        Tower towerComponent = towerGO.GetComponent<Tower>();
        if (towerComponent != null)
        {
            tile.tower = towerComponent;
        }
    }
}
