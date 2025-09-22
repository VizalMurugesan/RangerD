using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public enum TreasureChestQuality { Normal, Medium , Large, Epic, Legend}
    [SerializeField] TreasureChestQuality quality;
    //BoxCollider box;
    InventoryManager inventory;
    public enum TreasureState { YetToInteract, Interacting, RequirementsMet, Opened}
    TreasureState state = TreasureState.YetToInteract;
    public Sprite openedSprite;

    public void Start()
    {
        //box = GetComponent<BoxCollider>();
        inventory = Game.Instance.inventoryManager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.CompareTag("Player")|| collision.CompareTag("PlayerPivot")) 
            &&!state.Equals(TreasureState.Opened))
        {
            if (RequirementsMet())
            {
                SpawnTreasure();
                state = TreasureState.Opened;
                GetComponent<SpriteRenderer>().sprite = openedSprite;
            }
            
        }
    }

    int TypeToValue()
    {
        int val;
        switch (quality)
        {
            case TreasureChestQuality.Normal:
                val = 100;
                break;
            case TreasureChestQuality.Medium:
                val = 200; 
                break;
            case TreasureChestQuality.Large:
                val = 500;
                break;
            case TreasureChestQuality.Epic: 
                val = 750;
                break;
            case TreasureChestQuality.Legend:
                val = 1000;
                break;
            default:
                val = 0; 
                break;


        }
        return val;
    }

    void SpawnTreasure()
    {
        int treasureQuantity = TypeToValue();
        int currentVal = 0;
        List<Item> itemsToSpawn = new List<Item>();
        while(currentVal<treasureQuantity)
        {
            int rand = Random.Range(0, Game.Instance.items.Length);
            Item itemToSpawn = Game.Instance.items[rand];
            if(itemToSpawn.Value+currentVal<=treasureQuantity)
            {
                itemsToSpawn.Add(itemToSpawn);
                currentVal += itemToSpawn.Value;
            }
            
        }

        foreach(Item item in itemsToSpawn)
        {
            inventory.AddItem(item.ItemName, item.Quantity, item.sprite, item.Description, item.OnUse);
        }
    }

    public virtual bool RequirementsMet()
    {
        return true;
    }
}
