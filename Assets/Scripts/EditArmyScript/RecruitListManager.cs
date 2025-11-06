using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위함

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 모집 화면의 왼쪽 스크롤 리스트를 관리합니다.
    /// </summary>
    public class UI_RecruitListManager : MonoBehaviour
    {
        [Header("프리팹 및 부모 객체 연결")]
        [SerializeField] private GameObject recruitButtonPrefab; // UI_RecruitButton.cs가 부착된 버튼 프리팹
        [SerializeField] private Transform contentParent;      // ScrollView의 Content 객체

        [Header("연결된 패널")]
        [SerializeField] private RecruitDetailPanel detailPanel; // 상세 정보 패널

        // 테스트용 데이터. 실제로는 다른 곳(예: GameManager, DB)에서 받아옵니다.
        public List<UnitSO> RecruitList = new List<UnitSO>();

        void Start()
        {
            // 리스트 채우기
            PopulateList();
        }


        /// <summary>
        /// 데이터 리스트를 기반으로 스크롤 뷰의 내용을 채웁니다.
        /// </summary>
        private void PopulateList()
        {
            // 기존 항목들 삭제 (필요에 따라)
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            // 프리팹이 설정되었는지 확인
            if (recruitButtonPrefab == null)
            {
                Debug.LogError("Recruit Button Prefab이 연결되지 않았습니다!");
                return;
            }

            // 리스트의 모든 데이터에 대해 버튼 생성
            foreach (UnitSO data in RecruitList)
            {
                GameObject buttonInstance = Instantiate(recruitButtonPrefab, contentParent);
                RecruitButton recruitButton = buttonInstance.GetComponent<RecruitButton>();

                if (recruitButton != null)
                {
                    // 버튼 설정 (데이터와 콜백 함수 전달)
                    recruitButton.Setup(data,
                        OnRecruitButtonHovered,     // 마우스 올렸을 때
                        OnRecruitButtonHoverExit,   // 마우스 나갔을 때
                        OnRecruitButtonSelected     // 클릭했을 때
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

            // TODO: 여기에 유닛을 부대에 편입하는 실제 로직을 구현합니다.
            // 예: PlayerParty.Instance.AddUnit(selectedData);

            // TODO: 비용 차감 로직
            // 예: PlayerWallet.Instance.DeductGold(selectedData.cost);

            // TODO: (선택적) 편입 후 버튼 비활성화 또는 리스트에서 제거
            // 예: selectedData.isRecruited = true; 
            //     (이후 PopulateList()를 다시 호출하거나 버튼의 UI 상태를 변경)
        }
    }
}
