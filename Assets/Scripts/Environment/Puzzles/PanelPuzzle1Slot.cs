using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelPuzzle1Slot : MonoBehaviour, IPointerClickHandler
{
    public Sprite CurrentSprite;
    public Sprite CorrectSprite;
    public Sprite defaultSprite;
    public Image image;
    PanelPuzzle1Manager manager;
    public Vector2Int Pos;

    public void Awake()
    {
        image = GetComponent<Image>();
        CurrentSprite = image.sprite;
        manager = transform.parent.GetComponent<PanelPuzzle1Manager>();
        defaultSprite = image.sprite;    

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (AreTheSlotsNeighbour(manager.nullSpot))
            {
                
                image.sprite = manager.nullSpot.image.sprite;
                
                manager.nullSpot.image.sprite = CurrentSprite;
                manager.nullSpot.CurrentSprite = CurrentSprite;
                
                CurrentSprite = image.sprite;
                manager.nullSpot = this;

                manager.CheckForPuzzleSolved();
            }
            
            
        }
    }

    bool AreTheSlotsNeighbour(PanelPuzzle1Slot other)
    {
        Debug.Log(other.Pos + ", " + Pos);
        Vector2Int Up = new Vector2Int(0, 1);
        Vector2Int Down = new Vector2Int(0, -1);
        Vector2Int Right = new Vector2Int(1, 0);
        Vector2Int Left = new Vector2Int(-1, 0);
        Vector2Int Difference = Pos - other.Pos;

        if (Difference != Up && Difference != Down && Difference != Right && Difference != Left) { return false; }
        return true;
    }
}
