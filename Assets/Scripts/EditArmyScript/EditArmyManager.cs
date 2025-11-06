using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 플레이어의 현재 부대(파티) 구성을 관리하는 싱글톤 매니저입니다.
    /// UI가 아닌 데이터만 관리합니다.
    /// </summary>
    public class EditArmyManager : MonoBehaviour
    {
        public static EditArmyManager Instance { get; private set; }

        [Header("부대 설정")]
        public int maxCost = 500; // 부대 최대 인원
        public int currentCost = 0;

        private List<UnitSO> partyMembers = new List<UnitSO>();


        public Button cancelButton;
        public Button saveButton;
        public TMP_InputField armyNameInput;

        /// <summary>
        /// 부대 목록이 변경될 때마다 UI 스크립트에게 알리기 위한 이벤트입니다.
        /// </summary>
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
            Debug.Log(GameDataHolder.ArmyToEdit.armyName);

            armyNameInput.text = GameDataHolder.ArmyToEdit.armyName;
            saveButton.onClick.AddListener(OnClickSaveButton);
            cancelButton.onClick.AddListener(OnClickCancelButton);
        }

        /// <summary>
        /// 부대에 유닛을 추가하려고 시도합니다.
        /// </summary>
        /// <returns>추가 성공 여부</returns>
        public bool AddUnit(UnitSO unit)
        { 
            // 부대가 꽉 찼는지 확인
            if (currentCost + unit.pointCost > maxCost)
            {
                Debug.LogWarning("부대가 꽉 찼습니다. 유닛을 추가할 수 없습니다.");
                // TODO: "부대가 꽉 찼습니다" UI 피드백
                return false;
            }
            currentCost += unit.pointCost;


            partyMembers.Add(unit);
            OnPartyUpdated?.Invoke(); // 부대가 변경되었음을 모두에게 알림
            return true;
        }

        /// <summary>
        /// 부대에서 유닛을 제거합니다.
        /// </summary>
        public void RemoveUnit(UnitSO unit)
        {
            if (partyMembers.Remove(unit))
            {
                currentCost -= unit.pointCost;
                OnPartyUpdated?.Invoke(); // 부대가 변경되었음을 모두에게 알림
            }
        }

        /// <summary>
        /// 현재 부대원 목록을 반환합니다.
        /// </summary>
        public List<UnitSO> GetPartyMembers()
        {
            return new List<UnitSO>(partyMembers); // 복사본 반환
        }

        public void OnClickSaveButton()
        {
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


    }
}
