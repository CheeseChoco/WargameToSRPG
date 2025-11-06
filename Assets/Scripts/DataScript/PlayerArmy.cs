using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerArmy
{
    public string armyName;
    public int totalCosts;

    public List<ArmyUnitEntry> units;

    public PlayerArmy()
    {
        armyName = "My First Army";
        totalCosts = 0;
        units = new List<ArmyUnitEntry>();
    }


}

[Serializable]
public class ArmyUnitEntry
{
    public string unitID;


    public ArmyUnitEntry(string id)
    {
        this.unitID = id;
    }
}
