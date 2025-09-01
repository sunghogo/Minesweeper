using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum TileState
{
    Hidden = 0,
    Open = 1
}

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] SpriteRenderer flagSpriteRenderer;
    [SerializeField] SpriteRenderer mineSpriteRenderer;
    [SerializeField] TextMeshProUGUI countTMP;
    Material tileMaterial;

    [field: Header("States")]
    [field: SerializeField] public bool IsFlagged { get; private set; } = false;
    [field: SerializeField] public bool IsRevealed { get; private set; } = false;
    [field: SerializeField] public bool IsMine { get; private set; } = false;

    [field: Header("Properties")]
    [field: SerializeField] public int NeighboringMines { get; private set; } = 0;
    [field: SerializeField] public List<Tile> Neighbors { get; private set; } = new List<Tile>();

    [field: Header("Text Colors")]
    [field: SerializeField]
    public List<Color> TextColors = new List<Color> {
        new Color(255, 255, 255, 255),       // 0 = White
        new Color(0, 0, 255, 255),           // 1 = Blue
        new Color(0, 128, 0, 255),           // 2 = Green
        new Color(255, 0, 0, 255),           // 3 = Red
        new Color(0, 0, 128, 255),           // 4 = Dark Blue
        new Color(128, 0, 0, 255),           // 5 = Dark Red
        new Color(0, 128, 128, 255),         // 6 = Teal
        new Color(0, 0, 0, 255),             // 7 = Black
        new Color(128, 128, 128, 255),       // 8 = Gray
    };

    [field: Header("Shader Properties")]
    [field: SerializeField] private static readonly int StateID = Shader.PropertyToID("_State");

    public void SetMine()
    {
        IsMine = true;
    }

    public void IncrementNeighboringMines()
    {
        ++NeighboringMines;
        UpdateCountTMP();
    }

    public void IncremenetNeighbors()
    {
        foreach (var tile in Neighbors)
        {
            tile.IncrementNeighboringMines();
        }
    }

    public void AddNeighbor(Tile tile)
    {
        Neighbors.Add(tile);
    }

    public void Reveal()
    {
        if (IsRevealed) return;

        if (IsFlagged && !GameManager.Instance.GameWin) DeactivateFlag();
        IsRevealed = true;
        SetTileState(TileState.Open);
        if (IsMine)
        {
            if (!GameManager.Instance.GameWin) ActivateMine();
            else SetTileState(TileState.Hidden);
            
            if (GameManager.Instance.GameStart) GameManager.Instance.EndGame();
        }
        else ActivateTMP();
    }

    public void RevealTiles(Tile tile)
    {
        Queue<Tile> queue = new Queue<Tile>();
        queue.Enqueue(tile);

        while (queue.Count > 0)
        {
            Tile t = queue.Dequeue();
            if (t.IsRevealed) continue;

            t.Reveal();

            if (t.NeighboringMines == 0)
            {
                foreach (Tile n in t.Neighbors)
                {
                    if (!n.IsRevealed && !n.IsMine)
                        queue.Enqueue(n);
                }
            }
        }
    }

    void OnMouseOver()
    {
        if (!GameManager.Instance.GameStart) return;

        if (Input.GetMouseButtonDown(0))
        {
            RevealTiles(this);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ToggleFlag();
        }
    }

    void Awake()
    {
        tileMaterial = GetComponent<SpriteRenderer>().material;
        SetTileState(TileState.Hidden);
        DeactivateFlag();
        DeactivateMine();
        DeactivateTMP();
    }

    void DeactivateFlag()
    {
        IsFlagged = false;
        flagSpriteRenderer.enabled = false;
        GameManager.Instance.IncrementRemainingMines();
        if (IsMine) GameManager.Instance.IncrementActualRemainingMines();
    }

    void ActivateFlag()
    {
        IsFlagged = true;
        flagSpriteRenderer.enabled = true;
        GameManager.Instance.DecrementRemainingMines();
        if (IsMine)
        {
            GameManager.Instance.DecrementActualRemainingMines();
            if (GameManager.Instance.ActualRemainingMines == 0) GameManager.Instance.WinGame();
        }
    }

    void ToggleFlag()
    {
        if (IsRevealed) return;

        if (IsFlagged) DeactivateFlag();
        else if (GameManager.Instance.RemainingMines > 0)ActivateFlag();
    }

    void DeactivateMine()
    {
        mineSpriteRenderer.enabled = false;

    }

    void ActivateMine()
    {
        mineSpriteRenderer.enabled = true;
    }

    void DeactivateTMP()
    {
        countTMP.enabled = false;

    }

    void ActivateTMP()
    {
        countTMP.enabled = true;
    }

    void UpdateCountTMP()
    {
        countTMP.color = TextColors[NeighboringMines];
        if (NeighboringMines == 0) countTMP.text = "";
        else countTMP.text = NeighboringMines.ToString();
    }

    void SetTileState(TileState state)
    {
        tileMaterial.SetFloat(StateID, (float)state);
    }
}
