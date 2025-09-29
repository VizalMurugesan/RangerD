using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public List<Enemy> enemies;
    public float DetectionRange = 0f;
    
    public bool GroupAggro = false;
    public float intervalbetweenstates = 0.3f;

    void Update()
    {
        if(Vector2.Distance(transform.position, (Vector2)Game.Instance.player.GetPlayerPosition()) < DetectionRange && !GroupAggro)
        {
            SetAggro();
            StartCoroutine(StartDecidingStates());
            //StartCoroutine(StartShowingReserved());
        }

        


    }

    void ManageSortingLayer(Enemy enemy)
    {
        if(enemy.pivot.transform.position.y> Game.Instance.player.PlayerPivot.transform.position.y)
        {
            enemy.spriteRenderer.sortingOrder = -10;
            
        }
        else
        {
            enemy.spriteRenderer.sortingOrder = 10;
        }
    }
    IEnumerator StartShowingReserved()
    {
        while (true)
        {
            Game.Instance.pathFinder.tileManager.DebugTileMap.ClearAllTiles();
            ShowReservedTiles();
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("showing");
        }
        
    }
    void ShowReservedTiles()
    {
        Vector3Int midNode = Game.Instance.pathFinder.GetNodeFromWorldPos(transform.position).Cell;
        //Debug.Log(midNode);
        for (int i = midNode.x-60; i< midNode.x + 60; i++)
        {
            for (int j = midNode.y - 60; j < midNode.y + 60; j++)
            {
                if (Game.Instance.pathFinder.grid.GetNode(i,j).isReserved)
                {
                    //Debug.Log("gaga");
                    Game.Instance.pathFinder.tileManager.DebugTileMap.SetTile(
                        new Vector3Int(i, j, 0), Game.Instance.pathFinder.tileManager.DebugSprite);
                }
            }
        }
    }

    public void UnreserveTiles()
    {
        Vector3Int midNode = Game.Instance.pathFinder.GetNodeFromWorldPos(transform.position).Cell;
        for (int i = midNode.x - 60; i < midNode.x + 60; i++)
        {
            for (int j = midNode.y - 60; j < midNode.y + 60; j++)
            {
                if (Game.Instance.pathFinder.grid.GetNode(i, j).isReserved)
                {
                    //Debug.Log("gaga");
                    Game.Instance.pathFinder.tileManager.DebugTileMap.SetTile(
                        new Vector3Int(i, j, 0), Game.Instance.pathFinder.tileManager.DebugSprite);
                }
            }
        }
    }

    void SetAggro()
    {
        GroupAggro = true;
        Game.Instance.characterLayerManager.CharactersInRange.AddRange(enemies);
        foreach(Enemy enemy in enemies)
        {
            enemy.SetAggroTrue();
            
        }
    }

    IEnumerator StartDecidingStates()
    {
        while (true)
        {
            //Debug.Log("deciding states");
            yield return new WaitForSeconds(intervalbetweenstates);
            for(int i = 0;  i < enemies.Count; i++)
            {
                if (!enemies[i].gameObject.activeInHierarchy) { enemies.RemoveAt(i); continue; }
                EnemyState DecidedState = enemies[i].DecideState();
                if (DecidedState != null) { DecidedState.StateActionInvoke(); }
                
            }
            if(enemies.Count == 0) { GroupAggro = false;  break; }
            
        }

        //gameObject.SetActive(false);
        this.enabled = false;
    }
}
