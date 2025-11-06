using System.Collections.Generic;
using UnityEngine;

namespace CheeseChoco.WargameToSRPG.UI
{
    /// <summary>
    /// 화면 오른쪽에 현재 부대 구성을 보여주는 패널을 관리합니다.
    /// PlayerPartyManager의 OnPartyUpdated 이벤트를 구독합니다.
    /// </summary>
    public class UI_PartyPanel : MonoBehaviour
    {
        [Header("프리팹 및 부모 객체 연결")]
        [SerializeField] private GameObject partySlotPrefab; // UI_PartySlot.cs가 부착된 프리팹
        [SerializeField] private Transform contentParent;    // 슬롯들이 생성될 부모 객체 (Grid/Vertical Layout)

        private void Start()
        {
            if (EditArmyManager.Instance == null)
            {
                Debug.LogError("PlayerPartyManager가 씬에 없습니다! UI가 작동하지 않습니다.");
                return;
            }

            // PlayerPartyManager의 OnPartyUpdated 이벤트가 호출될 때마다
            // 내 UI를 새로고침(UpdatePartyList)하도록 등록합니다.
            EditArmyManager.Instance.OnPartyUpdated += UpdatePartyList;

            // 씬 시작 시 한 번 현재 목록을 불러옵니다.
            UpdatePartyList();
        }

        private void OnDestroy()
        {
            // 이 오브젝트가 파괴될 때 이벤트 구독을 해제합니다. (메모리 누수 방지)
            if (EditArmyManager.Instance != null)
            {
                EditArmyManager.Instance.OnPartyUpdated -= UpdatePartyList;
            }
        }

        /// <summary>
        /// PlayerPartyManager의 데이터를 기반으로 UI 슬롯을 새로고침합니다.
        /// </summary>
        private void UpdatePartyList()
        {
            // 1. 기존에 있던 모든 슬롯을 삭제합니다.
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            if (partySlotPrefab == null)
            {
                Debug.LogError("Party Slot Prefab이 연결되지 않았습니다!");
                return;
            }

            // 2. PlayerPartyManager에서 최신 부대원 목록을 가져옵니다.
            List<UnitSO> currentParty = EditArmyManager.Instance.GetPartyMembers();

            // 3. 목록만큼 새 슬롯 프리팹을 생성하고 설정합니다.
            foreach (UnitSO unit in currentParty)
            {
                GameObject slotInstance = Instantiate(partySlotPrefab, contentParent);
                RemoveButton slotScript = slotInstance.GetComponent<RemoveButton>();
                if (slotScript != null)
                {
                    slotScript.Setup(unit);
                }
            }
        }
    }
}
