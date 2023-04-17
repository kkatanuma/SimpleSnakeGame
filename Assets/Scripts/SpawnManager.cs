using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float xBound = 19.5f;
    private float yBound = 19.5f;
    public float startDelay = 2.0f;
    public float maxPowerups = 20;
    private float repeat = 1.0f;
    public GameObject powerupPrefab;
    public GameObject defaultWallPrefab;
    public GameObject[] wallPrefabs;
    public float[] yAngles = { 0, 180 };
    public float[] zAngles = { 0, 90, 270 };
    public float spawnProbablility = 0.25f;
    private int powerupCount;
    public float obstacleCheckRaduis = 5f;
    public int maxSpawnAttemptsPerObstacle = 10;
    // Start is called before the first frame update
    void Start()
    {
        InitiateWalls();
        SpawnRandomWall();
        InvokeRepeating("SpawnPowerups", startDelay, repeat);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("Powerup");
        powerupCount = powerups.Length;
        
    }

    void SpawnPowerups()
    {
        Vector3 spawnPos = Vector3.zero;
        bool isValid = false;

        int spawnAttempts = 0;

        while (!isValid && spawnAttempts < maxSpawnAttemptsPerObstacle)
        {
            spawnAttempts++;
            float spawnPosX = Random.Range(-xBound, xBound);
            float spawnPosY = Random.Range(-yBound, yBound);
            spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
            isValid = true;
            Collider[] colliders = Physics.OverlapSphere(spawnPos, obstacleCheckRaduis);

            foreach (Collider collider in colliders)
            {
                if (collider.tag == "Wall" || collider.tag == "Powerup")
                {
                    isValid = false;
                }
            }
        }
        if (isValid && powerupCount < maxPowerups)
        {
            Instantiate(powerupPrefab, spawnPos, Quaternion.identity);
        }
    }

/*    bool PreventSpawnOverLap(Vector3 spawnPos)
    {
        Collider[] colliders = Physics.OverlapSphere(spawnPos, obstacleCheckRaduis);

        foreach(Collider collider in colliders)
        {
            if(collider.tag == "Wall" || collider.tag == "Powerup")
            {
                isValid = false;
            }
        }
        }return true;
    }*/

    
    void SpawnRandomWall()
    {
        for (int i = 0; i < 10; i++)
        {
            int wallIndex = Random.Range(0, wallPrefabs.Length);
            int yAngleIndex = Random.Range(0, yAngles.Length); 
            int zAngleIndex = Random.Range(0, zAngles.Length);
            
            Vector3 spawnPos = new Vector3(Random.Range(-xBound + 5, xBound-5), Random.Range(-yBound +5 , yBound -5 ), 0);
            Instantiate(wallPrefabs[wallIndex], spawnPos, Quaternion.Euler(0, yAngles[yAngleIndex], zAngles[zAngleIndex]));
        }
    }

    void InitiateWalls()
    {
        //Building side walls
        for(int i = 0; i < 10; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                    Instantiate(defaultWallPrefab, new Vector3(xBound, yBound - (4 * i), 0), Quaternion.identity);
                    Instantiate(defaultWallPrefab, new Vector3(-xBound, yBound - (4 * i), 0), Quaternion.identity);
            }
        }
        //Building Top and Bottom Walls
        for(int i = 0; i < 10; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                Instantiate(defaultWallPrefab, new Vector3(-xBound + (4 * i), yBound, 0), Quaternion.Euler(0, 0, 90));
                Instantiate(defaultWallPrefab, new Vector3(-xBound + (4 * i), -yBound, 0), Quaternion.Euler(0, 0, 90));
            }
        }
    }
}