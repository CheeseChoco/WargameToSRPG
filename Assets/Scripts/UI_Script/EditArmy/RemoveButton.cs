using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 부대 패널(UI_PartyPanel) 내부에 표시될 개별 유닛 슬롯 UI
    /// </summary>
    public class RemoveButton : MonoBehaviour
    {
        [Header("UI 요소 연결")]
        [SerializeField] private TextMeshProUGUI unitNameText;
        // [SerializeField] private Image unitIcon;
        [SerializeField] private Button removeButton; // 부대에서 제거하는 'X' 버튼

        private UnitSO currentData;

        /// <summary>
        /// 슬롯 UI를 설정합니다.
        /// </summary>
        public void Setup(UnitSO data)
        {
            currentData = data;

            if (unitNameText) unitNameText.text = currentData.unitName;
            // if (unitIcon) unitIcon.sprite = currentData.unitIcon;

            if (removeButton != null)
            {
                removeButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
                removeButton.onClick.AddListener(OnRemoveClicked);
            }
        }

        private void OnRemoveClicked()
        {
            if (currentData == null) return;

            // 데이터 매니저에게 제거 요청
            EditArmyManager.Instance?.RemoveUnit(currentData);
        }
    }
}
