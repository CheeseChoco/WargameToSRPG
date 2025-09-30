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
        public int attackRange = 1;

        // --- [추가] ---
        public int health = 100;
        public int maxHealth = 100;
        public int attackDamage = 25;
        // --- [추가 끝] ---

        public bool hasActedThisTurn = false;

        // --- [추가] 데미지를 받는 함수 ---
        public void TakeDamage(int damage)
        {
            health -= damage;
            Debug.Log(name + "이(가) " + damage + "의 데미지를 입었습니다. 현재 체력: " + health);

            if (health <= 0)
            {
                Die();
            }
        }

        // --- [추가] 캐릭터가 사망했을 때 처리 ---
        private void Die()
        {
            Debug.Log(name + "이(가) 쓰러졌습니다.");

            // 타일에서 캐릭터 정보 제거
            if (standingOnTile != null)
            {
                standingOnTile.characterOnTile = null;
            }

            // GameManager에게 사망 사실 알리기 (나중에 구현)
            // GameManager.Instance.OnCharacterDied(this);

            // 게임 오브젝트 파괴
            Destroy(gameObject);
        }
    }
}

