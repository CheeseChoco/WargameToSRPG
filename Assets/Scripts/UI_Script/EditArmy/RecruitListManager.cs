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

        // 테스트용 데이터. 실제로는 다른 곳(예: GameManager, DB)에서 받아옵니다.
        private List<RecruitData> testRecruitList = new List<RecruitData>();

        void Start()
        {
            // 테스트 데이터 생성
            CreateTestData();

            // 리스트 채우기
            PopulateList();
        }

        private void CreateTestData()
        {
            testRecruitList.Add(new RecruitData("recruit_001", "검사", "기본적인 근접 유닛입니다.", 100));
            testRecruitList.Add(new RecruitData("recruit_002", "궁수", "원거리 공격 유닛입니다.", 120));
            testRecruitList.Add(new RecruitData("recruit_003", "마법사", "광역 마법을 사용합니다.", 200));
            testRecruitList.Add(new RecruitData("recruit_004", "성직자", "아군을 치유합니다.", 150));
            testRecruitList.Add(new RecruitData("recruit_005", "창병", "방어력이 높습니다.", 130));
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
            foreach (RecruitData data in testRecruitList)
            {
                // 프리팹 인스턴스화
                GameObject buttonInstance = Instantiate(recruitButtonPrefab, contentParent);

                // 버튼 스크립트 가져오기
                RecruitButton recruitButton = buttonInstance.GetComponent<RecruitButton>();

                if (recruitButton != null)
                {
                    // 버튼 설정 (데이터와 콜백 함수 전달)
                    recruitButton.Setup(data, OnRecruitButtonClicked);
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
        private void OnRecruitButtonClicked(RecruitData selectedData)
        {
            Debug.Log($"선택됨: {selectedData.unitName} (ID: {selectedData.unitID})");

            // TODO: 여기에 오른쪽 상세 정보 패널을 업데이트하는 로직을 추가합니다.
            // 예: UI_RecruitDetailPanel.ShowDetails(selectedData);
        }
    }
}
