using UnityEngine;
using System.Collections.Generic; 

namespace CheeseChoco.WargameToSRPG.UI
{
    public class UI_RecruitListManager : MonoBehaviour
    {
        [Header("프리팹 및 부모 객체 연결")]
        [SerializeField] private GameObject recruitButtonPrefab; 
        [SerializeField] private Transform contentParent;   

        [Header("연결된 패널")]
        [SerializeField] private RecruitDetailPanel detailPanel; 

        public List<UnitSO> RecruitList = new List<UnitSO>();

        void Start()
        {
            PopulateList();
        }

        private void PopulateList()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            if (recruitButtonPrefab == null)
            {
                Debug.LogError("Recruit Button Prefab이 연결되지 않았습니다!");
                return;
            }

            foreach (UnitSO data in RecruitList)
            {
                GameObject buttonInstance = Instantiate(recruitButtonPrefab, contentParent);
                RecruitButton recruitButton = buttonInstance.GetComponent<RecruitButton>();

                if (recruitButton != null)
                {
                    recruitButton.Setup(data,
                        OnRecruitButtonHovered,   
                        OnRecruitButtonHoverExit,  
                        OnRecruitButtonSelected     
                    );
                }
                else
                {
                    Debug.LogError("Recruit Button Prefab에 UI_RecruitButton 스크립트가 없습니다!");
                }
            }
        }

        /// <summary>
        /// 리스트의 버튼이 클릭되었을 때 호출될 콜백 함수입니다.
        /// </summary>
        /// <param name="selectedData">선택된 버튼의 데이터</param>
        private void OnRecruitButtonHovered(UnitSO selectedData)
        {
            if (detailPanel != null)
            {
                detailPanel.ShowDetails(selectedData);
            }
        }

        private void OnRecruitButtonHoverExit()
        {
            if (detailPanel != null)
            {
                detailPanel.HideDetails();
            }
        }

        private void OnRecruitButtonSelected(UnitSO selectedData)
        {
            Debug.Log($"부대 편입 시도: {selectedData.unitName} (ID: {selectedData.unitID})");

            EditArmyManager.Instance.AddUnit( selectedData );

        }
    }
}
