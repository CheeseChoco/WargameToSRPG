using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class StageSelectManager : MonoBehaviour
{
    private int selectedStageNum = -1;
    public Button gameStartButton;

    public GameObject armyButtonPrefab;

    public Transform armyScrollContent;

    private PlayerArmy selectedArmy;
    public static StageSelectManager Instance {  get; private set; }

    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        if(gameStartButton != null)
        {
            gameStartButton.interactable = false;
        }

        SettingArmy();

    }
    public void OnClickStartButton()
    {
        if(selectedStageNum == -1)
        {
            Debug.Log("stage를 선택하지 않았습니다.");
            return;
        }
        else
        {
            GameDataHolder.ArmyToEdit = selectedArmy;
            GameDataHolder.SelectedStageNum = selectedStageNum;
            SceneManager.LoadScene("gamebase");
        }
    }

    public void OnClickStageButton(GameObject selectedButton, int stageNum)
    {
        EventSystem.current.SetSelectedGameObject(selectedButton);
        gameStartButton.interactable = true;
        selectedStageNum = stageNum;
    }

    private void SettingArmy()
    {
        List<string> armyNames = SaveLoadService.GetAllSavedArmyNames();

        foreach (string name in armyNames)
        {
            GameObject buttonGO = Instantiate(armyButtonPrefab, armyScrollContent);
            buttonGO.GetComponentInChildren<TMP_Text>().text = name; // 프리팹 구조에 맞게 수정

            string tempName = name; // 람다식 클로저 문제 방지
            buttonGO.GetComponent<Button>().onClick.AddListener(() => OnArmySelected(tempName));
        }
    }

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnArmySelected(string armyName)
    {
        Debug.Log($"{armyName}부대 선택");
        // 1. (SaveLoadService) 선택된 부대의 JSON 데이터 로드
        selectedArmy = SaveLoadService.LoadArmy(armyName);
        Debug.Log($"selectedArmy = {selectedArmy.armyName}");

        if (selectedArmy == null)
        {
            Debug.LogError("부대를 로드하는 데 실패했습니다!");
            return;
        }
    }
}
