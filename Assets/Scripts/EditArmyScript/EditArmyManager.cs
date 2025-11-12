using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace CheeseChoco.WargameToSRPG.UI
{
    public class EditArmyManager : MonoBehaviour
    {
        public static EditArmyManager Instance { get; private set; }

        [Header("부대 설정")]
        public int maxCost = 500;
        public int currentCost = 0;

        private List<UnitSO> partyMembers = new List<UnitSO>();


        public Button cancelButton;
        public Button saveButton;
        public Button P500Button;
        public Button P1000Button;
        public Button P2000Button;
        public TMP_InputField armyNameInput;

        public Action OnPartyUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }

        }

        private void Start()
        {
            if (GameDataHolder.ArmyToEdit == null) GameDataHolder.ArmyToEdit = new PlayerArmy();
            PlayerArmy army = GameDataHolder.ArmyToEdit;

            foreach(var unit in army.units)
            {
                Debug.Log(unit.unitID);
                if (UnitDatabaseManager.Instance.unitDatabase.TryGetValue(unit.unitID, out UnitSO unitData))
                {
                    AddUnit(unitData);
                }
            }
            OnPartyUpdated?.Invoke();




            armyNameInput.text = GameDataHolder.ArmyToEdit.armyName;
            saveButton.onClick.AddListener(OnClickSaveButton);
            cancelButton.onClick.AddListener(OnClickCancelButton);
            P500Button.onClick.AddListener(() => OnClickPointButton(500));
            P1000Button.onClick.AddListener(() => OnClickPointButton(1000));
            P2000Button.onClick.AddListener(() => OnClickPointButton(2000));
        }

        public bool AddUnit(UnitSO unit)
        { 
            if (currentCost + unit.pointCost > maxCost)
            {
                Debug.LogWarning("부대가 꽉 찼습니다. 유닛을 추가할 수 없습니다.");
                return false;
            }
            currentCost += unit.pointCost;


            partyMembers.Add(unit);
            OnPartyUpdated?.Invoke(); 
            return true;
        }

        public void RemoveUnit(UnitSO unit)
        {
            if (partyMembers.Remove(unit))
            {
                currentCost -= unit.pointCost;
                OnPartyUpdated?.Invoke(); 
            }
        }

        public List<UnitSO> GetPartyMembers()
        {
            return new List<UnitSO>(partyMembers);
        }

        public void OnClickSaveButton()
        {
            foreach(var name in SaveLoadService.GetAllSavedArmyNames())
            {
                if (name == armyNameInput.text)
                {
                    Debug.Log("이미 존재하는 이름입니다.");
                    return;
                }
            }
            PlayerArmy saveArmy = new PlayerArmy();
            
            if(armyNameInput.text != null)
            {
                saveArmy.armyName = armyNameInput.text;
            }
            List<string> armyList = new List<string>(); 
            foreach(var unit in partyMembers)
            {
                saveArmy.units.Add(new ArmyUnitEntry(unit.unitID));
            }
            saveArmy.totalCosts = currentCost;


            SaveLoadService.SaveArmy(saveArmy);
            SceneManager.LoadScene("SelectArmy");
        }

        public void OnClickCancelButton()
        {
            SceneManager.LoadScene("SelectArmy");
        }

        public void OnClickPointButton(int point)
        {
            if (point < currentCost) {
                Debug.Log("현재 코스트가 초과되었습니다.");
                return; }
            maxCost = point;
            
            OnPartyUpdated?.Invoke();
        }


    }
}
