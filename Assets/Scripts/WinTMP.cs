using UnityEngine;
using TMPro;

public class WinTMP : MonoBehaviour
{
    TMP_Text tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        GameManager.OnGameWin += HandleGameWin;
        GameManager.OnGameStart += HandleGameStart;
        HandleGameStart();
    }

    void OnDestroy()
    {
        GameManager.OnGameWin += HandleGameWin;
        GameManager.OnGameStart -= HandleGameStart;
    }

    void HandleGameStart()
    {
        tmp.enabled = false;
    }

    void HandleGameWin()
    { 
        tmp.enabled = true;
    }
}
