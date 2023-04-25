using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance { get; private set; }
    private float minBound = 0.5f;
    private float maxBound = 39.5f;
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
    public bool gameReady = false;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        /*InitiateWalls();
        SpawnRandomWall();*/
    }

    public void Start()
    {
        //SpawnSnake();
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
            float spawnPosX = Random.Range(minBound, maxBound);
            float spawnPosY = Random.Range(minBound, maxBound);
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


public void SpawnRandomWall()
    {
        for (int i = 0; i < maxWalls; i++) {
            Vector3 spawnPos = Vector3.zero;
            bool isValid = false;
            int spawnAttempts = 0;
            while (!isValid && spawnAttempts < maxSpawnAttemptsPerObstacle)
            {
                spawnAttempts++;
                float spawnPosX = Random.Range(minBound + 5.0f, maxBound - 5.0f);
                float spawnPosY = Random.Range(minBound + 5.0f, maxBound - 5.0f);
                spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
                isValid = PreventSpawnOverLap(spawnPos, 3.0f);
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

public void InitiateWalls()
    {
        //Building side walls
        for(int i = 0; i < 10; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                    Instantiate(defaultWallPrefab, new Vector3(maxBound, maxBound - (4 * i), 0), Quaternion.identity);
                    Instantiate(defaultWallPrefab, new Vector3(-minBound +1.0f, maxBound - (4 * i), 0), Quaternion.identity);
            }
        }
        //Building Top and Bottom Walls
        for(int i = 0; i < 10; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                Instantiate(defaultWallPrefab, new Vector3(minBound + (4 * i), minBound, 0), Quaternion.Euler(0, 0, 90));
                Instantiate(defaultWallPrefab, new Vector3(minBound + (4 * i), maxBound, 0), Quaternion.Euler(0, 0, 90));
            }
        }
    }

    public void SpawnSnake()
    {
        Vector3 spawnPos = Vector3.zero;
        bool isValid = false;

        int spawnAttempts = 0;
        while (!isValid && spawnAttempts < maxSpawnAttemptsPerObstacle)
        {
            spawnAttempts++;
            float spawnPosX = Random.Range((int)minBound +5.0f, (int)maxBound -5.0f);
            float spawnPosY = Random.Range((int)minBound + 5.0f, (int)maxBound - 5.0f);
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

    public void SpawnAWall()
    {
            Vector3 spawnPos = Vector3.zero;
            bool isValid = false;
            int spawnAttempts = 0;
            while (!isValid && spawnAttempts < maxSpawnAttemptsPerObstacle)
            {
                spawnAttempts++;
                float spawnPosX = Random.Range(minBound + 5.0f, maxBound - 5.0f);
                float spawnPosY = Random.Range(minBound + 5.0f, maxBound - 5.0f);
                spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
                isValid = PreventSpawnOverLap(spawnPos, 3.0f);
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
