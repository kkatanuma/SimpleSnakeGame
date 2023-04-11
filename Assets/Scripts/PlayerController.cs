using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerDirection direction;

    public float speed = 0.95f;
    public float movementFrequency = 0.1f;
    private float counter;
    private bool move;

    [SerializeField]
    private GameObject tailPrefab;

    private List<Vector3> deltaPositions;
    private List<Rigidbody> nodes;

    private Rigidbody body;
    private Rigidbody head;
    private Transform tr;

    public bool addNode;

    // Start is called before the first frame update
    void Awake()
    {
        tr = transform;
        body = GetComponent<Rigidbody>();

        InitSnakeNodes();
        InitPlayer();

        deltaPositions = new List<Vector3>()        {
            new Vector3(-speed, 0f), //-dx
            new Vector3(0f, speed),  //dy
            new Vector3(speed, 0f),  //dx
            new Vector3(0f, -speed), //-dy
        };
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovementFrequency();
    }

    void FixedUpdate()
    {
        if (move)
        {
            move = false;
            Move();
        }
    }

    void InitSnakeNodes()
    {
        nodes = new List<Rigidbody>();
        nodes.Add(tr.GetChild(0).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(1).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(2).GetComponent<Rigidbody>());

        head = nodes[0];
    }

    void SetRandomDirection()
    {
        int randomDir = Random.Range(0, (int)PlayerDirection.COUNT);
        direction = (PlayerDirection)randomDir;
    }

    void InitPlayer()
    {
        SetRandomDirection();
        switch (direction)
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

    void Move()
    {
        Vector3 deltaPosition = deltaPositions[(int)direction];
        Vector3 parentPos = head.position;
        Vector3 prevPosition;

        body.position += deltaPosition;
        head.position += deltaPosition;

        for(int i = 1; i < nodes.Count; i++)
        {
            prevPosition = nodes[i].position;
            nodes[i].position = parentPos;
            parentPos = prevPosition;
        }

        //Check if we need to add node
        if (addNode)
        {
            addNode = false;
            GameObject newTail = Instantiate(tailPrefab, nodes[nodes.Count - 1].position, Quaternion.identity);

            //Keep the scale relative to parent
            newTail.transform.SetParent(transform ,true);
            nodes.Add(newTail.GetComponent<Rigidbody>());
        }
    }

    void CheckMovementFrequency()
    {
        counter += Time.deltaTime;
        if(counter >= movementFrequency)
        {
            counter = 0f;
            move = true;
        }
    }

    public void SetInputDirection(PlayerDirection dir)
    {
        //Prevent change direction to opposite sides
        if(dir == PlayerDirection.UP && direction == PlayerDirection.DOWN ||
           dir == PlayerDirection.DOWN && direction == PlayerDirection.UP ||
           dir == PlayerDirection.RIGHT && direction == PlayerDirection.LEFT ||
           dir == PlayerDirection.LEFT && direction == PlayerDirection.RIGHT)
        {
            return;
        }else
        {
            direction = dir;

            //TO handle playerInput immediately
            ForceMove();
        }
        
    }

    void ForceMove()
    {
        counter = 0;
        move = false;
        Move();
    }

    public void Grow()
    {
        GameObject newTail = Instantiate(tailPrefab, nodes[nodes.Count - 1].position += deltaPositions[(int)direction], Quaternion.identity);
        nodes.Add(tr.GetChild(nodes.Count).GetComponent<Rigidbody>());
    }

}
