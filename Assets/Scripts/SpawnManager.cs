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
    public int maxWalls = 10;
    private float repeat = 1.0f;
    public GameObject powerupPrefab;
    public GameObject defaultWallPrefab;
    public GameObject[] wallPrefabs;
    public GameObject SnakePrefab;
    public float[] yAngles = { 0, 180 };
    public float[] zAngles = { 0, 90, 270 };
    public float spawnProbablility = 0.25f;
    private int powerupCount;
    public int maxSpawnAttemptsPerObstacle = 20;
    // Start is called before the first frame update
    void Awake()
    {
        InitiateWalls();
        SpawnRandomWall();
    }

    void Start()
    {
        SpawnSnake();
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
            isValid = PreventSpawnOverLap(spawnPos, 3.0f);
        }
        if (isValid && powerupCount < maxPowerups)
        {
            Instantiate(powerupPrefab, spawnPos, Quaternion.identity);
        }
    }

    bool PreventSpawnOverLap(Vector3 spawnPos, float checkRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(spawnPos, checkRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Wall" || collider.tag == "Powerup" || collider.tag == "Snake")
            {
                return false;
            }
        }
        return true;
    }


void SpawnRandomWall()
    {
        for (int i = 0; i < maxWalls; i++) {
            Vector3 spawnPos = Vector3.zero;
            bool isValid = false;
            int spawnAttempts = 0;
            while (!isValid && spawnAttempts < maxSpawnAttemptsPerObstacle)
            {
                spawnAttempts++;
                float spawnPosX = Random.Range(-xBound + 5.0f, xBound - 5.0f);
                float spawnPosY = Random.Range(-yBound + 5.0f, yBound - 5.0f);
                spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
                isValid = PreventSpawnOverLap(spawnPos, 2.0f);
            }
            if (isValid)
            {
                int wallIndex = Random.Range(0, wallPrefabs.Length);
                int yAngleIndex = Random.Range(0, yAngles.Length);
                int zAngleIndex = Random.Range(0, zAngles.Length);
                Instantiate(wallPrefabs[wallIndex], spawnPos, Quaternion.Euler(0, yAngles[yAngleIndex], zAngles[zAngleIndex]));
            }
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

    void SpawnSnake()
    {
        Vector3 spawnPos = Vector3.zero;
        bool isValid = false;

        int spawnAttempts = 0;
        while (!isValid && spawnAttempts < maxSpawnAttemptsPerObstacle)
        {
            spawnAttempts++;
            float spawnPosX = Random.Range(-xBound +5.0f, xBound -5.0f);
            float spawnPosY = Random.Range(-yBound + 5.0f, yBound - 5.0f);
            spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
            isValid = PreventSpawnOverLap(spawnPos, 3.0f);
        }
        if (isValid)
        {
            Instantiate(SnakePrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            //Just in case set the default Location
            Instantiate(SnakePrefab, new Vector3(-15.0f, 15.0f), Quaternion.identity);
        }
    }
}