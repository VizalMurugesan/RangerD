using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 5f;  // Adjust this for more/less smoothing
    public Vector3 offset;
    Vector3 playerPos;

    void LateUpdate()
    {
        if (Game.Instance.player != null)
        {
            //Vector3 targetPosition = Game.Instance.player.transform.position;
            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y, transform.position.z), smoothSpeed * Time.deltaTime);
            //transform.position = smoothedPosition;
            playerPos = Game.Instance.player.gameObject.transform.position;
            transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
        }
    }
}
