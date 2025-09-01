using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestartButton : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Button button;
    [SerializeField] TMP_Text tmp;
    [SerializeField] Image panel;

    void Awake()
    {
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnGameWin += HandleGameWin;

        button.onClick.AddListener(() => GameManager.Instance.StartGame());
    }

    void OnDestroy()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnGameWin += HandleGameWin;
        if (button) button.onClick.RemoveAllListeners();
    }

    void HandleGameStart()
    {
        tmp.text = "Restart";
    }

    void HandleGameWin()
    {
        tmp.text = "Start";
    }
}
