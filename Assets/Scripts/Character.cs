using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float invMovementSpeed;
    public List<Node> CurrentPath;
    public Vector3Int ManhattenDirec;
    public Node CurrentNode;
    public Animator anim;
    public GameObject pivot;

    public List<SpriteRenderer> renderers;

    public virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    public IEnumerator Move(int NodesToIgnore)
    {

        CurrentNode = Game.Instance.pathFinder.GetNodeFromWorldPos(transform.position);
        int nodesToRemove = Mathf.Min(CurrentPath.Count, NodesToIgnore);
        for (int i = 0; i < nodesToRemove; i++)
        {
            CurrentPath[CurrentPath.Count - 1].isReserved = false;
            CurrentPath.RemoveAt(CurrentPath.Count - 1);
        }
        foreach (var node in CurrentPath)
        {
            ManhattenDirec = node.Cell-CurrentNode.Cell;
            HandleHorizontalDirection(ManhattenDirec);

            if (anim != null) 
            { 
                anim.SetFloat("DirectionX", ManhattenDirec.x);
                anim.SetFloat("DirectionY", ManhattenDirec.y);
                anim.SetBool("IsChasing", true); }
            if (!gameObject.activeInHierarchy) { StopAllCoroutines(); }
            yield return MoveToPos(node.WorldPos);
            
            CurrentNode.isReserved = false;
            CurrentNode = node;

            
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

    public void LookLeft()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(-1f* Mathf.Abs(scale.x), scale.y, scale.z);
    }

    public void LookRight()
    {
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
    }

    void HandleHorizontalDirection(Vector3 direction)
    {
        if (direction.x > 0)
        {
            LookRight();
        }
        else if(direction.x < 0)
        {
            LookLeft();
        }
    }

    public virtual void SetSortingOrder(int order, bool IsCharacterBelow)
    {
       
    }
}
