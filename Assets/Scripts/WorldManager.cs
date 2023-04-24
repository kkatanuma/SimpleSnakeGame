using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance { get; private set; }
    private Pathfinding pathfinding;
    public GameOverScreen GameOverScreen;
    private int powerupObtained = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        pathfinding = new Pathfinding(40, 40);

    }

    private void Update()
    {
        checkWallPositions();
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

    private void checkWallPositions()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            PathNode temp = pathfinding.GetNode(wall.transform.position);
            temp.isWalkable = false;
        }
    }

    public void AddPoint()
    {
        powerupObtained++;
        if(powerupObtained >= 5) {
            powerupObtained = 0;
            SpawnManager.instance.SpawnAWall();
        }
    }
}
