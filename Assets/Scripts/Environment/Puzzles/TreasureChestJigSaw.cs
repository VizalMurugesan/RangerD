using UnityEngine;
using UnityEngine.UI;
public class TreasureChestJigSaw : TreasureChest
{
    public bool Solved = false;
    public GameObject puzzlePanel;

    public override bool RequirementsMet()
    {
        if(!puzzlePanel.activeInHierarchy && !Solved)
        {
            puzzlePanel.GetComponent<PanelPuzzle1Manager>().OpenPanel();
        }
        return Solved;
    }

    public void SetSolvedToTrue()
    {
        Solved = true;
    }
}
