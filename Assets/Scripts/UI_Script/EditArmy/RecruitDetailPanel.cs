using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 모집 화면 오른쪽의 상세 정보 패널을 관리합니다.
    /// </summary>
    public class RecruitDetailPanel : MonoBehaviour
    {
        [Header("UI 요소 연결")]
        [SerializeField] private GameObject detailPanelRoot; // 상세 정보 패널의 최상위 객체
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI unitDescriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        // [SerializeField] private Image unitPortraitImage; // 유닛 대형 이미지
        // [SerializeField] private Button recruitButton; // --- [수정] 이 버튼은 더 이상 필요하지 않습니다. ---

        private UnitSO currentSelectedData;

        private void Awake()
        {
            // --- [수정] 버튼 리스너 연결 로직 제거 ---
            // if (recruitButton != null)
            // {
            //     recruitButton.onClick.AddListener(OnRecruitClicked);
            // }

            // 시작 시에는 아무것도 선택되지 않았으므로 패널을 숨깁니다.
            if (detailPanelRoot != null)
            {
                detailPanelRoot.SetActive(false);
            }
        }

        /// <summary>
        /// 선택된 유닛의 데이터로 상세 정보 패널을 업데이트합니다.
        /// </summary>
        /// <param name="data">표시할 모집 데이터</param>
        public void ShowDetails(UnitSO data)
        {
            currentSelectedData = data;

            if (unitNameText) unitNameText.text = currentSelectedData.unitName;
            if (unitDescriptionText) unitDescriptionText.text = currentSelectedData.description;
            if (costText) costText.text = $"Cost : {currentSelectedData.pointCost}";
            // if (unitPortraitImage) unitPortraitImage.sprite = currentSelectedData.unitPortrait;

            // --- [수정] 버튼 활성화 로직 제거 ---
            // TODO: 플레이어의 소지금을 확인하고 비용과 비교하여 모집 버튼 활성화/비활성화 로직 추가
            // 예: bool canAfford = PlayerWallet.Instance.CurrentGold >= currentSelectedData.cost;
            // if (recruitButton) recruitButton.interactable = canAfford;

            // 패널을 활성화합니다.
            if (detailPanelRoot != null)
            {
                detailPanelRoot.SetActive(true);
            }
        }

        /// <summary>
        /// 상세 정보 패널을 숨깁니다.
        /// </summary>
        public void HideDetails()
        {
            if (detailPanelRoot != null)
            {
                detailPanelRoot.SetActive(false);
            }
            currentSelectedData = null;
        }

        // --- [수정] OnRecruitClicked 메서드는 더 이상 필요하지 않습니다. ---
        // /// <summary>
        // /// "모집" 버튼 클릭 시 호출됩니다.
        // /// </summary>
        // private void OnRecruitClicked()
        // {
        //     ...
        // }
    }
}

