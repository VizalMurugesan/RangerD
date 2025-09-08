using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class PathFinder : MonoBehaviour
{
    public TileManager tileManager;
    public Grid2D grid;

    private void Awake()
    {
        tileManager = GetComponent<TileManager>();
        grid = new Grid2D(tileManager.MapWidth, tileManager.MapHeight);
        SetGridNodes();
        //StartCoroutine(Debugwalkable());
        
    }

    public void SetGridNodes()
    {

        for (int i = 0; i<tileManager.MapWidth; i++)
        {
            for(int j = 0; j<tileManager.MapHeight; j++)
            {
                Vector3Int Cell = new Vector3Int(i, j, 0);
                Vector3 Wordpos = tileManager.grid.CellToWorld(Cell);

                bool walkable = tileManager.IsWalkable(Cell);
                //Debug.Log(i+", "+ j+", "+ Wordpos+", "+ Cell+", "+ walkable);
                grid.nodes[i, j] = new Node(i, j, Wordpos, Cell, walkable, null);
            }
        }
        //debugNode();
    }

    void debugNode()
    {
        int count = 0;
        foreach (var node in grid.nodes)
        {
            //count++;
            if (node == null) Debug.Log("null node"+count);
        }
    }

    private IEnumerator Debugwalkable()
    {
        
        //if (grid == null || grid.nodes == null) return;
        
        for (int i = 0; i < tileManager.MapWidth; i++)
        {
            for (int j = 0; j < tileManager.MapHeight; j++)
            {
                
                Node node = grid.nodes[i, j];
                

                if (node.IsWalkable) {
                    
                    tileManager.DebugTileMap.SetTile(node.Cell, tileManager.DebugSprite); 
                }

                
            }
        }
        yield return new WaitForSeconds(4f);
        tileManager.DebugTileMap.ClearAllTiles();
        StartCoroutine(Debugwalkable());
    }

    public List<Node> FindPath(Vector3 StartPos, Vector3 TargetPos, Character charac)
    {

        
        List<Node> path = new List<Node>();

        Node startNode = GetNodeFromWorldPos(StartPos);
        Node targetNode = GetNodeFromWorldPos(TargetPos);

        

        PriorityQueue<Node, float> openSet = new PriorityQueue <Node, float>();

        Dictionary<Vector3Int,float> pathCost = new Dictionary<Vector3Int,float>(); //tracks g score
        Dictionary<Vector3Int, float> estimatedtotalCost = new Dictionary<Vector3Int, float>(); // tracks f score
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();


        openSet.Enqueue(startNode, 0f);
        pathCost.Add(startNode.Cell, 0f);
        estimatedtotalCost.Add(startNode.Cell, GetDistance(startNode, targetNode));



        while(openSet.Count > 0)
        {
            
            Node curr = openSet.Dequeue();
            if (curr.Cell.Equals(targetNode.Cell))
            { 
                path = ReconstructPath(cameFrom, curr.Cell);
                
                return path;
            }
            
            
            foreach (Node neighbour in grid.GetNeighbours(curr))
            {
                
                if (!grid.GetNode(neighbour.x, neighbour.y).IsWalkable) { continue; }
                
                
                float newGcost = pathCost[curr.Cell] + GetCost(neighbour,curr,charac);
                
                
                
                if (!pathCost.ContainsKey(neighbour.Cell))
                    
                {
                    
                    pathCost.Add(neighbour.Cell, newGcost);
                    cameFrom.Add(neighbour.Cell, curr.Cell);
                    estimatedtotalCost[neighbour.Cell] = newGcost+ GetDistance(neighbour, targetNode);
                    openSet.Enqueue(neighbour, estimatedtotalCost[neighbour.Cell]);
                }
                else if( newGcost < pathCost[neighbour.Cell])
                {

                    pathCost[neighbour.Cell] = newGcost;
                    cameFrom[neighbour.Cell] = curr.Cell;
                    estimatedtotalCost[neighbour.Cell] = newGcost + GetDistance(neighbour, targetNode);
                    openSet.Enqueue(neighbour, estimatedtotalCost[neighbour.Cell]);
                }
            }
        }


        
        return path;
    }

    public Node GetNodeFromWorldPos(Vector3 pos)
    {
        Node node = null;
        if (tileManager.grid == null)
        {
            
        }
        Vector3Int posint = tileManager.grid.WorldToCell(pos);
        
        
        node = grid.GetNode(posint.x, posint.y);
        
        return node;
    }


    private List<Node> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Node> path = new List<Node>();

        path.Add(grid.GetNode(current.x, current.y));
        
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            grid.GetNode(current.x, current.y).isReserved = true;
            //tileManager.DebugTileMap.SetTile(current, tileManager.DebugSprite);
            path.Add(grid.GetNode(current.x, current.y));
        }
        path.Reverse();
        path[0].isReserved = false;
        
        path.RemoveAt(0);
        
        return path;
    }

    private float GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.Cell.x - b.Cell.x);
        int dstY = Mathf.Abs(a.Cell.y - b.Cell.y);

        if (dstX > dstY)
            return 1.4f * dstY + 1f * (dstX - dstY); // diagonal movement
        return 1.4f * dstX + 1f * (dstY - dstX);
    }

    float GetCost(Node node, Node current, Character character)
    {
        float Cost = 1f;
        if(node.isReserved) {  Cost+= 5f; }
        //if (IsNeighbourDiagonal(current, node)) { Cost*= 1.5f; }
        //if(character.CurrentPath.Contains(node)){ Cost /= 1.5f; }
        return Cost;
    }

    bool IsNeighbourDiagonal(Node a, Node b)
    {
        Vector2Int diff = ((Vector2Int)a.Cell - (Vector2Int)b.Cell);
        return diff.x!=0 && diff.y!=0;
    }

    
   


}

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private readonly SortedDictionary<TPriority, Queue<TElement>> _dictionary = new();

    public int Count { get; private set; }

    public void Enqueue(TElement element, TPriority priority)
    {
        if (!_dictionary.ContainsKey(priority))
            _dictionary[priority] = new Queue<TElement>();

        _dictionary[priority].Enqueue(element);
        Count++;
    }

    public TElement Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException("The priority queue is empty.");

        var firstPair = _dictionary.First();
        var element = firstPair.Value.Dequeue();

        if (firstPair.Value.Count == 0)
            _dictionary.Remove(firstPair.Key);

        Count--;
        return element;
    }
}
