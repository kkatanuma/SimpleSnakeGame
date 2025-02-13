using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(int width, int height, int cellSize)
    {
        grid = new Grid<PathNode>(width, height, cellSize, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public Pathfinding(float width, float height, float cellSize)
    {
        grid = new Grid<PathNode>((int)width, (int)height, (int)cellSize, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }


    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y, 0) * grid.GetCellSize() + new Vector3(0.5f, 0.5f, 0) * grid.GetCellSize());
            }
            return vectorPath;
        }
    }

    /// <summary>
    /// Find shortest path in grids from starting points to end points
    /// path are calculated by following A* pathfinding algorithm 
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns></returns>
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
 
        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();  

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) continue;
                if (!neighborNode.isWalkable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);
                if(tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }
        //No nodes left in openList
        return null;
    }


    private List<PathNode>CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Check Neighbor of current Node for 4 directions
    /// </summary>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }


    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i=1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public PathNode GetNode(int x, int y)
    {
       return grid.GetGridObject(x, y);
    }

    public PathNode GetNode(Vector3 position)
    {
        return grid.GetGridObject((int)position.x, (int)position.y);
    }
    public Grid<PathNode> GetGrid()
    {
        return grid;
    }
}
