using System.Collections;
using UnityEngine;
using System;
using static UnityEditor.PlayerSettings;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 5f;  // Adjust this for more/less smoothing
    public Vector3 offset;
    Vector3 playerPos;
    public float Delay;
    public float duration;
    Coroutine moveCoroutine;

    void LateUpdate()
    {
        
        if (!IsCameraOnPlayer() && moveCoroutine==null)
        {
            moveCoroutine = StartCoroutine(StartMovingTowardsPlayer());
        }
    }

    IEnumerator StartMovingTowardsPlayer()
    {
        yield return new WaitForSeconds(Delay);
        float t = 0;
        Vector2 StartPos = transform.position;
        Vector2 newPos = Vector2.zero;
        while (!IsCameraOnPlayer() || Game.Instance.player.IsPlayerOnMove())
        {
            
            Vector2 targetPos = Game.Instance.player.transform.position;
            
            newPos = Vector2.Lerp(StartPos, targetPos, t / duration);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            t += Time.deltaTime;
            yield return null;
        }
        moveCoroutine = null;
    }

    bool IsCameraOnPlayer()
    {
        Vector2 pos = transform.position;
        return Vector2.Distance(pos,Game.Instance.player.transform.position)<0.001f;
    }
}
