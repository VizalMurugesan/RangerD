using UnityEngine;

public class CameraController : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.player != null)
        {
            Vector3 newPosition = Game.Instance.player.transform.position;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z) ;
        }
        
    }
}
