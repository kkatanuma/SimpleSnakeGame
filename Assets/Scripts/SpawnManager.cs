using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    Pathfinding pathfinding;
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
    public GameObject EnemyPrefab;
    public float[] yAngles = { 0, 180 };
    public float[] zAngles = { 0, 90, 270 };
    public float spawnProbablility = 0.25f;
    private int powerupCount;
    public int maxSpawnAttemptsPerObstacle = 20;
    public bool gameReady = false;
    private List<Vector3> enemySpawnPos;
  
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        pathfinding = new Pathfinding(40, 40);
        InitiateWalls();
        enemySpawnPos = new List<Vector3>()
        {
            new Vector3(36, 3, 0),
            new Vector3(36, 36, 0),
            new Vector3(3, 3, 0),
            new Vector3(3, 36, 0),
        };
    }

    public void Start()
    {
        SpawnSnake();
        InvokeRepeating("SpawnPowerups", startDelay, repeat);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWallPosition();
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("Powerup");
        powerupCount = powerups.Length;
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 1f + Vector3.one * .5f, new Vector3(path[i + 1].x, path[i + 1].y) * 1f + Vector3.one * .5f, Color.green, 5f);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }

    private Vector3 RoundToGrid(Vector3 position)
    {
        float gridSize = pathfinding.GetGrid().GetCellSize();
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector3(x, y, position.z) + Vector3.one * 0.5f;
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
            Instantiate(powerupPrefab, RoundToGrid(spawnPos), Quaternion.identity);
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
    public void InitiateWalls()
    {
        //Building side walls
        for (int i = 0; i < 10; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                Instantiate(defaultWallPrefab, new Vector3(maxBound, maxBound - (4 * i), 0), Quaternion.identity);
                Instantiate(defaultWallPrefab, new Vector3(-minBound + 1.0f, maxBound - (4 * i), 0), Quaternion.identity);
            }
        }
        //Building Top and Bottom Walls
        for (int i = 0; i < 10; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                Instantiate(defaultWallPrefab, new Vector3(minBound + (4 * i), minBound, 0), Quaternion.Euler(0, 0, 90));
                Instantiate(defaultWallPrefab, new Vector3(minBound + (4 * i), maxBound, 0), Quaternion.Euler(0, 0, 90));
            }
        }
        //Generating Random Walls
        for (int i = 0; i < maxWalls; i++)
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
                Instantiate(wallPrefabs[wallIndex], RoundToGrid(spawnPos), Quaternion.Euler(0, yAngles[yAngleIndex], zAngles[zAngleIndex]));
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
            float spawnPosX = Random.Range((int)minBound + 5.0f, (int)maxBound - 5.0f);
            float spawnPosY = Random.Range((int)minBound + 5.0f, (int)maxBound - 5.0f);
            spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
            isValid = PreventSpawnOverLap(spawnPos, 3.0f);
        }
        if (isValid)
        {

            Instantiate(SnakePrefab, RoundToGrid(spawnPos), Quaternion.identity);
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
            Instantiate(wallPrefabs[wallIndex], RoundToGrid(spawnPos), Quaternion.Euler(0, yAngles[yAngleIndex], zAngles[zAngleIndex]));
        }
    }

    public void SpawnEnemySnake()
    {
        int randomDir = Random.Range(0, enemySpawnPos.Count);
        GameObject enemySnake = Instantiate(EnemyPrefab, Vector3.zero, Quaternion.identity);
        if (randomDir == 2 || randomDir == 3)
        {
            enemySnake.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        Vector3 gridPosition = RoundToGrid(enemySpawnPos[randomDir]);
        enemySnake.transform.position = gridPosition;
        EnemySnake enemyScript =  enemySnake.GetComponent<EnemySnake>();
        Vector3 targetPos = FurtestPowerupPosition(gridPosition);

        List<Vector3> path = pathfinding.FindPath(gridPosition, targetPos);
        enemyScript.SetPath(path);
    }

    private void CheckWallPosition()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            pathfinding.GetNode(wall.transform.position).SetIsWalkable(false);
        }
    }

    Vector3 FurtestPowerupPosition(Vector3 pos)
    {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag("Powerup");
        float maxDistance = Mathf.NegativeInfinity;
        Vector3 targetPos = Vector3.zero;
        foreach(GameObject powerup in powerups)
        {
            float distance = (Vector3.Distance(pos, powerup.transform.position));
            if(distance > maxDistance)
            {
                maxDistance = distance;
                targetPos = powerup.transform.position;
            }
         }
        return targetPos;
    }
}
