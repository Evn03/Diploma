using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapParser : MonoBehaviour
{
    public GameObject grassPrefab;  // TX Tileset Grass 3
    public GameObject roadPrefab;   // TX Tileset Grass Pavement 2
    public GameObject wallPrefab;   // TX Tileset Stone Ground Pavement 2
    public GameObject entryPrefab;  // TX Props Wooden Gate Opened (I)
    public GameObject exitPrefab;   // TX Props Wooden Gate (O)

    public TextAsset jsonMap; // JSON файл с картой

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Парсинг JSON файла
        MapData mapData = JsonUtility.FromJson<MapData>(jsonMap.text);
        string[] map = mapData.map;

        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                char tile = map[y][x];
                GameObject prefab = null;

                // Определяем, какой префаб использовать в зависимости от символа
                switch (tile)
                {
                    case 'G':
                        prefab = grassPrefab;
                        break;
                    case 'R':
                        prefab = roadPrefab;
                        break;
                    case 'W':
                        prefab = wallPrefab;
                        break;
                    case 'I':
                        prefab = entryPrefab;
                        break;
                    case 'O':
                        prefab = exitPrefab;
                        break;
                }

                // Создаём тайл
                if (prefab != null)
                {
                    // Позиционирование тайлов в 2D
                    Vector3 position = new Vector3(x, -y, 0); // Инвертируем y, чтобы карта строилась сверху вниз
                    Instantiate(prefab, position, Quaternion.identity);
                }
            }
        }
    }
}

[System.Serializable]
public class MapData
{
    public string[] map; // Массив строк, представляющий карту
}