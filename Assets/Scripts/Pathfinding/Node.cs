using UnityEngine;

public class Node : MonoBehaviour
{
    public int x;
    public int y;

    public float hcost; //to target
    public float gcost; //from start
    public float fcost => hcost + gcost;

    public bool IsWalkable;

    public Node Parent;

    public Vector3 WorldPos;

    public Vector3Int Cell;
    

    public Node(int x, int y, Vector3 WorldPos, Vector3Int Cell, bool Iswalkable, Node parent )
    {
        this.x = x;
        this.y = y;
        this.WorldPos = WorldPos;
        this.Cell = Cell;
        IsWalkable = Iswalkable;
        Parent = parent;
        
    }

    // For heap priority (lower FCost is "better")
    public int CompareTo(Node other)
    {
        int compare = fcost.CompareTo(other.fcost);
        if (compare == 0) compare = hcost.CompareTo(other.hcost);
        return compare; // lower is better
    }
    public bool Equals(Node other)
    {
        return Cell == other.Cell;
    }

}
