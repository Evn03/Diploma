using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public string[] map;
}

public class MapParser : MonoBehaviour
{
    public static MapParser Instance;

    [Header("Tile Prefabs")]
    public GameObject grassPrefab;
    public GameObject roadPrefab;
    public GameObject wallPrefab;
    public GameObject entryPrefab;
    public GameObject exitPrefab;

    [Header("Map Settings")]
    public TextAsset jsonMap;
    public float scale = 1f;

    private TileData[,] grid;
    private int width, height;
    private Vector3 mapOffset;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GenerateMap();
    }

    public TileData GetTileAt(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return grid[x, y];
        return null;
    }

    private void GenerateMap()
    {
        MapData mapData = JsonUtility.FromJson<MapData>(jsonMap.text);
        string[] map = mapData.map;

        height = map.Length;
        width = map[0].Length;
        grid = new TileData[width, height];

        mapOffset = new Vector3(-width / 2f * scale + scale / 2f, height / 2f * scale - scale / 2f, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                char tile = map[y][x];
                GameObject prefab = null;

                switch (tile)
                {
                    case 'G': prefab = grassPrefab; break;
                    case 'R': prefab = roadPrefab; break;
                    case 'W': prefab = wallPrefab; break;
                    case 'I': prefab = entryPrefab; break;
                    case 'O': prefab = exitPrefab; break;
                }

                if (prefab != null)
                {
                    Vector3 position = new Vector3(x * scale, -y * scale, 0) + mapOffset;
                    GameObject tileGO = Instantiate(prefab, position, Quaternion.identity, transform);

                    tileGO.transform.localScale = Vector3.one * scale;

                    bool isWalkable = tile == 'R' || tile == 'I' || tile == 'O';
                    bool isBuildable = tile == 'G';

                    TileData tileData = new TileData(x, y, isBuildable, isWalkable, tileGO);
                    grid[x, y] = tileData;

                    TileBehaviour behaviour = tileGO.GetComponent<TileBehaviour>();
                    if (behaviour != null)
                        behaviour.Init(x, y);
                }
            }
        }
        
        Vector3 mapCenter = new Vector3((width - 1) * scale / 2f, -(height - 1) * scale / 2f, -10f) + mapOffset;
        Camera.main.transform.position = mapCenter;

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = (float)width / height;
        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = height * scale / 2f;
        }
        else
        {
            float differenceInSize = (width / screenRatio) * scale / 2f;
            Camera.main.orthographicSize = differenceInSize;
        }
    }
}
