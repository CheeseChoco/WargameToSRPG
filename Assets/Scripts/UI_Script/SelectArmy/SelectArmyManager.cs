
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class SelectArmyManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform armyScrollContent; 
    public Transform unitScrollContent; 
    public GameObject armyButtonPrefab; 
    public GameObject unitInfoPrefab;  

    public Button editButton;
    public Button deleteButton;
    public Button backButton;
    public Button newButton;


    [Header("Unit Database")]
    // (중요) 유닛 ID로 UnitSO를 찾기 위한 데이터베이스가 필요
    // 1. Resources 폴더를 사용하거나
    // 2. 모든 UnitSO를 들고있는 ScriptableObject를 만들 수 있음
    // 여기서는 간단히 Inspector에 UnitSO 리스트를 연결한다고 가정
    public List<UnitSO> allUnitSOs;
    private Dictionary<string, UnitSO> unitDatabase; // 빠른 탐색을 위한 딕셔너리

    private string selectedArmyName; // 현재 선택된 부대 이름
    private PlayerArmy selectedArmyData; // 현재 선택된 부대의 실제 데이터

    void Start()
    {
        // 1. UnitSO 리스트를 딕셔너리로 변환 (빠른 탐색용)
        unitDatabase = new Dictionary<string, UnitSO>();
        foreach (UnitSO so in allUnitSOs)
        {
            if (so != null && !unitDatabase.ContainsKey(so.unitID))
            {
                unitDatabase.Add(so.unitID, so);
            }
        }


        // 3. 버튼 리스너 연결
        backButton.onClick.AddListener(OnBack);
        editButton.onClick.AddListener(OnEdit);
        deleteButton.onClick.AddListener(OnDelete);
        newButton.onClick.AddListener(OnCreate);

        // 4. 씬 시작 시 부대 목록 로드 및 UI 초기화
        RefreshArmyList();
    }

    // (공용) 부대 목록 전체 새로고침
    void RefreshArmyList()
    {
        // 1. 선택 상태 초기화
        selectedArmyName = null;
        selectedArmyData = null;

        // 2. 버튼 비활성화 (선택된 게 없으므로)
        editButton.interactable = false;
        deleteButton.interactable = false;

        // 3. 양쪽 스크롤 뷰 클리어
        ClearScrollContent(armyScrollContent);
        ClearScrollContent(unitScrollContent);

        // 4. 부대 목록(왼쪽) 채우기
        //    (SaveLoadService는 static이므로 바로 접근)
        List<string> armyNames = SaveLoadService.GetAllSavedArmyNames();

        foreach (string name in armyNames)
        {
            GameObject buttonGO = Instantiate(armyButtonPrefab, armyScrollContent);
            buttonGO.GetComponentInChildren<TMP_Text>().text = name; // 프리팹 구조에 맞게 수정

            // (중요) 버튼에 클릭 이벤트 연결
            string tempName = name; // 람다식 클로저 문제 방지
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnArmySelected(tempName));
        }
    }

    // 부대 버튼(왼쪽)을 클릭했을 때
    void OnArmySelected(string armyName)
    {
        selectedArmyName = armyName;

        // 1. (SaveLoadService) 선택된 부대의 JSON 데이터 로드
        selectedArmyData = SaveLoadService.LoadArmy(armyName);

        if (selectedArmyData == null)
        {
            Debug.LogError("부대를 로드하는 데 실패했습니다!");
            RefreshArmyList(); // 목록 새로고침
            return;
        }

        // 2. 유닛 목록(오른쪽) 채우기
        PopulateUnitList(selectedArmyData);

        // 3. 편집/삭제 버튼 활성화
        editButton.interactable = true;
        deleteButton.interactable = true;

        // (UX 팁) 현재 선택된 버튼 시각적으로 표시 (색상 변경 등)
    }

    // 유닛 목록(오른쪽)을 채우는 함수
    void PopulateUnitList(PlayerArmy army)
    {
        ClearScrollContent(unitScrollContent);

        foreach (ArmyUnitEntry entry in army.units)
        {
            GameObject unitUI = Instantiate(unitInfoPrefab, unitScrollContent);

            // (구조적 조언)
            // PlayerArmy에는 unitID만 있습니다.
            // 유닛의 '이름'이나 '아이콘'을 표시하려면
            // 위에서 만든 unitDatabase에서 UnitSO 원본을 찾아와야 합니다.
            if (unitDatabase.TryGetValue(entry.unitID, out UnitSO unitData))
            {
                // 예: unitUI의 Text에 유닛 이름(unitData.unitName) 표시
                // 예: unitUI의 Image에 유닛 아이콘(unitData.unitIcon - UnitSO에 추가 필요) 표시
                unitUI.GetComponentInChildren<TMP_Text>().text = unitData.unitName;
            }
            else
            {
                unitUI.GetComponentInChildren<TMP_Text>().text = $"{entry.unitID} (원본 데이터 없음)";
            }
        }
    }
    void OnCreate()
    {
        // 1. GameDataHolder의 편집 데이터를 null로 설정
        //    (EditArmy 씬에 '새 부대' 모드임을 알림)
        GameDataHolder.ArmyToEdit = null;

        // 2. EditArmy 씬으로 이동
        SceneManager.LoadScene("EditArmy");
    }

    // 'Back' 버튼 클릭
    void OnBack()
    {
        SceneManager.LoadScene("MainMenu"); // MainMenu 씬 이름으로 변경
    }

    // 'Edit' 버튼 클릭
    void OnEdit()
    {
        if (selectedArmyData == null) return;

        // 1. GameDataHolder에 현재 선택된 부대 데이터 전달
        // (EditArmy 씬에서 이 데이터를 받아 편집을 시작)
        GameDataHolder.ArmyToEdit = selectedArmyData;

        // 2. EditArmy 씬으로 이동
        SceneManager.LoadScene("EditArmy");
    }

    // 'Delete' 버튼 클릭
    void OnDelete()
    {
        if (string.IsNullOrEmpty(selectedArmyName)) return;

        // (UX 팁) "정말 삭제하시겠습니까?" 팝업창을 띄우는 것이 좋음
        // 여기서는 바로 삭제

        SaveLoadService.DeleteArmy(selectedArmyName);

        // 삭제 후 목록 새로고침
        RefreshArmyList();
    }

    // 스크롤 뷰의 내용물을 지우는 헬퍼 함수
    void ClearScrollContent(Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}