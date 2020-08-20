using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager Instance = null;

    [SerializeField] GameObject gameUI = default;
    [SerializeField] GameObject gameOverUI = default;
    [SerializeField] TextMeshProUGUI scoreText = default;

    private void Awake() {
        Instance = this;
    }

    public void ShowGameOverScreen() {
        scoreText.text = ScoreManager.Instance.GetScore().ToString("d4");
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
    }
}
