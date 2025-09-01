using UnityEngine;
using System;

public enum GameState
{
    StartingScreen,
    GameStart,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnHighScoreChanged;
    public static event Action<int> OnRemainingMinesChanged;

    public static event Action OnNextLevel;
    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action OnScreenStart;
    public static event Action OnGameWin;



    [field: Header("Game States")]
    [field: SerializeField] public bool StartingScreen { get; private set; }
    [field: SerializeField] public bool GameStart { get; private set; }
    [field: SerializeField] public bool GameOver { get; private set; }
    [field: SerializeField] public bool ChangeLevel { get; private set; }
    [field: SerializeField] public bool GameWin { get; private set; }

    [field: Header("Shared Data")]
    [field: SerializeField] public int Score { get; private set; } = 0;
    [field: SerializeField] public int HighScore { get; private set; } = 0;
    [field: SerializeField] public int RemainingMines { get; private set; } = 0;
    [field: SerializeField] public int ActualRemainingMines { get; private set; } = 0;


    private float timer = 0f;

    public void SetRemainingMines(int mines)
    {
        ActualRemainingMines = RemainingMines = mines;
        OnRemainingMinesChanged?.Invoke(mines);
    }

    public void IncrementRemainingMines()
    {
        ++RemainingMines;
        OnRemainingMinesChanged?.Invoke(RemainingMines);
    }

    public void DecrementRemainingMines()
    {
        --RemainingMines;
        OnRemainingMinesChanged?.Invoke(RemainingMines);
    }

    public void IncrementActualRemainingMines()
    {
        ++ActualRemainingMines;
    }

    public void DecrementActualRemainingMines()
    {
        --ActualRemainingMines;
    }

    public void IncrementScore()
    {
        ++Score;
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }

    void UpdateHighScore()
    {
        HighScore = Score;
        OnHighScoreChanged?.Invoke(HighScore);
    }

    public void StartGame()
    {
        StartingScreen = false;
        GameStart = true;
        GameOver = false;
        ChangeLevel = false;
        GameWin = false;
        ResetScore();
        timer = 0f;
        OnGameStart?.Invoke();
        OnScoreChanged?.Invoke(Score);
        OnHighScoreChanged?.Invoke(HighScore);
    }

    public void EndGame()
    {
        StartingScreen = false;
        GameStart = false;
        GameOver = true;
        ChangeLevel = false;
        GameWin = false;
        OnGameOver?.Invoke();
    }

    public void StartScreen()
    {
        StartingScreen = true;
        GameStart = false;
        GameOver = false;
        ChangeLevel = false;
        GameWin = false;
        OnScreenStart?.Invoke();
    }

    public void NextLevel()
    {
        ChangeLevel = true;
        OnNextLevel?.Invoke();
    }

    public void WinGame()
    {
        StartingScreen = false;
        GameStart = false;
        GameOver = false;
        ChangeLevel = false;
        GameWin = true;
        if (HighScore == 0 || Score < HighScore)
        {
            UpdateHighScore();
        }
        OnGameWin?.Invoke();
    }

    void ProcessTime()
    {
        timer += Time.fixedDeltaTime;
        if (timer > 1f)
        {
            --timer; ;
            IncrementScore();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        if (GameStart) ProcessTime();
    }

    void Start()
    {
        Instance.StartScreen();   
    }
}
