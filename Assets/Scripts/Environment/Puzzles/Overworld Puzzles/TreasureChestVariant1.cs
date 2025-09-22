using UnityEngine;
using System.Collections.Generic;

public class TreasureChestVariant1 : TreasureChest
{
    public List<EndPointsTreasureChest> endPoints;

    public override bool RequirementsMet()
    {
        foreach(var point in  endPoints)
        {
            if (!point.fullfilled)
            {
                return false;
            }
        }
        return true;
    }

}
