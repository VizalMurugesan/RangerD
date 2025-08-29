using UnityEngine;

public class LayerTrigger : MonoBehaviour
{
    

    [SerializeField] private Game.Layers FromLayer;
    [SerializeField] private Game.Layers ToLayer;

    public void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            Game.Instance.player.ChangePlayerLayer(ToLayer);
            
        }
        
       
    }



}
