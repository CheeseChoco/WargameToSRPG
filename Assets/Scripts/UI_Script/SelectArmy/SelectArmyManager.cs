
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


    private string selectedArmyName;
    private PlayerArmy selectedArmyData;

    void Start()
    {


        backButton.onClick.AddListener(OnBack);
        editButton.onClick.AddListener(OnEdit);
        deleteButton.onClick.AddListener(OnDelete);
        newButton.onClick.AddListener(OnCreate);

        RefreshArmyList();
    }

    void RefreshArmyList()
    {
        selectedArmyName = null;
        selectedArmyData = null;

        editButton.interactable = false;
        deleteButton.interactable = false;

        ClearScrollContent(armyScrollContent);
        ClearScrollContent(unitScrollContent);

        List<string> armyNames = SaveLoadService.GetAllSavedArmyNames();

        foreach (string name in armyNames)
        {
            GameObject buttonGO = Instantiate(armyButtonPrefab, armyScrollContent);
            buttonGO.GetComponentInChildren<TMP_Text>().text = name; 

            string tempName = name;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnArmySelected(tempName));
        }
    }

    void OnArmySelected(string armyName)
    {
        selectedArmyName = armyName;

        selectedArmyData = SaveLoadService.LoadArmy(armyName);

        if (selectedArmyData == null)
        {
            Debug.LogError("부대를 로드하는 데 실패했습니다!");
            RefreshArmyList();
            return;
        }

        PopulateUnitList(selectedArmyData);

        editButton.interactable = true;
        deleteButton.interactable = true;

    }

    void PopulateUnitList(PlayerArmy army)
    {
        ClearScrollContent(unitScrollContent);

        foreach (ArmyUnitEntry entry in army.units)
        {
            GameObject unitUI = Instantiate(unitInfoPrefab, unitScrollContent);

            if (UnitDatabaseManager.Instance.unitDatabase.TryGetValue(entry.unitID, out UnitSO unitData))
            {
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
        GameDataHolder.ArmyToEdit = null;

        SceneManager.LoadScene("EditArmy");
    }

    void OnBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnEdit()
    {
        if (selectedArmyData == null) return;

        GameDataHolder.ArmyToEdit = selectedArmyData;

        SceneManager.LoadScene("EditArmy");
    }

    void OnDelete()
    {
        if (string.IsNullOrEmpty(selectedArmyName)) return;


        SaveLoadService.DeleteArmy(selectedArmyName);

        RefreshArmyList();
    }

    void ClearScrollContent(Transform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}