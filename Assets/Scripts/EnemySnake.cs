using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class EnemySnake : Snake
{
    protected override void FixedUpdate()
    {
/*        if (move)
        {
            move = false; // Reset move to false after updating the position
            if (path != null && currentPathIndex < path.Count)
            {
                Vector3 parentPos = head.position;
                Vector3 targetPosition = path[currentPathIndex];
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
        }*/
    }
}