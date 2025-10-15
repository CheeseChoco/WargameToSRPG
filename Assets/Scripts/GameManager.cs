using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

namespace finished3
{
    // korean: 게임의 전반적인 상태와 턴을 관리하는 '두뇌' 역할을 합니다.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI 설정")]
        public GameObject actionButton;
        public TextMeshProUGUI actionButtonText;

        private UnitInfo activeCharacter; // 현재 행동 중인 캐릭터 (플레이어)
        [Header("캐릭터 생성 설정")]
        public List<GameObject> characterPrefabs;


        public List<UnitInfo> playerCharacters { get; private set; }
        // --- [추가] 생성된 적 캐릭터들을 관리할 리스트 ---
        private List<UnitInfo> enemyCharacters;
        public List<OverlayTile> placementAreaTiles;
        
        public int stageId = 1;

        public enum GamePhase { CharacterPlacement, PlayerTurn, EnemyTurn }
        public GamePhase currentPhase { get; private set; }

        private MouseController mouseController;
        private UnitAction unitAction;
        private int actionCount;

        [Header("승리 UI")]
        public TextMeshProUGUI winText;
        private bool isGameWon = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                playerCharacters = new List<UnitInfo>();
                placementAreaTiles = new List<OverlayTile>();

                enemyCharacters = new List<UnitInfo>();
				unitAction = gameObject.AddComponent<UnitAction>();
                stageId = GameDataHolder.SelectedStageNum;
			}
        }

        private void Start()
        {

            mouseController = FindFirstObjectByType<MouseController>();

            InitializePlacementArea();
            SpawnInitialCharacters();
            SpawnInitialEnemies();

            currentPhase = GamePhase.CharacterPlacement;
            if (actionButton != null)
            {
                actionButton.SetActive(true);
                // korean: [수정됨] 텍스트를 한글에서 영어로 변경했습니다.
                actionButtonText.text = "End Placement";
            }
        }

        private void Update()
        {
            if(isGameWon && Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        public void OnActionButtonClick()
        {
            switch (currentPhase)
            {
                case GamePhase.CharacterPlacement:
                    EndPlacementAndStartBattle();
                    break;
                case GamePhase.PlayerTurn:
                    EndPlayerTurn();
                    break;
            }
        }

        private void EndPlacementAndStartBattle()
        {
            if (currentPhase != GamePhase.CharacterPlacement) return;

            foreach (var tile in placementAreaTiles)
            {
                tile.ResetColor();
            }

            StartPlayerTurn();
        }

        private void EndPlayerTurn()
        {
            if (currentPhase != GamePhase.PlayerTurn) return;

            Debug.Log("플레이어 턴 종료! 적 턴을 시작합니다.");
            currentPhase = GamePhase.EnemyTurn;

            mouseController.DeselectCharacter(); // 캐릭터 선택 해제

            if (actionButton != null)
                actionButton.SetActive(false);


            StartCoroutine(EnemyTurnRoutine());
        }

        private void StartPlayerTurn()
        {
            Debug.Log("플레이어 턴 시작!");
            currentPhase = GamePhase.PlayerTurn;

            foreach (var character in playerCharacters)
            {
                character.hasMovedThisTurn = false;
                character.hasActedThisTurn = false;
            }

            if (actionButton != null && actionCount == 0)
            {
                actionButton.SetActive(true);
                actionButtonText.text = "Turn End";
            }
        }

        private IEnumerator EnemyTurnRoutine()
        {
            Debug.Log("페이즈: 적 턴");

            // 모든 적 유닛이 행동
            foreach (var enemy in enemyCharacters.ToList())
            {
                // 캐릭터가 살아있고 활성화 상태인지 확인
                if (enemy.gameObject.activeSelf)
                {
                    // enemy가 null이 아닌지 한번 더 체크해주면 더욱 안전합니다.
                    if (enemy != null)
                    {
                        yield return StartCoroutine(enemy.GetComponent<EnemyAI>().ProcessTurn());
                        yield return new WaitForSeconds(1f);
                    }
                }
            }

            // 모든 적의 행동이 끝나면 다시 플레이어 턴으로 전환
            currentPhase = GamePhase.PlayerTurn;
            Debug.Log("페이즈: 플레이어 턴");
            // 플레이어 입력 허용 (예: MouseController 다시 활성화)
            // FindObjectOfType<MouseController>().enabled = true;
            yield return new WaitForSeconds(1.0f);
            StartPlayerTurn();
        }

        private void InitializePlacementArea()
        {
            if (MapManager.Instance.map == null || MapManager.Instance.map.Count == 0) { return; }
            int minX = MapManager.Instance.map.Keys.Min(v => v.x);
            int minY = MapManager.Instance.map.Keys.Min(v => v.y);
            int maxY = MapManager.Instance.map.Keys.Max(v => v.y);
            for (int x = minX; x <= minX + 3; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    OverlayTile tile = MapManager.Instance.GetTileAt(new Vector2Int(x, y));
                    if (tile != null)
                    {
                        placementAreaTiles.Add(tile);
                        tile.SetColor(new Color(0.1f, 0.1f, 0.8f, 0.4f));
                    }
                }
            }
        }


        private void SpawnInitialCharacters()
        {
            List<OverlayTile> availableSpawnTiles = placementAreaTiles.Where(t => t.unitOnTile == null).ToList();
            for (int i = 0; i < characterPrefabs.Count; i++)
            {
                if (i >= availableSpawnTiles.Count) { break; }
                GameObject charInstance = Instantiate(characterPrefabs[i]);
                UnitInfo UnitInfo = charInstance.GetComponent<UnitInfo>();
                UnitInfo.faction = Faction.Player;
                OverlayTile spawnTile = availableSpawnTiles[i];

                mouseController.PositionunitOnTile(UnitInfo, spawnTile);
                playerCharacters.Add(UnitInfo);
            }
        }

        // --- [추가] 적 캐릭터들을 스폰하는 함수 ---
        private void SpawnInitialEnemies()
        {
            List<EnemySpawnData> enemySpawnDatas = StageManager.Instance.GetEnemySpawnData(stageId);
            if (enemySpawnDatas.Count == 0)
            {
                Debug.LogWarning("GameManager에 설정된 적 스폰 좌표가 없습니다.");
                return;
            }

            // 1. 설정된 좌표 리스트를 순회합니다.
            for (int i = 0; i < enemySpawnDatas.Count; i++)
            {
                // 생성할 적 프리팹이 부족하면 중단합니다.
                if (i >= enemySpawnDatas.Count)
                {
                    Debug.LogWarning("스폰 좌표보다 설정된 적 프리팹 수가 부족합니다.");
                    break;
                }

                Vector2Int spawnPos = enemySpawnDatas[i].spawnPosition;
                OverlayTile tile = MapManager.Instance.GetTileAt(spawnPos);

                // 2. 해당 좌표에 타일이 있고, 비어있는지 확인합니다.
                if (tile != null && tile.unitOnTile == null)
                {
                    // 3. 적 프리팹을 생성하고 배치합니다.
                    GameObject charInstance = Instantiate(enemySpawnDatas[i].enemyPrefab);
                    UnitInfo UnitInfo = charInstance.GetComponent<UnitInfo>();
                    UnitInfo.faction = Faction.Enemy;

                    mouseController.PositionunitOnTile(UnitInfo, tile);
                    enemyCharacters.Add(UnitInfo);
                }
                else
                {
                    Debug.LogWarning("스폰 좌표 " + spawnPos + "에 타일이 없거나 다른 캐릭터가 있어서 적을 생성할 수 없습니다.");
                }
            }
        }
        public void OnUnitDied(UnitInfo deadUnit)
        {
            if (deadUnit.faction == Faction.Player)
            {
                playerCharacters.Remove(deadUnit);
            }
            else if (deadUnit.faction == Faction.Enemy)
            {
                enemyCharacters.Remove(deadUnit);

                if (enemyCharacters.Count == 0)
                {
                    winText.gameObject.SetActive(true);
                    isGameWon = true;
                }
            }
        }

        public void UnitMove(UnitInfo unit, OverlayTile tile, List<OverlayTile> rangeTiles)
		{
            actionCount++;
            unitAction.UnitMove(unit, tile, rangeTiles, () =>
            {
                unit.hasMovedThisTurn = true;
                actionCount--;

			});  
        }

        public void UnitMoveNAttack(UnitInfo unit, OverlayTile tile, List<OverlayTile> rangeTiles)
        {
            //Debug.Log("게임매니저 공격 시작");
            actionCount++;
			unitAction.UnitMoveNAttack(unit, tile, rangeTiles, () =>
			{
				unit.hasActedThisTurn = true;
				actionCount--;
			});
		}

        public void UnitAttack(UnitInfo unit, OverlayTile tile)
        {
            unit.hasActedThisTurn = true;
            unitAction.UnitAttack(unit, tile);
        }

	}
}

