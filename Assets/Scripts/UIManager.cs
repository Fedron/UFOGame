using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager Instance = null;

    [SerializeField] GameObject gameUI = default;
    [SerializeField] GameObject gameOverUI = default;
    [SerializeField] GameObject newBestUI = default;
    [SerializeField] TextMeshProUGUI scoreText = default;
    [SerializeField] TextMeshProUGUI bestScoreText = default;

    private void Awake() {
        Instance = this;
    }

    public void ShowGameOverScreen() {
        scoreText.text = ScoreManager.Instance.GetScore().ToString("d4");

        int cur = ScoreManager.Instance.GetScore();
        int best = PlayerPrefs.GetInt("score", 0);
        if (cur > best) {
            PlayerPrefs.SetInt("score", cur);
            newBestUI.SetActive(true);
        }
        bestScoreText.SetText(string.Concat("Best: ", PlayerPrefs.GetInt("score", 0).ToString("d4")));

        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
    }
}
