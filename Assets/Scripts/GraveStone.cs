using UnityEngine;

public class GraveStone : Objects
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    BoxCollider2D box;
    public override void Start()
    {
        base.Start();
        FrontLayer = "Structures after player";
        BackLayer = "Structures before player";
        box = GetComponent<BoxCollider2D>();

    }

    public override void BringBack()
    {
        base.BringBack();
        box.enabled = false;
    }

    public override void BringFront()
    {
        base.BringFront();
        box.enabled = true;
    }
}
