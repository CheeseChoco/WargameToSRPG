using System;
using System.Collections.Generic;

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

// 부대에 포함된 개별 유닛의 정보
[Serializable]
public class ArmyUnitEntry
{
    // UnitDefinitionSO의 unitID와 매칭되는 키
    public string unitID;


    public ArmyUnitEntry(string id)
    {
        this.unitID = id;
    }
}