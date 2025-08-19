using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private string ItemName;
    [SerializeField] private int Quantity;
    [SerializeField] private Sprite sprite;
    [SerializeField] public string Description;

    [SerializeField] InventoryManager inventoryManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(inventoryManager == null)
        {
            inventoryManager = Game.Instance.inventoryManager;
        }

        Description = "Restores 50 health on use.";
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
