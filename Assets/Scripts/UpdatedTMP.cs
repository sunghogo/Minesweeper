using UnityEngine;
using TMPro;

public class UpdatedTMP : MonoBehaviour
{
    TMP_Text tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (gameObject.CompareTag("Score") || gameObject.CompareTag("Time"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.Score.ToString();
            GameManager.OnScoreChanged += UpdateText;
        }
        else if (gameObject.CompareTag("High Score") || gameObject.CompareTag("Best"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.HighScore.ToString();
            GameManager.OnHighScoreChanged += UpdateText;
        }
        else if (gameObject.CompareTag("Mines"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.RemainingMines.ToString();
            GameManager.OnRemainingMinesChanged += UpdateText;
        }

        HandleGameOver();

        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameStart += HandleGameStart;
    }

    void OnDestroy()
    {
        if (gameObject.CompareTag("Score") || gameObject.CompareTag("Time")) GameManager.OnScoreChanged -= UpdateText;
        else if (gameObject.CompareTag("High Score") || gameObject.CompareTag("Best")) GameManager.OnHighScoreChanged -= UpdateText;
        else if (gameObject.CompareTag("Mines")) GameManager.OnRemainingMinesChanged -= UpdateText;

        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameStart -= HandleGameStart;
    }

    void UpdateText(int number)
    {
        tmp.text = $"{gameObject.tag}: {number}";
    }

    void HandleGameStart()
    {
        tmp.enabled = true;
    }

    void HandleGameOver()
    { 
        tmp.enabled = false;
    }
}
