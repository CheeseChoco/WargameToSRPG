using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Action을 사용하기 위함

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 모집 리스트의 개별 항목 버튼에 부착되는 스크립트입니다.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RecruitButton : MonoBehaviour
    {
        [Header("UI 요소 연결")]
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI costText;
        // [SerializeField] private Image unitIconImage; // 아이콘용 이미지

        private Button button;
        private RecruitData currentData;
        private Action<RecruitData> onClickCallback;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
        }

        /// <summary>
        /// 버튼을 초기화하고 데이터를 설정합니다.
        /// </summary>
        /// <param name="data">표시할 모집 데이터</param>
        /// <param name="callback">버튼 클릭 시 호출될 콜백 함수</param>
        public void Setup(RecruitData data, Action<RecruitData> callback)
        {
            currentData = data;
            onClickCallback = callback;

            // UI 업데이트
            if (unitNameText) unitNameText.text = currentData.unitName;
            if (costText) costText.text = $"비용: {currentData.cost}";
            // if (unitIconImage) unitIconImage.sprite = currentData.unitIcon;
        }

        /// <summary>
        /// 버튼이 클릭되었을 때 호출됩니다.
        /// </summary>
        private void OnButtonClicked()
        {
            // 관리자에게 이 버튼의 데이터와 함께 클릭 이벤트를 알립니다.
            onClickCallback?.Invoke(currentData);
        }
    }
}
