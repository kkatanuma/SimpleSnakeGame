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
    private uint powerupObtained = 0;

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
        if (powerupObtained % 3 == 0)
        {
            SpawnManager.Instance.SpawnEnemySnake();
        }
        if (powerupObtained % 5 ==0) {
            SpawnManager.Instance.SpawnAWall();
        }
    }
}
