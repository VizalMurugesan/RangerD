using UnityEngine;

public class Grid2D: MonoBehaviour
{
    public int width;
    public int height;
    public Node[,] nodes;

    public Grid2D(int width, int height)
    {
        this.width = width;
        this.height = height;
        nodes = new Node[width, height];
    }

    public void SetNode(int x, int y, Node node)
    {
        nodes[x, y] = node;
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return nodes[x, y];
        return null;
    }

    // Get neighbours in 4 directions (can expand to 8)
    public Node[] GetNeighbours(Node node)
    {
        return new Node[]
        {
            GetNode(node.x + 1, node.y),
            GetNode(node.x - 1, node.y),
            GetNode(node.x, node.y + 1),
            GetNode(node.x, node.y - 1),
            GetNode(node.x+1, node.y - 1),
            GetNode(node.x+1, node.y + 1),
            GetNode(node.x-1, node.y - 1),
            GetNode(node.x-1, node.y+1)

        };
    }

    

}
