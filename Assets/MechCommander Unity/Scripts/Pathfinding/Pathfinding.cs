using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{

    MapGrid grid;
    static Pathfinding instance;

    void Awake()
    {
        grid = GetComponent<MapGrid>();
        instance = this;
    }

//    public static Vector2[] RequestPath(Vector3 from, Vector3 to)
//    {
//        return instance.FindPath(from, to);
//    }
//
//    Vector2[] FindPath(Vector3 from, Vector3 to)
//    {
//
//        Stopwatch sw = new Stopwatch();
//        sw.Start();
//
//        Vector2[] waypoints = new Vector2[0];
//        bool pathSuccess = false;
//
//        MapNode startNode = grid.NodeFromWorldPoint(from);
//        MapNode targetNode = grid.NodeFromWorldPoint(to);
//        startNode.parent = startNode;
//
//        if (!startNode.walkable)
//        {
//            startNode = grid.ClosestWalkableNode(startNode);
//        }
//        if (!targetNode.walkable)
//        {
//            targetNode = grid.ClosestWalkableNode(targetNode);
//        }
//
//        if (startNode.walkable && targetNode.walkable)
//        {
//
//            Heap<MapNode> openSet = new Heap<MapNode>(grid.MaxSize);
//            HashSet<MapNode> closedSet = new HashSet<MapNode>();
//            openSet.Add(startNode);
//
//            while (openSet.Count > 0)
//            {
//                MapNode currentNode = openSet.RemoveFirst();
//                closedSet.Add(currentNode);
//
//                if (currentNode == targetNode)
//                {
//                    sw.Stop();
//                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
//                    pathSuccess = true;
//                    break;
//                }
//
//                foreach (MapNode neighbour in grid.GetNeighbours(currentNode))
//                {
//                    if (!neighbour.walkable || closedSet.Contains(neighbour))
//                    {
//                        continue;
//                    }
//
//                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + TurningCost(currentNode, neighbour);
//                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
//                    {
//                        neighbour.gCost = newMovementCostToNeighbour;
//                        neighbour.hCost = GetDistance(neighbour, targetNode);
//                        neighbour.parent = currentNode;
//
//                        if (!openSet.Contains(neighbour))
//                            openSet.Add(neighbour);
//                        else
//                            openSet.UpdateItem(neighbour);
//                    }
//                }
//            }
//        }
//
//        if (pathSuccess)
//        {
//            waypoints = RetracePath(startNode, targetNode);
//        }
//
//        return waypoints;
//
//    }
//
//
//    int TurningCost(MapNode from, MapNode to)
//    {
//        /*
//		Vector2 dirOld = new Vector2(from.gridX - from.parent.gridX, from.gridY - from.parent.gridY);
//		Vector2 dirNew = new Vector2(to.gridX - from.gridX, to.gridY - from.gridY);
//		if (dirNew == dirOld)
//			return 0;
//		else if (dirOld.x != 0 && dirOld.y != 0 && dirNew.x != 0 && dirNew.y != 0) {
//			return 5;
//		}
//		else {
//			return 10;
//		}
//		*/
//
//        return 0;
//    }
//
//    Vector2[] RetracePath(MapNode startNode, MapNode endNode)
//    {
//        List<MapNode> path = new List<MapNode>();
//        MapNode currentNode = endNode;
//
//        while (currentNode != startNode)
//        {
//            path.Add(currentNode);
//            currentNode = currentNode.parent;
//        }
//        Vector2[] waypoints = SimplifyPath(path);
//        Array.Reverse(waypoints);
//        return waypoints;
//
//    }
//
//    Vector2[] SimplifyPath(List<MapNode> path)
//    {
//        List<Vector2> waypoints = new List<Vector2>();
//        Vector2 directionOld = Vector2.zero;
//
//        for (int i = 1; i < path.Count; i++)
//        {
//            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
//            if (directionNew != directionOld)
//            {
//                waypoints.Add(path[i].worldPosition);
//            }
//            directionOld = directionNew;
//        }
//        return waypoints.ToArray();
//    }
//
//    int GetDistance(MapNode nodeA, MapNode nodeB)
//    {
//        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
//        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
//
//        if (dstX > dstY)
//            return 14 * dstY + 10 * (dstX - dstY);
//        return 14 * dstX + 10 * (dstY - dstX);
//    }


}