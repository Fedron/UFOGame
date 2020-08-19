using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour {
    [Header("World Setup")]
    [SerializeField] int width = default;
    [SerializeField] int height = default;
    [SerializeField, Range(0, 100)] int randomFillPercent = default;
    [SerializeField, Min(0)] int maxBuildings = default;

    [Header("Tilemap & Tiles")]
    [SerializeField] Tilemap waterTilemap = default;
    [SerializeField] TileBase waterTile = default;
    [SerializeField] Tilemap foliageTilemap = default;
    [SerializeField] TileBase[] foliageTiles = default;
    [SerializeField] GameObject[] buildings = default;

    string seed;
    int[,] map;
    List<Vector2> emptyTiles = new List<Vector2>();

    private void Start() {
        GenerateMap();
        SpawnBuildings();
        Invoke("SpawnTiles", 0.05f);
        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(
            Random.Range(5, width - 5), Random.Range(8, height - 5), 0f
        );
        Invoke("RecalculateNavmesh", 0.2f);
    }

    private void RecalculateNavmesh() {
        AstarPath.active.Scan(AstarPath.active.data.gridGraph);
    }

    private void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++) {
            SmoothMap();
        }

    	emptyTiles = new List<Vector2>();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (map[x, y] == 0) emptyTiles.Add(new Vector2(x, y));
            }
        }
    }

    private void RandomFillMap() {
        seed = System.DateTime.Now.Ticks.ToString();
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1;
                }
                else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    private void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    private int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    if (neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY];
                    }
                } else {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private void SpawnTiles() {
        foliageTilemap.ClearAllTiles();
        for (int x = -40; x < width + 40; x++) {
            for (int y = -40; y < height + 40; y++) {
                if (x < 0 || x >= width || y < 0 || y >= height) {
                    waterTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
                    continue;
                }

                if (map[x, y] == 1) {
                    waterTilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
                    continue;
                }

                // Check a building isn't spawned at these coords
                if (Physics2D.OverlapBox(new Vector2(x, y), Vector2.one, 0f)) continue;

                int tileIndex = Random.Range(0, foliageTiles.Length + 12);
                if (tileIndex < foliageTiles.Length)
                    foliageTilemap.SetTile(new Vector3Int(x, y, 0), foliageTiles[tileIndex]);
            }
        }
        waterTilemap.gameObject.AddComponent<TilemapCollider2D>();

        // Spawns all the edge colliders
        Vector3[] edges = new Vector3[4];
        edges[0] = new Vector3(width / 2, height + 5f, 0f);
        edges[1] = new Vector3(width + 5f, height / 2, 0f);
        edges[2] = new Vector3(width / 2, -5f, 0f);
        edges[3] = new Vector3(-5f, height / 2, 0f);
        foreach (Vector3 pos in edges) {
            GameObject edge = new GameObject("Edge");
            edge.transform.position = pos;
            edge.transform.SetParent(transform);
            BoxCollider2D col = edge.AddComponent<BoxCollider2D>();
            col.size = new Vector2(width + 10f, 1f);

            if (pos.x == width + 5f || pos.x == -5f) {
                edge.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }
    }

    private void SpawnBuildings() {
        for (int i = 0; i < maxBuildings; i++) {
            GameObject building = Instantiate(buildings[Random.Range(0, buildings.Length)], new Vector3Int(Random.Range(0, width), Random.Range(0, height), 0), Quaternion.identity); ;
        }
    }

    public Vector2 GetRandomPosition() {
		return emptyTiles[Random.Range(0, emptyTiles.Count)];
    }
}
