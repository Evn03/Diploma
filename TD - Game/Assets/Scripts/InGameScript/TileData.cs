using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public int x;
    public int y;
    public bool isBuildable;
    public bool isWalkable;
    public GameObject tileGO;

    private Tower towerOnTile;

    public TileData(int x, int y, bool isBuildable, bool isWalkable, GameObject tileGO)
    {
        this.x = x;
        this.y = y;
        this.isBuildable = isBuildable;
        this.isWalkable = isWalkable;
        this.tileGO = tileGO;
    }

    public bool HasTower => towerOnTile != null;
    public Tower tower
    {
        get => towerOnTile;
        set => towerOnTile = value;
    }

    public Vector3 worldPosition => tileGO != null ? tileGO.transform.position : Vector3.zero;
}
