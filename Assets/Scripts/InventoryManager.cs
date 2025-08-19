using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    public bool InventoryActive;

    public ItemSlot[] ItemSlots;

    public TMP_Text SelectedSlotName;
    public TMP_Text SelectedSlotDescription;
    //public GameObject SelectedSlotImageGameObject;
    public Image SelectedSlotImage;

    public GameObject SlotInterface;



    public Sprite DefaultSelectedSlotSprite;
    public ItemSlot SelectedItemSlot;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && !InventoryActive)
        {
            InventoryMenu.SetActive(true);
            InventoryActive = true;
            Time.timeScale = 0f;
        }
        else if (Input.GetButtonDown("Inventory") && InventoryActive)
        {
            InventoryMenu.SetActive(false);
            InventoryActive = false;
            SetDefaultDescription();
            Time.timeScale = 1f;
        }


    }

    public void AddItem(string  name, int quantity, Sprite sprite, string Description, Action Use)
    {
        foreach (ItemSlot slot in ItemSlots)
        {
            if(slot.itemName == name)
            {
                slot.AddQuantity(quantity);
                break;
            }
            else if (!slot.IsFull)
            {
                slot.AddItem(name, quantity, sprite, Description, Use);
                break;
            }
        }
    }

    public void DeselectAllSlots()
    {
        foreach (ItemSlot slot in ItemSlots)
        {
            if (slot.IsSlotSelected)
            {
                slot.IsSlotSelected = false;
                slot.SelectedShader.SetActive(false);
            }
        }
        if (SlotInterface.activeInHierarchy)
        {
            SlotInterface.SetActive(false); 
        }
        SetDefaultDescription();
        SelectedItemSlot = null;
    }


    //for description panel
    public void SetSelectedSlotDetails(string name, string Description, Sprite sprite)
    {
        SelectedSlotName.text = name;
        SelectedSlotDescription.text = Description;
        SelectedSlotImage.sprite = sprite;

    }

    void SetDefaultDescription()
    {
        SelectedSlotName.text = "";
        SelectedSlotDescription.text = "NO ITEM SELECTED" ;
        SelectedSlotImage.sprite = DefaultSelectedSlotSprite;
    }

    public void UseSelectedItem()
    {
        SelectedItemSlot.Use.Invoke();
    }

    public void DiscardSelectedItem()
    {
        SelectedItemSlot.DiscardItemInSlot();
    }
}
