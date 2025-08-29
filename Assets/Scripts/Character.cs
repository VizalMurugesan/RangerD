using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float invMovementSpeed;
    //public List<Node> CurrentPath;
    public IEnumerator Move(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            yield return MoveToPos(node.WorldPos);
        }
    }

    public IEnumerator Move(List<Node> nodes, int NodesToIgnore)
    {
        for(int i = 0; i < nodes.Count; i++)
        {
            if(Mathf.Abs(nodes.Count - i) <= NodesToIgnore)
            {
                yield break;
            }
            else
            {
                yield return MoveToPos(nodes[i].WorldPos);
            }
        }
    }

    IEnumerator MoveToPos(Vector3 targetPos)
    {
        float t = 0f;
        Vector3 initialPos = transform.position;
        while (t < invMovementSpeed)
        {
            transform.position = Vector2.Lerp(initialPos, targetPos, t / invMovementSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        yield break;
    }
}
