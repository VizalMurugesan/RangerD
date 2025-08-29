using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Unity.VisualScripting;
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //Item Details
    [NonSerialized] public string itemName;
    [NonSerialized] public int quantity;
    [NonSerialized] private Sprite itemSprite;
    [NonSerialized] public bool IsFull = false;
    [NonSerialized] public String DescriptionText;

    //Item Slot Details

    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    [SerializeField] public GameObject SelectedShader;

    public Action Use;

    private InventoryManager inventoryManager;

    public bool IsSlotSelected = false;

    private void Start()
    {
        inventoryManager = Game.Instance.inventoryManager;
    }
    public void AddItem(string name, int quantity, Sprite itemSprite, string Description, Action Use)
    {
        itemName = name;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        DescriptionText = Description;
        IsFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        quantityText.gameObject.SetActive(true);
        itemImage.sprite = itemSprite;

        this.Use = Use;

        
    }
    public void AddQuantity(int extraquantity)
    {
        quantity += extraquantity;
        quantityText.text = quantity.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void OnRightClick()
    {
        if(IsFull)
        {
            inventoryManager.SlotInterface.transform.position = Input.mousePosition;
            inventoryManager.SlotInterface.SetActive(true);
        }
       
    }

    private void OnLeftClick()
    {
        if (inventoryManager.SelectedItemSlot != null)
        {
            
            if (inventoryManager.SelectedItemSlot.Equals(this))
            {
                inventoryManager.DeselectAllSlots();
                return;
            }
            
        }
    
        inventoryManager.DeselectAllSlots();

        if (IsFull) 
        { 
            inventoryManager.SetSelectedSlotDetails(itemName, DescriptionText, itemSprite);
            
            
        }
        inventoryManager.SelectedItemSlot = this;

        SelectedShader.SetActive(true);
        IsSlotSelected = true;
    }

    public void DiscardItemInSlot()
    {
        itemName = null;
        quantity = 0;
        itemSprite = inventoryManager.DefaultSelectedSlotSprite;
        DescriptionText = "";
        IsFull = false;

        quantityText.text = quantity.ToString();
        quantityText.enabled = false;
        quantityText.gameObject.SetActive(false);
        itemImage.sprite = itemSprite;

        Use = null;
    }
    
}
