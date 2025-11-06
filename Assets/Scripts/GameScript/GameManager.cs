using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace finished3
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI 설정")]
        public GameObject actionButton;
        public TextMeshProUGUI actionButtonText;

        private UnitInfo activeCharacter;
        [Header("캐릭터 생성 설정")]
        public List<GameObject> characterPrefabs;


        public List<UnitInfo> playerCharacters { get; private set; }
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

            mouseController.DeselectCharacter();

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

            foreach (var enemy in enemyCharacters.ToList())
            {
                if (enemy.gameObject.activeSelf)
                {
                    if (enemy != null)
                    {
                        yield return StartCoroutine(enemy.GetComponent<EnemyAI>().ProcessTurn());
                        yield return new WaitForSeconds(1f);
                    }
                }
            }

            currentPhase = GamePhase.PlayerTurn;
            Debug.Log("페이즈: 플레이어 턴");
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
            PlayerArmy army = GameDataHolder.ArmyToEdit;
            Debug.Log(army.armyName);

            foreach (ArmyUnitEntry entry in army.units)
            {
                Debug.Log($"{entry.unitID} 검색");
                if (UnitDatabaseManager.Instance.unitDatabase.TryGetValue(entry.unitID, out UnitSO unitData))
                {
                    Debug.Log($"{unitData.unitName} Add");
                    characterPrefabs.Add(unitData.unitPrefab);
                }
            }




            List<OverlayTile> availableSpawnTiles = placementAreaTiles.Where(t => t.unitOnTile == null).ToList();
            for (int i = 0; i < characterPrefabs.Count; i++)
            {
                if (i >= availableSpawnTiles.Count) { break; }
                GameObject charInstance = Instantiate(characterPrefabs[i]);
                UnitInfo UnitInfo = charInstance.GetComponent<UnitInfo>();
                UnitInfo.faction = Faction.Player;
                OverlayTile spawnTile = availableSpawnTiles[i];

                mouseController.PositionunitOnTile(UnitInfo, spawnTile);
                Debug.Log($"{UnitInfo.name} 소환됨");
                playerCharacters.Add(UnitInfo);
            }
        }

        private void SpawnInitialEnemies()
        {
            List<EnemySpawnData> enemySpawnDatas = StageManager.Instance.GetEnemySpawnData(stageId);
            if (enemySpawnDatas.Count == 0)
            {
                Debug.LogWarning("GameManager에 설정된 적 스폰 좌표가 없습니다.");
                return;
            }

            for (int i = 0; i < enemySpawnDatas.Count; i++)
            {
                if (i >= enemySpawnDatas.Count)
                {
                    Debug.LogWarning("스폰 좌표보다 설정된 적 프리팹 수가 부족합니다.");
                    break;
                }

                Vector2Int spawnPos = enemySpawnDatas[i].spawnPosition;
                OverlayTile tile = MapManager.Instance.GetTileAt(spawnPos);

                if (tile != null && tile.unitOnTile == null)
                {
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

