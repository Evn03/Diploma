using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public Tower selectedTowerPrefab;
    public static TowerPlacer Instance;
    private bool isPlacingTower = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void SelectTower(Tower towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;
        isPlacingTower = true;

        HighlightBuildableTiles();
    }

    private void HighlightBuildableTiles()
    {
   
        for (int x = 0; x < MapParser.Instance.Width; x++)
        {
            for (int y = 0; y < MapParser.Instance.Height; y++)
            {
                TileData tile = MapParser.Instance.GetTileAt(x, y);
                if (tile != null)
                {
                    TileBehaviour tb = tile.tileGO.GetComponent<TileBehaviour>();
                    if (tb != null)
                    {
                        if (tile.isBuildable && !tile.HasTower)
                            tb.Highlight(Color.green); // допустимые
                        else
                            tb.Highlight(Color.red);   // недопустимые
                    }
                }
            }
        }
    }

    private void ResetAllHighlights()
    {
        for (int x = 0; x < MapParser.Instance.Width; x++)
        {
            for (int y = 0; y < MapParser.Instance.Height; y++)
            {
                TileData tile = MapParser.Instance.GetTileAt(x, y);
                if (tile != null)
                {
                    TileBehaviour tb = tile.tileGO.GetComponent<TileBehaviour>();
                    if (tb != null)
                        tb.ResetHighlight();
                }
            }
        }
    }

    public void CancelPlacement()
    {
        selectedTowerPrefab = null;
        isPlacingTower = false;
        ResetAllHighlights();
    }

    public void TryPlaceTower(int x, int y)
    {
        
        TileData tile = MapParser.Instance.GetTileAt(x, y);
        Debug.Log($"Устанавливаем башню на тайл {x},{y}, позиция: {tile.worldPosition}");
        if (!isPlacingTower || selectedTowerPrefab == null)
        {
            Debug.Log("Режим установки неактивен или башня не выбрана");
            return;
        }

        if (tile == null || !tile.isBuildable || tile.HasTower)
        {
            Debug.Log("Нельзя построить башню на этой клетке.");
            return;
        }

        if (!UIManager.Instance.SpendCurrency(selectedTowerPrefab.cost))
        {
            Debug.Log("Недостаточно валюты.");
            isPlacingTower = false;
            ResetAllHighlights();
            return;
        }

        
        Vector3 position = tile.worldPosition;

        float spriteHeight = selectedTowerPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        position.y += spriteHeight / 4f * -0.8f;

        GameObject towerGO = Instantiate(selectedTowerPrefab.gameObject, position, Quaternion.identity);
        SpriteRenderer sr = towerGO.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = y * 10 + 5;
        }
        Tower towerComponent = towerGO.GetComponent<Tower>();
        if (towerComponent != null)
        {
            tile.tower = towerComponent;
        }
        selectedTowerPrefab = null;
        isPlacingTower = false;
        EnemyManager.Instance.NotifyPathChanged();
        ResetAllHighlights();
    }
}
