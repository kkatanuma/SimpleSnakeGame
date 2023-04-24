using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    int score { get; set; } = 0;
    int highscore = 0;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);
        scoreText.text = score.ToString() + " POINTS";
        highscoreText.text = "HIGHSCORE:" + highscore.ToString();
    }


    public void AddPoint()
    {
        score += 1;
        scoreText.text = score.ToString() + " POINTS";
        if(highscore < score)
            PlayerPrefs.SetInt("highscore", score);
    }

    public int GetScore()
    {
        return score;
    }
}
