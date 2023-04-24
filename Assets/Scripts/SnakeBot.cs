using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SnakeBot : Snake
{
    public GameObject[] powerups;
    private Pathfinding pathfinding;
    private List<Vector3> path;
    private int stepCounter;
    private int stepsBeforeChangeDirection = 5; // Change direction every 5 steps


    protected override void Update()
    {
        base.Update();

        if (move)
        {
            stepCounter++;

            // Check for powerups
            powerups = GameObject.FindGameObjectsWithTag("Powerup");

            if (powerups.Length > 0)
            {
                // Move towards the nearest powerup using pathfinding
                GameObject nearestPowerup = FindNearestPowerup();
                path = Pathfinding.Instance.FindPath(head.position, nearestPowerup.transform.position);

                if (path != null && path.Count > 1)
                {
                    Vector3 nextPosition = path[1];

                    // Check if the next position is walkable, if not, change direction
                    if (IsNextPositionWalkable(nextPosition))
                    {
                        SetDirection(nextPosition - head.position);
                    }
                    else
                    {
                        ChangeDirectionToNextInCircle();
                    }
                }
            }
            else
            {
                // Move in a circular pattern if there are no powerups
                if (stepCounter >= stepsBeforeChangeDirection)
                {
                    stepCounter = 0;
                    ChangeDirectionToNextInCircle();
                }
            }
        }
    }

    bool IsNextPositionWalkable(Vector3 nextPosition)
    {

        if (pathfinding == null)
        {
            return true; // If pathfinding is not initialized yet, assume the position is walkable
        }

        int x, y;
        pathfinding.GetGrid().GetXY(nextPosition, out x, out y);
        PathNode nextNode = pathfinding.GetGrid().GetGridObject(x, y);
        return nextNode.isWalkable;
    }

    GameObject FindNearestPowerup()
    {
        GameObject nearestPowerup = powerups[0];
        float minDistance = Vector3.Distance(head.position, nearestPowerup.transform.position);

        foreach (GameObject powerup in powerups)
        {
            float distance = Vector3.Distance(head.position, powerup.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPowerup = powerup;
            }
        }

        return nearestPowerup;
    }

    void SetDirection(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);

        if (angle >= -45 && angle < 45)
        {
            currentDirection = PlayerDirection.UP;
        }
        else if (angle >= 45 && angle < 135)
        {
            currentDirection = PlayerDirection.RIGHT;
        }
        else if (angle >= 135 || angle < -135)
        {
            currentDirection = PlayerDirection.DOWN;
        }
        else if (angle >= -135 && angle < -45)
        {
            currentDirection = PlayerDirection.LEFT;
        }
    }

    void ChangeDirectionToNextInCircle()
    {
        // Change the direction in a clockwise order: UP -> RIGHT -> DOWN -> LEFT -> UP
        switch (currentDirection)
        {
            case PlayerDirection.UP:
                currentDirection = PlayerDirection.RIGHT;
                break;
            case PlayerDirection.RIGHT:
                currentDirection = PlayerDirection.DOWN;
                break;
            case PlayerDirection.DOWN:
                currentDirection = PlayerDirection.LEFT;
                break;
            case PlayerDirection.LEFT:
                currentDirection = PlayerDirection.UP;
                break;
        }
    }


}