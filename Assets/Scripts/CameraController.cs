using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothSpeed = 5f;  // Adjust this for more/less smoothing

    void Update()
    {
        if (Game.Instance.player != null)
        {
            Vector3 targetPosition = Game.Instance.player.transform.position;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, targetPosition.y, transform.position.z), smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
