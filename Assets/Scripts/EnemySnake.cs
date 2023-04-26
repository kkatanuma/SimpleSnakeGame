using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemySnake : MonoBehaviour
{
    //[SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveStep = 1f;
    Pathfinding pathfinding;
    private float movementFrequency = 0.1f; // Set movementFrequency to 1 so the snake moves every second
    private float counter;
    public bool move;
    Vector3 startPosition;
    Vector3 targetPosition;
    private Transform tr;
    private List<Rigidbody> nodes;
    private Rigidbody head;

    private List<Vector3> path;
    private int currentPathIndex;


    private void Awake()
    {
        tr = transform;
        InitSnakeNodes();
    }
    private void Start()
    {
        pathfinding = new Pathfinding(40, 40);
        Vector3 startPosition = RoundToGrid(transform.position);
        targetPosition = RoundToGrid(new Vector3(35, 37)); // Set your desired target position
        SetTargetPosition(targetPosition);
    }

    void Update()
    {
        CheckMovementFrequency();
        CheckWallPositions();
        
        if(Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if(path != null)
            {
                for(int i=0; i < path.Count -1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 1f + Vector3.one * .5f, new Vector3(path[i + 1].x, path[i + 1].y) * 1f + Vector3.one * .5f, Color.green, 5f);
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }
    }

    private void FixedUpdate()
    {
        if (move)
        {
            move = false; // Reset move to false after updating the position

            if (path != null && currentPathIndex < path.Count)
            {
                Vector3 parentPos = head.position;
                Vector3 targetPosition = path[currentPathIndex];
                head.position = Vector3.MoveTowards(head.position, targetPosition, moveStep);

                if (Vector3.Distance(head.position, targetPosition) < 0.1f)
                {
                    currentPathIndex++;
                }

                for (int i = 1; i < nodes.Count; i++)
                {
                    Vector3 prevPos = nodes[i].position;
                    nodes[i].position = Vector3.MoveTowards(nodes[i].position, parentPos, moveStep);
                    parentPos = prevPos;
                }
            }
            else
            {
                Debug.Log("can't find the path");
            }
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        CheckWallPositions();
        Vector3 startPosition = RoundToGrid(transform.position);
        path = Pathfinding.Instance.FindPath(startPosition, targetPosition);

        if (path != null)
        {
            path.RemoveAt(0); // Remove the starting position from the path
            currentPathIndex = 0;
        }
        else
        {
            Debug.LogError("Path not found!");
        }
    }

    private Vector3 RoundToGrid(Vector3 position)
    {
        float gridSize = pathfinding.GetGrid().GetCellSize();
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector3(x, y, position.z);
    }

    void CheckMovementFrequency()
    {
        counter += Time.deltaTime;
        if (counter >= movementFrequency)
        {
            counter = 0f;
            move = true;
        }
    }

    void CheckWallPositions()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach(GameObject wall in walls)
        {
            Pathfinding.Instance.GetNode(wall.transform.position).SetIsWalkable(false);
        }
    }

    private void InitSnakeNodes()
    {
        nodes = new List<Rigidbody>
        {
            tr.GetChild(0).GetComponent<Rigidbody>(),
            tr.GetChild(1).GetComponent<Rigidbody>(),
            tr.GetChild(2).GetComponent<Rigidbody>()
        };

        head = nodes[0];
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tail"))
        {
            //Destroy(gameObject);
            //WorldManager.instance.GameOver();
            Debug.Log("game over");
        }
    }
}