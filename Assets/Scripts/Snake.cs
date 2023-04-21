using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private uint playerId;
    private float speed = 1.0f;
    private float movementFrequency = 0.1f;
    private float bounds = 19.5f;
    private float counter;
    private bool move;
    public PlayerDirection currentDirection;
    public GameObject tailPrefab;
    private List<Vector3> deltaPositions;
    private List<Vector3> previousHeadPositions;
    private List<Rigidbody> nodes;

    private Rigidbody body;
    private Rigidbody head;
    private Transform tr;
    private Collider snakeCollider;

    public bool addNode;
    private bool outOfBounds;

    // Start is called before the first frame update
    void Awake()
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

    void FixedUpdate()
    {
        if(move)
        {
            move = false;
            Move();
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

    private void SpawnSnake()
    {
        SetRandomDirection();
        switch (currentDirection)
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

    void SetRandomDirection()
    {
        int randomDir = Random.Range(0, (int)PlayerDirection.COUNT);
        currentDirection = (PlayerDirection)randomDir;
    }

protected virtual void Move()
{
    Vector3 deltaPosition = deltaPositions[(int)currentDirection];
    Vector3 parentPos = head.position;
    Vector3 prevPosition;

    body.position += deltaPosition;
    head.position += deltaPosition;

    // Wrap around if the snake goes out of bounds
    if (head.position.x > bounds || head.position.x < -bounds || head.position.y > bounds || head.position.y < -bounds)
    {
        // Add the previous head position to previousHeadPositions list
        previousHeadPositions.Add(parentPos);

        if (head.position.x > bounds)
        {
            head.position = new Vector3(-bounds, head.position.y, 0f);
            snakeCollider.transform.position = new Vector3(-bounds, head.position.y, 0f);
        }
        else if (head.position.x < -bounds)
        {
            head.position = new Vector3(bounds, head.position.y, 0f);
            snakeCollider.transform.position = new Vector3(bounds, head.position.y, 0f);
        }
        else if (head.position.y > bounds)
        {
            head.position = new Vector3(head.position.x, -bounds, 0f);
            snakeCollider.transform.position = new Vector3(head.position.x, -bounds, 0f);
        }
        else if (head.position.y < -bounds)
        {
            head.position = new Vector3(head.position.x, bounds, 0f);
            snakeCollider.transform.position = new Vector3(head.position.x, bounds, 0f);
        }
    }

    for (int i = 1; i < nodes.Count; i++)
    {
        // Check if there is a previous head position for this node to follow
        if (previousHeadPositions.Count > 0)
        {
            prevPosition = nodes[i].position;
            nodes[i].position = previousHeadPositions[0];
            parentPos = prevPosition;

            // Remove the followed position from the list
            previousHeadPositions.RemoveAt(0);
        }
        else
        {
            prevPosition = nodes[i].position;
            nodes[i].position = parentPos;
            parentPos = prevPosition;
        }
    }

    // Check if we need to add node
    if (addNode)
    {
        addNode = false;
        GameObject newTail = Instantiate(tailPrefab, nodes[nodes.Count - 1].position, Quaternion.identity);

        // Keep the scale relative to parent
        newTail.transform.SetParent(transform, true);
        nodes.Add(newTail.GetComponent<Rigidbody>());
    }
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

    public void ForceMove()
    {
        counter = 0;
        move = false;
        Move();
    }

    public void Grow()
    {
        GameObject newTail = Instantiate(tailPrefab, nodes[nodes.Count - 1].position += deltaPositions[(int)currentDirection], Quaternion.identity) ;
        nodes.Add(tr.GetChild(nodes.Count).GetComponent<Rigidbody>());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Debug.Log("Powerup collected");
            addNode = true;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Game Over!! Collided with: " + other.name);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Tail"))
        {
            Debug.Log("Game Over!! Collided with: " + other.name + gameObject.GetInstanceID());
            Destroy(gameObject);
        }
    }
}
