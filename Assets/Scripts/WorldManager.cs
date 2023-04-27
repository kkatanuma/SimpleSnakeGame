using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance { get; private set; }
    public GameOverScreen GameOverScreen;
    private int powerupObtained = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

    }

    private void Update()
    {
    }

    public void GameOver()
    {
        GameOverScreen.Setup();
    }

    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Application closed");
    }


    public void AddPoint()
    {
        powerupObtained++;
        if(powerupObtained == 5) {
            SpawnManager.Instance.SpawnAWall();
        }
        if(powerupObtained == 3)
        {
            powerupObtained = 0;
            SpawnManager.Instance.SpawnEnemySnake();
        }
    }
}
