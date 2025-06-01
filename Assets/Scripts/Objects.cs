using UnityEngine;

public class Objects : MonoBehaviour
{
    GameObject Player ;
    public SpriteRenderer Rendr;
    private float YOffset = -2.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rendr = GetComponent<SpriteRenderer>();
        if(Game.Instance.player != null)
        {
            Player = Game.Instance.player.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y>= Player.transform.position.y - YOffset)
        {
            BringFront();
        }
        else
        {
            BringBack();
        }
    }

    private void BringFront()
    {
        Rendr.sortingLayerName = "Structures before player";
    }

    private void BringBack()
    {
        Rendr.sortingLayerName = "Structures after player";
    }
}
