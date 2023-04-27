using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using JetBrains.Annotations;

public class EnemySnake : Snake
{
    int currentPathIndex;
    List<Vector3> m_path;
    private float moveStep = 1.0f;
    protected override void Awake()
    {
        tr = transform;
        body = GetComponent<Rigidbody>();
        InitSnakeNodes();
        currentPathIndex = 0;
        m_movementFrequency = 0.08f;
    }

    protected override void FixedUpdate()
    {
        if (move)
        {
            move = false; // Reset move to false after updating the position
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
                Destroy(gameObject);
            }
        }
    }

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

    public void SetPath(List<Vector3> path)
    {
        m_path = path;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }else if (other.gameObject.CompareTag("Tail"))
        {
            Destroy(other.transform.parent.gameObject);
            WorldManager.instance.GameOver();
        }
    }

}