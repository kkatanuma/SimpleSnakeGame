using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameOverScreen GameOverScreen;
    private uint powerupObtained = 0;
    private bool isGameActive;
    public TextMeshProUGUI countdownText;
    private IEnumerator coroutine;
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isGameActive)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Display GameOver Background
    /// </summary>
    public void GameOver()
    {
        isGameActive = false;
        GameOverScreen.gameObject.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        countdownText.gameObject.SetActive(true);
        coroutine = Countdown(3);
        StartCoroutine(coroutine);
        startButton.interactable = false;
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("SnakeGame");
        startButton.interactable = true;
    }

    IEnumerator Countdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            
            yield return new WaitForSecondsRealtime(1);
            counter--;
            countdownText.text = counter.ToString();
        }
        countdownText.gameObject.SetActive(false);
        isGameActive = true;
    }




/// <summary>
/// Add Point whenever powerups are collected
/// Spawn EnemySnake every 3 powerups
/// Spawn a wall every 5 poerups
/// </summary>
public void AddPoint()
    {
        powerupObtained++;
        if (powerupObtained % 3 == 0)
        {
            SpawnManager.Instance.SpawnEnemySnake();
        }
        if (powerupObtained % 5 == 0)
        {
            SpawnManager.Instance.SpawnAWall();
        }
    }
}
