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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
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
