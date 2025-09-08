using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float invMovementSpeed;
    public List<Node> CurrentPath;

    public IEnumerator Move(int NodesToIgnore)
    {
        foreach(var node in CurrentPath)
        {
            if (!gameObject.activeInHierarchy) { StopAllCoroutines(); }
            yield return MoveToPos(node.WorldPos);
            if (!node.Equals(CurrentPath[CurrentPath.Count - 1]))
            {
                node.isReserved = false;

            }
        }
        
    }

    IEnumerator MoveToPos(Vector3 targetPos)
    {
        float t = 0f;
        Vector3 initialPos = transform.position;
        float speed = invMovementSpeed*Vector2.Distance(initialPos, targetPos);
        while (t < speed)
        {
            transform.position = Vector2.Lerp(initialPos, targetPos, t / speed);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        yield break;
    }

    public void SetCurrentpathReservedToFalse()
    {
        foreach (Node node in CurrentPath)
        {
            node.isReserved = false;
            Game.Instance.pathFinder.tileManager.DebugTileMap.SetTile(node.Cell, null);
        }
    }
}
