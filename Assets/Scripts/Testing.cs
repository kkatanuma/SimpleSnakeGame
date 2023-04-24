using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;
    private List<PathNode> path;
    private Vector3 mouseWorldPosition;
    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(40, 40);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            Debug.Log(x + ":" + y);
            path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                Debug.Log("Path: ");
                for (int i = 0; i < path.Count; i++)
                {
                    Debug.Log(path[i].x + "," + path[i].y);
                }

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Vector3 startPosition = new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 0.5f;
                    Vector3 endPosition;
                    if (i == path.Count - 2)
                    {
                        endPosition = mouseWorldPosition;
                    }
                    else
                    {
                        endPosition = new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 0.5f;
                    }
                    Debug.DrawLine(startPosition, endPosition, Color.green, 5f);
                }
            }
        }
    }

}
