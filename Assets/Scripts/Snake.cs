using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Snake Configuration")]
    private float speed = 1.0f;
    protected PlayerDirection m_currentDirection;
    public PlayerDirection CurrentDirection
    {
        get { return m_currentDirection; }
        set { m_currentDirection = value; }
    }
    [Header("Snake Components")]
    protected Rigidbody body;
    protected Rigidbody head;
    protected Transform tr;
    protected List<Rigidbody> nodes;
    private Collider snakeCollider;
    public GameObject tailPrefab;

    private float counter;
    protected float movementFrequency = 0.1f;
    protected bool move;
    private bool addNode;
    private float minBounds = 0.0f;
    private float maxBounds = 40.0f;
    private List<Vector3> deltaPositions;
    private List<Vector3> previousHeadPositions;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        tr = transform;
        body = GetComponent<Rigidbody>();
        snakeCollider = GetComponent<Collider>();
        InitSnakeNodes();
        SpawnSnake();

        deltaPositions = new List<Vector3>(){
            new Vector3(-speed, 0f), //-dx
            new Vector3(0f, speed),  //dy
            new Vector3(speed, 0f),  //dx
            new Vector3(0f, -speed), //-dy
        };
        previousHeadPositions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovementFrequency();
    }

    protected void FixedUpdate()
    {
        if (move)
        {
            move = false;
            Move();
        }
    }

    /// <summary>
    /// Initialize Snake, Add Rigidbody in a List for easier management while moving
    /// </summary>
    protected void InitSnakeNodes()
    {
        nodes = new List<Rigidbody>
        {
            tr.GetChild(0).GetComponent<Rigidbody>(),
            tr.GetChild(1).GetComponent<Rigidbody>(),
            tr.GetChild(2).GetComponent<Rigidbody>()
        };
        head = nodes[0];
    }

    /// <summary>
    /// Spawn snake in random direction and adjust corresponding nodes depending on 
    /// current direction
    /// </summary>
    private void SpawnSnake()
    {
        SetRandomDirection();
        switch (m_currentDirection)
        {
            case PlayerDirection.RIGHT:
                nodes[1].position = nodes[0].position - new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position - new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;
            case PlayerDirection.LEFT:
                nodes[1].position = nodes[0].position + new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position + new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;
            case PlayerDirection.UP:
                nodes[1].position = nodes[0].position - new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position - new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
            case PlayerDirection.DOWN:
                nodes[1].position = nodes[0].position + new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position + new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
        }

    }

    /// <summary>
    /// Returns random direction by pursing to PlayerDirection enum
    /// </summary>
    public void SetRandomDirection()
    {
        int randomDir = Random.Range(0, (int)PlayerDirection.COUNT);
        m_currentDirection = (PlayerDirection)randomDir;
    }

    /// <summary>
    /// Updates counter to keep track of movement frequency
    /// </summary>
    protected void CheckMovementFrequency()
    {
        counter += Time.deltaTime;
        if (counter >= movementFrequency)
        {
            counter = 0f;
            move = true;
        }
    }

    /// <summary>
    /// Move position of head using deltaPositoins with currentDirection
    /// once head is updated, update all nodes by following parent position
    /// If addNode is set to True, New Tail will be added
    /// </summary>
    protected virtual void Move()
    {
        Vector3 deltaPosition = deltaPositions[(int)m_currentDirection];
        Vector3 parentPos = head.position;

        body.position += deltaPosition;
        head.position += deltaPosition;

        if (IsHeadOutOfBounds())
        {
            WrapAround();
        }

        UpdateNodePositions(parentPos);

        if (addNode)
        {
            addNode = false;
            GameObject newTail = Instantiate(tailPrefab, nodes[nodes.Count - 1].position, Quaternion.identity);
            newTail.transform.SetParent(transform, true);
            nodes.Add(newTail.GetComponent<Rigidbody>());
        }
    }

    /// <summary>
    /// Checks if Snake Head is out of bounds 
    /// </summary>
    /// <returns></returns>
    private bool IsHeadOutOfBounds()
    {
        return head.position.x > maxBounds || head.position.x < minBounds || head.position.y > maxBounds || head.position.y < minBounds;
    }

    /// <summary>
    /// Moves snake to the opposite side of the world when going 
    /// out of bounds
    /// </summary>
    private void WrapAround()
    {
        previousHeadPositions.Add(head.position);

        if (head.position.x > maxBounds)
        {
            SetHeadAndColliderPosition(new Vector3(minBounds, head.position.y, 0f));
        }
        else if (head.position.x < minBounds)
        {
            SetHeadAndColliderPosition(new Vector3(maxBounds, head.position.y, 0f));
        }
        else if (head.position.y > maxBounds)
        {
            SetHeadAndColliderPosition(new Vector3(head.position.x, -minBounds, 0f));
        }
        else if (head.position.y < -minBounds)
        {
            SetHeadAndColliderPosition(new Vector3(head.position.x, maxBounds, 0f));
        }
    }

    /// <summary>
    /// Update position of head and collider box.
    /// Fixed issue of collider not updating when going out of bounds
    /// </summary>
    /// <param name="position"></param> 
    private void SetHeadAndColliderPosition(Vector3 position)
    {
        head.position = position;
        snakeCollider.transform.position = position;
    }

    /// <summary>
    /// Move each nodes by updating to previous nodes position
    /// when Snake wraparound it will store snake head as a
    /// temporary buffer
    /// </summary>
    /// <param name="parentPos"></param>
    private void UpdateNodePositions(Vector3 parentPos)
    {
        Vector3 prevPosition;

        for (int i = 1; i < nodes.Count; i++)
        {
            if (previousHeadPositions.Count > 0)
            {
                prevPosition = nodes[i].position;
                nodes[i].position = previousHeadPositions[0];
                parentPos = prevPosition;
                previousHeadPositions.RemoveAt(0);
            }
            else
            {
                prevPosition = nodes[i].position;
                nodes[i].position = parentPos;
                parentPos = prevPosition;
            }
        }
    }

    /// <summary>
    /// Handles collision with other objects
    /// when collides with powerups it will set addNode to True
    /// when colliding with any other objects in the game it will call GameOver
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tags.POWERUP))
        {
            addNode = true;
            Destroy(other.gameObject);
            ScoreManager.Instance.AddPoint();
            WorldManager.Instance.AddPoint();
        }
        else if (other.gameObject.CompareTag(Tags.WALL) ||
            other.gameObject.CompareTag(Tags.TAIL) || other.gameObject.CompareTag(Tags.SNAKE))
        {
            Destroy(gameObject);
            WorldManager.Instance.GameOver();
        }
    }
}
