using System.Collections.Generic;
using UnityEngine;


public class EnemySnake : Snake
{
    private int currentPathIndex;
    private List<Vector3> m_path;
    public List<Vector3> Path
    { 
        set { m_path = value; }
    }
    private float moveStep = 1.0f;
    protected override void Awake()
    {
        tr = transform;
        body = GetComponent<Rigidbody>();
        InitSnakeNodes();
        currentPathIndex = 0;
        movementFrequency = 0.08f;
    }

    /// <summary>
    /// Move Snake Head to given path positions moves nodes towards the previous positions
    /// Snake will be destroyed when reaching target position
    /// </summary>
    protected override void Move()
    {
        if (m_path != null && currentPathIndex < m_path.Count)
        {
            Vector3 parentPos = head.position;
            Vector3 targetPosition = m_path[currentPathIndex];
            head.position = Vector3.MoveTowards(head.position, targetPosition, moveStep);
            body.position = Vector3.MoveTowards(body.position, targetPosition, moveStep);

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
            //Reached Target Position
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set Direction and Change position of the nodes based on direction
    /// </summary>
    /// <param name="dir"></param>
    public void SetDirection(int dir)
    {
        m_currentDirection = (PlayerDirection)dir;
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
    /// Handles collision with other objects
    /// when collides with Powerup destroy EnemySnake
    /// when collides with Tail of a Player it will destroy the Player Snake
    /// and calls GameOver
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tags.POWERUP))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }else if (other.gameObject.CompareTag(Tags.TAIL))
        {
            Destroy(other.transform.parent.gameObject);
            GameManager.Instance.GameOver();
        }
    }

}