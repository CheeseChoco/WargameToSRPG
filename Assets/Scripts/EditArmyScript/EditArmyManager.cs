using UnityEngine;
using System.Collections.Generic;
using System; // Action을 사용하기 위함

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
        public int maxPartySize = 6; // 부대 최대 인원

        private List<RecruitData> partyMembers = new List<RecruitData>();

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
                DontDestroyOnLoad(this.gameObject); // 씬이 바뀌어도 유지
            }
        }

        /// <summary>
        /// 부대에 유닛을 추가하려고 시도합니다.
        /// </summary>
        /// <returns>추가 성공 여부</returns>
        public bool AddUnit(RecruitData unit)
        {
            // 부대가 꽉 찼는지 확인
            if (partyMembers.Count >= maxPartySize)
            {
                Debug.LogWarning("부대가 꽉 찼습니다. 유닛을 추가할 수 없습니다.");
                // TODO: "부대가 꽉 찼습니다" UI 피드백
                return false;
            }

            // 이미 부대에 있는지 확인
            if (partyMembers.Contains(unit))
            {
                Debug.LogWarning("이미 부대에 있는 유닛입니다.");
                // TODO: "이미 있는 유닛입니다" UI 피드백
                return false;
            }

            partyMembers.Add(unit);
            OnPartyUpdated?.Invoke(); // 부대가 변경되었음을 모두에게 알림
            return true;
        }

        /// <summary>
        /// 부대에서 유닛을 제거합니다.
        /// </summary>
        public void RemoveUnit(RecruitData unit)
        {
            if (partyMembers.Remove(unit))
            {
                OnPartyUpdated?.Invoke(); // 부대가 변경되었음을 모두에게 알림
            }
        }

        /// <summary>
        /// 현재 부대원 목록을 반환합니다.
        /// </summary>
        public List<RecruitData> GetPartyMembers()
        {
            return new List<RecruitData>(partyMembers); // 복사본 반환
        }
    }
}
