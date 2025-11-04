using System;
using System.Collections.Generic;

[System.Serializable]
public class ArmySaveData
{
    public string armyID; // 부대 ID (PlayerArmy.armyName 사용)
    public List<string> unitIDs; // 유닛 SO의 이름(string) 목록
    public List<string> armyTraits; // 부대 특성
}