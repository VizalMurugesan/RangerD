using UnityEngine;

public class Props : Objects
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        FrontLayer = "Structures after player";
        BackLayer = "Structures before player";
    }

    
}
