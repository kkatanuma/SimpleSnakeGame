/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBot : Snake
{

    private GameObject powerup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    protected override void Move()
    {
        // Find the powerup in the scene if not found yet
        if (powerup == null)
        {
            powerup = GameObject.FindGameObjectWithTag("Powerup");
        }

        if (powerup != null)
        {
            // Implement A* pathfinding algorithm to find the shortest path to the powerup
            List<Vector3> path = FindPathToPowerup();

            // Set the direction based on the next step in the path
            if (path != null && path.Count > 0)
            {
                SetDirectionFromPath(path[0]);
            }
        }

        // Call the base Move() method to handle movement and collisions
        base.Move();
    }

    private List<Vector3> FindPathToPowerup()
    {
        // Implement the A* pathfinding algorithm here and return the path as a list of Vector3 positions
        // You can use a third-party A* pathfinding library or create your own implementation
        return new List<Vector3>();
    }

    private void SetDirectionFromPath(Vector3 nextStep)
    {
        // Calculate the direction based on the next step in the path
        Vector3 delta = nextStep - nodes[0].position;

        if (delta.x < 0)
        {
            currentDirection = PlayerDirection.LEFT;
        }
        else if (delta.x > 0)
        {
            currentDirection = PlayerDirection.RIGHT;
        }
        else if (delta.y < 0)
        {
            currentDirection = PlayerDirection.DOWN;
        }
        else if (delta.y > 0)
        {
            currentDirection = PlayerDirection.UP;
        }
    }
}

*/