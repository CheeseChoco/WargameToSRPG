using System;
using System.Collections.Generic;

// 이 클래스 자체가 하나의 JSON 파일이 됩니다.
[Serializable]
public class PlayerArmy
{
    public string armyName;
    public int totalCosts; // UI 표시 및 검증용

    // 이 리스트에 플레이어가 선택한 유닛들이 담깁니다.
    public List<ArmyUnitEntry> units;

    public PlayerArmy()
    {
        // 저장된 파일이 없을 때를 대비한 기본 생성자
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