using UnityEngine;

namespace finished3
{
    public enum Faction
    {
        Player,
        Enemy,
        Neutral
    }

    public class CharacterInfo : MonoBehaviour
    {
        public OverlayTile standingOnTile;
        public Faction faction;

        [Header("능력치 설정")]
        public int movementRange = 4;
        // korean: [추가됨] 캐릭터의 공격 사거리입니다. 근접 유닛은 1입니다.
        public int attackRange = 1;

        // korean: [수정됨] 이동 또는 공격 등 '행동'을 했는지 여부를 확인합니다.
        public bool hasActedThisTurn = false;
    }
}

