using UnityEngine;

namespace finished3
{
    public enum Faction
    {
        Player,
        Enemy,
        Neutral
    }

    public class UnitInfo : MonoBehaviour
    {
        public OverlayTile standingOnTile;
        public Faction faction;
        public UnitSO unitSO;

        [Header("능력치 설정")]
        public int movementRange;
        public int attackRange;

        public int health;
        public int maxHealth;
        public int attackDamage;

        [HideInInspector]
        public UnitHealthUI unitHealthUI;

        public bool hasMovedThisTurn = false;
        public bool hasActedThisTurn = false;

        private void Start()
        {
            movementRange = unitSO.movementRange; attackRange = unitSO.attackRange; maxHealth = unitSO.maxHealth; attackDamage = unitSO.attackPower;

            health = maxHealth;
            if(unitHealthUI != null)
            {
                unitHealthUI.InitializeHealth();
            }
        }


        public void TakeDamage(int damage)
        {
            health -= damage;
            Debug.Log(name + "이(가) " + damage + "의 데미지를 입었습니다. 현재 체력: " + health);

            if(unitHealthUI != null)
            {
                unitHealthUI.UpdateHealth();
            }

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log(name + "이(가) 쓰러졌습니다.");

            if (standingOnTile != null)
            {
                standingOnTile.unitOnTile = null;
            }
            GameManager.Instance.OnUnitDied(this);


            Destroy(gameObject);
        }
    }
}

