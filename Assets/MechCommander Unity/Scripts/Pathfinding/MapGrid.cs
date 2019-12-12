using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MechCommanderUnity;
using UnityEditor;
using System;

public class MapGrid : MonoBehaviour
{
    [Flags]
    public enum Mask
    {
        FullMask = -1,
        Bit1 = 0,
        Bit2 = 1,
        Bit3 = 2,
        Bit4 = 3,
        Bit5 = 4,
        Bit6 = 5,
        Bit7 = 6,
        Bit8 = 7,
        Extra1 = -2,
        Extra2 = -3,
        TileIndex = -4,
        Heigth = -5,
        TileId = -6,
        OverlayId = -7,
        MapCoords = -8,
        Nothing = -9,
    }

    public Mask MaskBit = Mask.Nothing;

    public bool displayGridGizmos;

    public Vector2 gridWorldSize;
    public Vector2 gridWorldBlocks;
    public Vector2 gridWorldVertexPerBlock;
    // public float nodeRadius;

    public float tileWidth;
    public float tileHeigth;
    public float tileZ;

    [SerializeField]
    [HideInInspector]
//    global::MCTile[] tileMap;



    float halfX2;
    float halfY2;

    MapNode[] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

//    void Awake()
//    {
//        //nodeDiameter = nodeRadius * 2;
//        //gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
//        //gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
//
//        gridSizeX = Mathf.RoundToInt(gridWorldBlocks.x * gridWorldVertexPerBlock.x);
//        gridSizeY = Mathf.RoundToInt(gridWorldBlocks.y * gridWorldVertexPerBlock.y);
//        halfX2 = tileWidth / 2f;
//        halfY2 = tileHeigth / 2f;
//
//        CreateGrid();
//    }
//
//    public int MaxSize
//    {
//        get
//        {
//            return gridSizeX * gridSizeY;
//        }
//    }
//
////    public global::MCTile[] TileMap
////    {
////        get { return tileMap; }
////        set { tileMap = value; }
////    }
//
//    void CreateGrid()
//    {
//        grid = new MapNode[gridSizeX * gridSizeY * 10];
//        var index = 0;
//        var walkable = true;
//
//        var gridIndex = 0;
//
//        for (int y = 0; y < gridWorldSize.y; y++)
//        {
//            for (int x = 0; x < gridWorldSize.x; x++)
//            {
//                index = IndexFromCoords((int)x, (int)y);
//                //var walk = tileMap[index].Mask < 41 ? true : false;
//                walkable = tileMap[index].Bit(Mask.Bit4)==0 ? true : false;
//
//                //for (float j = y; j < y+1; j+=0.1f)
//                //{
//                //    for (float i = x; i < x + 1; i += 0.1f)
//                //    {
//
//                //        Vector3 ScreenPosition = Vector3.zero;
//                //        var z = tileMap[index].Heigth;
//                //        ScreenPosition.x = ((i - j) * halfX2);
//                //        ScreenPosition.y = (((i + j) * -halfY2) + ((z) * tileZ));
//                //        ScreenPosition.z = z;
//
//                //        try
//                //        {
//                //            grid[gridIndex] = new MapNode(walkable, ScreenPosition, (int)i, (int)j);
//                //        } catch (Exception)
//                //        {
//
//                //            Debug.LogError("trying to insert " + gridIndex + " in  " +grid.Length);
//                //        }
//
//
//                //        gridIndex++;
//                //    }
//                //}
//
//
//                Vector3 ScreenPosition = Vector3.zero;
//                var z = tileMap[index].Heigth;
//                ScreenPosition.x = ((x - y) * halfX2);
//                ScreenPosition.y = (((x + y) * -halfY2) + ((z) * tileZ)) - halfY2;
//                ScreenPosition.z = z;
//
//
//                grid[index] = new MapNode(walkable, ScreenPosition, x, y);
//            }
//        }
//    }
//
//    public List<MapNode> GetNeighbours(MapNode node, int depth = 1)
//    {
//        List<MapNode> neighbours = new List<MapNode>();
//
//        for (int x = -depth; x <= depth; x++)
//        {
//            for (int y = -depth; y <= depth; y++)
//            {
//                if (x == 0 && y == 0)
//                    continue;
//
//                int checkX = node.gridX + x;
//                int checkY = node.gridY + y;
//
//                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
//                {
//                    neighbours.Add(grid[IndexFromCoords(checkX, checkY)]);
//                }
//            }
//        }
//
//        return neighbours;
//    }
//
//
//    public MapNode NodeFromWorldPoint(Vector3 worldPosition)
//    {
//        var half = tileWidth / 2;
//
//        float z = (worldPosition.z * (tileZ));
//
//        float screenX = worldPosition.x;
//        float screenY = worldPosition.y;
//
//        float isoX = ((screenX / half) + ((screenY - z) / -halfY2)) / 2;
//        float isoY = (((screenY - z) / -halfY2) - (screenX / half)) / 2;
//        return grid[IndexFromCoords((int)isoX, (int)isoY)];
//
//        /*
//        var isoPos = new Vector3(isoX, isoY, worldPosition.z);
//
//        
//
//
//        //var z = (worldPosition.z * (tileZ));
//
//        //var posX = (worldPosition.x / half + worldPosition.y / halfY2) / 2;
//        //var posY = (((worldPosition.y - (z)) / halfY2) - (worldPosition.x / half)) / 2;  // + (LstTiles[index].Heigth * ((float)MetersPerElevLevel / 100));
//
//
//        //var vc= new Vector3(-posX, -posY, 0);
//        Debug.Log(isoPos);
//
//        int blockX = (int) (isoPos.x / gridWorldVertexPerBlock.x);
//        int blockY = (int)(isoPos.y / gridWorldVertexPerBlock.x);
//
//        int vertexX = (int)(isoPos.x - (blockX * gridWorldVertexPerBlock.x));
//        int vertexY = (int)(isoPos.y - (blockY * gridWorldVertexPerBlock.x));
//
//        Debug.Log(blockX + " " + blockY);
//        Debug.Log(vertexX + " " + vertexY);
//
//        var index = (blockX * gridWorldBlocks.x + blockY) + (vertexX * gridWorldVertexPerBlock.x + vertexY);
//
//        Debug.Log(index);
//        return grid[(int)index];
//        var xx = isoPos.x / (gridWorldVertexPerBlock.x * gridWorldVertexPerBlock.y);
//        var yy = isoPos.y % (gridWorldVertexPerBlock.x * gridWorldVertexPerBlock.y);
//        Debug.Log(new Vector2(xx, yy));
//
//        float percentX = (isoPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
//        float percentY = (isoPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
//        percentX = Mathf.Clamp01(percentX);
//        percentY = Mathf.Clamp01(percentY);
//
//        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
//        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
//        Debug.Log(new Vector2(x, y));
//        return grid[x * gridSizeX + y];
//
//    */
//    }
//
//    public int IndexFromCoords(int checkX, int checkY)
//    {
//        int blockX = (int)(checkX / gridWorldVertexPerBlock.x);
//        int blockY = (int)(checkY / gridWorldVertexPerBlock.x);
//
//        int vertexX = (int)(checkX - (blockX * gridWorldVertexPerBlock.x));
//        int vertexY = (int)(checkY - (blockY * gridWorldVertexPerBlock.x));
//
//        //  Debug.Log(blockX + " " + blockY);
//        //   Debug.Log(vertexX + " " + vertexY);
//
//        var index = ((blockY * gridWorldBlocks.y + blockX) * gridWorldVertexPerBlock.y * gridWorldVertexPerBlock.x) + (vertexY * gridWorldVertexPerBlock.y + vertexX);
//
//        return (int)index;
//    }
//
//    public MapNode ClosestWalkableNode(MapNode node)
//    {
//        int maxRadius = Mathf.Max(gridSizeX, gridSizeY) / 2;
//        for (int i = 1; i < maxRadius; i++)
//        {
//            MapNode n = FindWalkableInRadius(node.gridX, node.gridY, i);
//            if (n != null)
//            {
//                return n;
//
//            }
//        }
//        return null;
//    }
//
//    MapNode FindWalkableInRadius(int centreX, int centreY, int radius)
//    {
//
//        for (int i = -radius; i <= radius; i++)
//        {
//            int verticalSearchX = i + centreX;
//            int horizontalSearchY = i + centreY;
//
//            // top
//            if (InBounds(verticalSearchX, centreY + radius))
//            {
//                if (grid[IndexFromCoords(verticalSearchX, centreY + radius)].walkable)
//                {
//                    return grid[IndexFromCoords(verticalSearchX, centreY + radius)];
//                }
//            }
//
//            // bottom
//            if (InBounds(verticalSearchX, centreY - radius))
//            {
//                if (grid[IndexFromCoords(verticalSearchX, centreY - radius)].walkable)
//                {
//                    return grid[IndexFromCoords(verticalSearchX, centreY - radius)];
//                }
//            }
//            // right
//            if (InBounds(centreY + radius, horizontalSearchY))
//            {
//                if (grid[IndexFromCoords((centreX + radius), horizontalSearchY)].walkable)
//                {
//                    return grid[IndexFromCoords((centreX + radius), horizontalSearchY)];
//                }
//            }
//
//            // left
//            if (InBounds(centreY - radius, horizontalSearchY))
//            {
//                if (grid[IndexFromCoords((centreX - radius), horizontalSearchY)].walkable)
//                {
//                    return grid[IndexFromCoords((centreX - radius), horizontalSearchY)];
//                }
//            }
//
//        }
//
//        return null;
//
//    }
//
//    bool InBounds(int x, int y)
//    {
//        return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
//    }
//
//    void OnDrawGizmos()
//    {
//        if (grid != null && displayGridGizmos)
//        {
//            var index = 0;
//            foreach (MapNode n in grid)
//            {
//                if (n == null) continue;
//
//                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
//                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
//                Gizmos.color = (n.walkable) ? Color.green : Color.red;
//                Gizmos.DrawCube(n.worldPosition, Vector3.one * (0.1f));
//
//                index = IndexFromCoords(n.gridX, n.gridY);
//
//                switch (this.MaskBit)
//                {
//                    case Mask.FullMask:
//                        Handles.Label(n.worldPosition, tileMap[index].Mask.ToString());
//                        break;
//                    case Mask.Extra1:
//                        Handles.Label(n.worldPosition, tileMap[index].otra.ToString());
//                        break;
//                    case Mask.Extra2:
//                        Handles.Label(n.worldPosition, tileMap[index].otro.ToString());
//                        break;
//                    case Mask.TileIndex:
//                        Handles.Label(n.worldPosition, index.ToString());
//                        break;
//                    case Mask.Heigth:
//                        Handles.Label(n.worldPosition, tileMap[index].Heigth.ToString());
//                        break;
//                    case Mask.TileId:
//                        Handles.Label(n.worldPosition, tileMap[index].TileId.ToString());
//                        break;
//                    case Mask.OverlayId:
//                        Handles.Label(n.worldPosition, tileMap[index].OverlayTileId.ToString());
//                        break;
//                    case Mask.MapCoords:
//                        Handles.Label(n.worldPosition, n.gridX + "-" + n.gridY);
//                        break;
//                    case Mask.Nothing:
//                        break;
//                    default:
//                        Handles.Label(n.worldPosition, tileMap[index].Bit(this.MaskBit).ToString());
//                        break;
//                }
//                //  index++;
//            }
//        }
//    }
}

