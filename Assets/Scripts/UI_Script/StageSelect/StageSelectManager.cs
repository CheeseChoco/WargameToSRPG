using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
    private int selectedStageNum = -1;
    public Button gameStartButton;

    public GameObject ArmySelectMenu;
    public GameObject 
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
}
