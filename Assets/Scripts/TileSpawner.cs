using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public static TileSpawner Instance { get; private set; }

    [Header("Refs")]
    public GameObject tilePrefab;

    [Header("Grid Settings")]
    [SerializeField] int rows = 8;
    [SerializeField] int columns = 8;
    [SerializeField] float spacing = 0.1f;
    [SerializeField] float tileSize = 0.5f;
    [SerializeField] int minesSpawned = 40;

    [field: Header("Properties")]
    [field: SerializeField] public List<List<Tile>> Tiles { get; private set; } = new List<List<Tile>>();
    [field: SerializeField] public int TotalTiles { get; private set; } = 0;
    [field: SerializeField] public int TotalMines { get; private set; } = 0;

    public void SpawnGrid()
    {
        if (tilePrefab == null) return;
        ClearGrid();

        float totalWidth = columns * tileSize + (columns - 1) * spacing;
        float totalHeight = rows * tileSize + (rows - 1) * spacing;

        Vector3 origin = transform.position;
        origin -= new Vector3(totalWidth / 2f - tileSize / 2f, totalHeight / 2f - tileSize / 2f, 0);

        for (int row = 0; row < rows; row++)
        {
            List<Tile> rowList = new List<Tile>();
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = origin + new Vector3(column * (tileSize + spacing), row * (tileSize + spacing), 0);
                Tile tile = Instantiate(tilePrefab, position, Quaternion.identity, transform).GetComponent<Tile>();
                tile.name = $"Tile_{row}_{column}";
                rowList.Add(tile);
            }
            Tiles.Add(rowList);
        }

        TotalMines = minesSpawned;
        GameManager.Instance.SetRemainingMines(TotalMines);
        TotalTiles = rows * columns;
    }

    public void GenerateMines()
    {
        int minesRemaining = TotalMines;

        for (int i = 0; i < TotalTiles; ++i)
        {
            int tilesRemaining = TotalTiles - i;

            // P(choose i) = k / i, where i = k +1, ..., N
            // P(Mine at Tile i) = Mines Remaining / Tiles Remaining
            if (Random.value < (float)minesRemaining / tilesRemaining)
            {
                int r = i / columns;
                int c = i % columns;

                Tile tile = GetTile(r, c);
                tile.SetMine();
                tile.IncremenetNeighbors();

                --minesRemaining;
                if (minesRemaining == 0) break;
            }
        }
    }

    public void InitializeNeighbors()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Tile tile = GetTile(row, column).GetComponent<Tile>();
                tile.Neighbors.Clear();

                for (int rOffset = -1; rOffset <= 1; rOffset++)
                {
                    for (int cOffset = -1; cOffset <= 1; cOffset++)
                    {
                        if (rOffset == 0 && cOffset == 0)
                            continue;

                        int r = row + rOffset;
                        int c = column + cOffset;

                        if (r >= 0 && r < rows && c >= 0 && c < columns)
                        {
                            Tile neighbor = GetTile(r, c).GetComponent<Tile>();
                            tile.AddNeighbor(neighbor);
                        }
                    }
                }
            }
        }
    }

    public void RevealGrid()
    {
        foreach (var row in Tiles)
        {
            foreach (var tile in row)
            {
                tile.Reveal();
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Tiles.Clear();
        TotalMines = 0;
        GameManager.Instance.SetRemainingMines(TotalMines);
        TotalTiles = 0;
    }

    Tile GetTile(int row, int col)
    {
        if (row >= 0 && row < Tiles.Count && col >= 0 && col < Tiles[row].Count)
            return Tiles[row][col];
        return null;
    }

    void InitializeGrid()
    {
        SpawnGrid();
        InitializeNeighbors();
        GenerateMines();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameWin += HandleGameWin;
    }

    void OnDestroy()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameWin -= HandleGameWin;
    }

    void HandleGameStart()
    {
        ClearGrid();
        InitializeGrid();
    }

    void HandleGameOver()
    {
        RevealGrid();
    }

    void HandleGameWin()
    {
        RevealGrid();
    }
}
