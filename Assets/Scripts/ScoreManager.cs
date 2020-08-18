using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance = null;
    [SerializeField] TextMeshProUGUI scoreText = default;
    [SerializeField] GameObject scorePopup = default;
    private int score = 0;

    private void Awake() {
        Instance = this;
        scoreText.SetText("0000");
    }

    public void AddScore(int add) {
        score += add;
        scoreText.SetText(score.ToString("d4"));

        GameObject popup  = Instantiate(scorePopup, FindObjectOfType<Canvas>().transform);
        popup.GetComponent<TextMeshProUGUI>().SetText($"+{add.ToString("d3")}");
        Destroy(popup, 1f);
    }
}
