using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    public bool InventoryActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

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
            Time.timeScale = 1f;
        }
    }
}
