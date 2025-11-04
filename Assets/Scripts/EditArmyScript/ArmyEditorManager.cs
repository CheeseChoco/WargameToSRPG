using System.Collections.Generic;
using TMPro; // TextMeshPro UI 사용
using UnityEngine;
using System.IO;

public class ArmyEditorManager : MonoBehaviour
{
    [Header("Game Data")]
    // [1] 편집 가능한 모든 유닛의 원본(SO) 리스트
    public List<UnitSO> allAvailableUnits;

    [Header("Point System")]
    public int maxArmyPoints = 2000; // 부대 최대 포인트

    [Header("UI References")]
    public Transform availableUnitsListParent; // 사용 가능한 유닛 목록이 표시될 UI 부모
    public Transform currentArmyListParent;   // 현재 부대에 포함된 유닛 목록이 표시될 UI 부모
    public TextMeshProUGUI pointsText;         // "현재 포인트: 1500 / 2000"

    // (UI 버튼 프리팹들)
    // public GameObject unitSelectButtonPrefab;
    // public GameObject troopEntryPrefab;

    // [2] 현재 편집 중인 부대 정보 (메모리)
    private PlayerArmy currentArmy;

    void Start()
    {
        currentArmy = GameDataHolder.ArmyToEdit;
        if (currentArmy == null ) currentArmy = new PlayerArmy();

        // (여기서 allAvailableUnits 리스트를 기반으로 UI 버튼들 생성)

        UpdateUI();
    }

    // [4] 유닛을 부대에 추가 (UI 버튼에서 호출)
    public void AddUnit(UnitSO unitToAdd)
    {
        int newTotalPoints = currentArmy.totalCosts + unitToAdd.pointCost;

        // 포인트 제한 체크
        if (newTotalPoints > maxArmyPoints)
        {
            Debug.LogWarning("포인트 초과! 유닛을 추가할 수 없습니다.");
            return;
        }

        // 데이터 업데이트
        currentArmy.totalCosts = newTotalPoints;
        currentArmy.units.Add(new ArmyUnitEntry(unitToAdd.unitID));

        // UI 갱신
        UpdateUI();
    }

    // [5] 부대에서 유닛 제거 (UI 버튼에서 호출)
    public void RemoveUnit(ArmyUnitEntry unitToRemove)
    {
        // (unitToRemove.unitID를 사용해 UnitDefinitionSO를 찾아서 pointCost를 가져와야 함)
        // UnitDefinitionSO unitSO = FindUnitSO_ByID(unitToRemove.unitID);
        // currentArmy.totalPointsUsed -= unitSO.pointCost;
        // currentArmy.units.Remove(unitToRemove);

        UpdateUI();
    }

    // [6] UI 갱신 (유닛 목록, 포인트 텍스트 등)
    private void UpdateUI()
    {
        // (currentArmyListParent의 자식 UI들을 갱신)

        pointsText.text = $"현재 포인트: {currentArmy.totalCosts} / {maxArmyPoints}";
    }

    // [7] 저장 버튼 (UI 버튼에서 호출)
    public void OnSaveButtonClicked()
    {
        SaveLoadService.SaveArmy(currentArmy);
    }
}