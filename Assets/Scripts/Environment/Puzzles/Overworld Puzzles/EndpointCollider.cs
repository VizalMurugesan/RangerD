using UnityEngine;

public class EndpointCollider : MonoBehaviour
{
    public EndPointsTreasureChest point;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("TreasureChestStuff")&& point.fullfilled)
        {
            point.SetFullFillToFalse();
        }
    }
}
