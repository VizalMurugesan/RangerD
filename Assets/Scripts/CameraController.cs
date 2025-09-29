using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 5f;  // Adjust this for more/less smoothing
    public Vector3 offset;
    Vector3 playerPos;
    public float Delay;
    public float duration;
    Coroutine moveCoroutine;
    bool shaking = false;
    public float shakemag;

    void LateUpdate()
    {
        
        if (!IsCameraOnPlayer() && moveCoroutine==null &&!shaking)
        {
            moveCoroutine = StartCoroutine(StartMovingTowardsPlayer());
        }
        //playerPos = Game.Instance.player.transform.position;
        //transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
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

    public IEnumerator Shake(float duration, float magnitude)
    {
        shaking = true;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
       
        moveCoroutine = null;
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakemag;
            float offsetY = Random.Range(-1f, 1f) * shakemag;

            transform.localPosition = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos; // Reset to original position
        shaking = false;
        
    }
}
