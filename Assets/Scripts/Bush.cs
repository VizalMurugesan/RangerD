using UnityEngine;

public class Bush : Objects
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        FrontLayer = "Vegetation after player";
        BackLayer = "Vegetation before player";
    }

    
    
}
