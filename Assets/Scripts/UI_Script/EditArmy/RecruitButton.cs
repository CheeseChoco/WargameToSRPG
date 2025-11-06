using System; // Action을 사용하기 위함
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 모집 리스트의 개별 항목 버튼에 부착되는 스크립트입니다.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RecruitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("UI 요소 연결")]
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI costText;
        // [SerializeField] private Image unitIconImage; // 아이콘용 이미지

        private UnitSO currentData;
        private Action<UnitSO> onHoverCallback;
        private Action onHoverExitCallback;
        private Action<UnitSO> onClickCallback;

        /// <summary>
        /// 버튼을 초기화하고 데이터를 설정합니다.
        /// </summary>
        /// <param name="data">표시할 모집 데이터</param>
        /// <param name="callback">버튼 클릭 시 호출될 콜백 함수</param>
        public void Setup(UnitSO data, Action<UnitSO> hoverCallback, Action hoverExitCallback, Action<UnitSO> clickCallback)
        {
            currentData = data;
            onHoverCallback = hoverCallback;
            onHoverExitCallback = hoverExitCallback;
            onClickCallback = clickCallback;

            // UI 업데이트
            if (unitNameText) unitNameText.text = currentData.unitName;
            if (costText) costText.text = $"Cost: {currentData.pointCost}";
            // if (unitIconImage) unitIconImage.sprite = currentData.unitIcon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverCallback?.Invoke(currentData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExitCallback?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClickCallback?.Invoke(currentData);
        }
    }
}
