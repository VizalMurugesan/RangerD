using UnityEngine;

public class LayerTrigger : MonoBehaviour
{
    BoxCollider2D FromLayerTopWallCollider;
    BoxCollider2D ToLayerTopWallCollider;

    [SerializeField] private GameObject FromLayerWall;
    [SerializeField] private GameObject ToLayerWall;

    [SerializeField] private Game.Layers FromLayer;
    [SerializeField] private Game.Layers ToLayer;

    public void Start()
    {
        FromLayerTopWallCollider = FromLayerWall.GetComponent<BoxCollider2D>();
        ToLayerTopWallCollider = ToLayerWall.GetComponent<BoxCollider2D>();
        ToLayerTopWallCollider.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Game.Instance.player.PlayerLayer = ToLayer;
        ToLayerTopWallCollider.enabled = true;
        FromLayerTopWallCollider.enabled = false;
    }



}
