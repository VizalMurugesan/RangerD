using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] public string ItemName;
    [SerializeField] public int Quantity;
    [SerializeField] public Sprite sprite;
    [SerializeField] public string Description;
    [SerializeField] public int Value;

    [SerializeField] InventoryManager inventoryManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(inventoryManager == null)
        {
            inventoryManager = Game.Instance.inventoryManager;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerPivot"))
        {
            inventoryManager.AddItem(ItemName, Quantity, sprite, Description, OnUse);
            gameObject.SetActive(false);
        }
    }

    public virtual void OnUse()
    {
        Debug.Log(ItemName+"used");
    }
}
