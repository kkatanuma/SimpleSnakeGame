using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameOverScreen GameOverScreen;
    private uint powerupObtained = 0;
    private bool isGameActive;

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
        GameOverScreen.Setup();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        StartCoroutine(Countdown(3));
    }

    IEnumerator Countdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            Debug.Log(counter);
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1));
            counter--;
        }
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
