using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    Pathfinding pathfinding;
    private int cellSize = 1;
    [Header("SpawnSettings")]
    private float minBound = 0.5f;
    private float maxBound = 39.5f;
    private float spawnDelay = 2.0f;
    private float maxPowerups = 5;
    private int maxWalls = 10;
    private float repeat = 1.0f;
    private float[] yAngles = { 0, 180 };
    private float[] zAngles = { 0, 90, 270 };
    private float spawnProbablility = 0.25f;
    private int powerupCount = 0;
    private int maxSpawnAttemptsPerObstacle = 20;
    private List<Vector3> enemySpawnPos;

    [Header("Prefabs")]
    public GameObject powerupPrefab;
    public GameObject defaultWallPrefab;
    public GameObject[] wallPrefabs;
    public GameObject SnakePrefab;
    public GameObject EnemyPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        pathfinding = new Pathfinding(40, 40, cellSize);
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
        InvokeRepeating("SpawnPowerups", spawnDelay, repeat);
    }

    // Update is called once per frame
    void Update()
    {
        CheckWallPositionInGrid();
        powerupCount = CheckPowerupCount();
    }

    /// <summary>
    /// Check wall position and set corresponding PathNode to false
    /// so the wall is excluded from path
    /// </summary>
    private void CheckWallPositionInGrid()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag(Tags.WALL);
        if (walls.Length > 0)
        {
            foreach (GameObject wall in walls)
            {
                pathfinding.GetNode(wall.transform.position).SetIsWalkable(false);
            }
        }
    }

    /// <summary>
    /// Check how many Powerups in the world
    /// </summary>
    /// <returns></returns>
    private int CheckPowerupCount()
    {
        GameObject[] powerups = GameObject.FindGameObjectsWithTag(Tags.POWERUP);
        return powerups.Length;
    }


    /// <summary>
    /// Spawn powerup in the location doesn't overlap with other objects
    /// when powerups is less than Max Powerups 
    /// </summary>
    void SpawnPowerups()
    {
        Vector3 spawnPos = FindValidSpawnPosition();

        if (spawnPos != Vector3.zero && powerupCount < maxPowerups)
        {
            Instantiate(powerupPrefab, RoundToGrid(spawnPos), Quaternion.identity);
        }
    }

    /// <summary>
    /// Returns valid position doesn't overlap with other objects 
    /// if no valid location is found method will return Vector3.zero
    /// </summary>
    /// <returns></returns>
    Vector3 FindValidSpawnPosition()
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
        return isValid ? spawnPos : Vector3.zero;
    }


    /// <summary>
    /// Check if Desired Position will collides with other objects in the world
    /// within given radius
    /// </summary>
    /// <param name="spawnPos">Target Position</param> 
    /// <param name="checkRadius">Radius to check for other objects</param>
    /// <returns></returns>
    bool PreventSpawnOverLap(Vector3 spawnPos, float checkRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(spawnPos, checkRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.tag == Tags.WALL || collider.tag == Tags.POWERUP 
                || collider.tag == Tags.SNAKE || collider.tag == Tags.TAIL)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Convert WorldPosition to GridPosition
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector3 RoundToGrid(Vector3 position)
    {
        float gridSize = pathfinding.GetGrid().GetCellSize();
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector3(x, y, position.z) + Vector3.one * 0.5f;
    }

    public void InitiateWalls()
    {
        //Building side walls
        for (int i = 0; i < maxWalls; i++)
        {
            float randomValue = Random.Range(0f, 1f);
            if (randomValue >= spawnProbablility)
            {
                Instantiate(defaultWallPrefab, new Vector3(maxBound, maxBound - (4 * i), 0), Quaternion.identity);
                Instantiate(defaultWallPrefab, new Vector3(-minBound + 1.0f, maxBound - (4 * i), 0), Quaternion.identity);
            }
        }
        //Building Top and Bottom Walls
        for (int i = 0; i < maxWalls; i++)
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
            Vector3 spawnPos = FindValidSpawnPosition();

            if (spawnPos != Vector3.zero)
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
        Vector3 spawnPos = FindValidSpawnPosition();

        if (spawnPos != Vector3.zero)
        {
            Instantiate(SnakePrefab, RoundToGrid(spawnPos), Quaternion.identity);
        }
        else
        {
            //Just in case, set default location
            Instantiate(SnakePrefab, new Vector3 (3, 3, 0), Quaternion.identity);
        }
    }

    public void SpawnAWall()
    {
        Vector3 spawnPos = FindValidSpawnPosition();

        if (spawnPos != Vector3.zero)
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
        enemyScript.Path = pathfinding.FindPath(gridPosition, targetPos);
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
