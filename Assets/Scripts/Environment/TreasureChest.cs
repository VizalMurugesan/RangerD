using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public float treasureQuantity;
    BoxCollider box;
    InventoryManager inventory;
    bool opened = false;
    public Sprite openedSprite;

    public void Start()
    {
        box = GetComponent<BoxCollider>();
        inventory = Game.Instance.inventoryManager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.CompareTag("Player")|| collision.CompareTag("PlayerPivot")) &&!opened)
        {
            SpawnTreasure();
            opened = true;
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }

    void SpawnTreasure()
    {
        for(int i = 0; i < treasureQuantity; i++)
        {
            int rand = Random.Range(0, Game.Instance.items.Length);
            Item itemToSpawn = Game.Instance.items[rand];
            inventory.AddItem(itemToSpawn.name, itemToSpawn.Quantity, itemToSpawn.sprite, itemToSpawn.Description, itemToSpawn.OnUse);
        }
    }
}
