using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PanelPuzzle1Manager : MonoBehaviour
{
    public PanelPuzzle1Slot nullSpot;
    public PanelPuzzle1Slot defaultnullSpot;
    public GameObject CancelButton;
    public GameObject ResetButton;
    public TreasureChestJigSaw jigSaw;

    public List<PanelPuzzle1Slot> slots;

    public void Awake()
    {
        defaultnullSpot = nullSpot;
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        CancelButton.SetActive(false);
        ResetButton.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        CancelButton.SetActive(true);
        ResetButton.SetActive(true);
        Time.timeScale = 0f;
    }

    public bool IsPuzzleSloved()
    {
        foreach (var slot in slots)
        {
            if (slot.CurrentSprite != slot.CorrectSprite) { return false; }

        }
        return true;

    }

    public void CheckForPuzzleSolved()
    {
        if (IsPuzzleSloved())
        {
            jigSaw.SetSolvedToTrue();
            ClosePanel();
        }
    }

    public void ResetPuzzle()
    {
        foreach(var slot in slots)
        {
            slot.image.sprite = slot.defaultSprite;
            slot.CurrentSprite = slot.defaultSprite;
        }
        nullSpot = defaultnullSpot;
    }
}
