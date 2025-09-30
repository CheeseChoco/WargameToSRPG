using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

namespace finished3
{
    // korean: 게임의 전반적인 상태와 턴을 관리하는 '두뇌' 역할을 합니다.
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("UI 설정")]
        public GameObject actionButton;
        public TextMeshProUGUI actionButtonText;

        [Header("캐릭터 생성 설정")]
        public List<GameObject> characterPrefabs;
        // --- [추가] 적 캐릭터 프리팹을 담을 리스트 ---
        public List<GameObject> enemyPrefabs;

        // --- [추가] 적 스폰 위치를 좌표로 직접 관리합니다. ---
        [Header("스테이지 설정")]
        public List<Vector2Int> enemySpawnCoordinates;

        public List<CharacterInfo> playerCharacters { get; private set; }
        // --- [추가] 생성된 적 캐릭터들을 관리할 리스트 ---
        public List<CharacterInfo> enemyCharacters { get; private set; }
        public List<OverlayTile> placementAreaTiles { get; private set; }

        public enum GamePhase { CharacterPlacement, PlayerTurn, EnemyTurn }
        public GamePhase currentPhase { get; private set; }

        private MouseController mouseController;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                playerCharacters = new List<CharacterInfo>();
                placementAreaTiles = new List<OverlayTile>();

                enemyCharacters = new List<CharacterInfo>();
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
                character.hasActedThisTurn = false;
            }

            if (actionButton != null)
            {
                actionButton.SetActive(true);
                // korean: [수정됨] 텍스트를 한글에서 영어로 변경했습니다.
                actionButtonText.text = "Turn End";
            }
        }

        private IEnumerator EnemyTurnRoutine()
        {
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
            List<OverlayTile> availableSpawnTiles = placementAreaTiles.Where(t => t.characterOnTile == null).ToList();
            for (int i = 0; i < characterPrefabs.Count; i++)
            {
                if (i >= availableSpawnTiles.Count) { break; }
                GameObject charInstance = Instantiate(characterPrefabs[i]);
                CharacterInfo characterInfo = charInstance.GetComponent<CharacterInfo>();
                characterInfo.faction = Faction.Player;
                OverlayTile spawnTile = availableSpawnTiles[i];

                mouseController.PositionCharacterOnTile(characterInfo, spawnTile);
                playerCharacters.Add(characterInfo);
            }
        }

        // --- [추가] 적 캐릭터들을 스폰하는 함수 ---
        private void SpawnInitialEnemies()
        {
            if (enemySpawnCoordinates.Count == 0)
            {
                Debug.LogWarning("GameManager에 설정된 적 스폰 좌표가 없습니다.");
                return;
            }

            // 1. 설정된 좌표 리스트를 순회합니다.
            for (int i = 0; i < enemySpawnCoordinates.Count; i++)
            {
                // 생성할 적 프리팹이 부족하면 중단합니다.
                if (i >= enemyPrefabs.Count)
                {
                    Debug.LogWarning("스폰 좌표보다 설정된 적 프리팹 수가 부족합니다.");
                    break;
                }

                Vector2Int spawnPos = enemySpawnCoordinates[i];
                OverlayTile tile = MapManager.Instance.GetTileAt(spawnPos);

                // 2. 해당 좌표에 타일이 있고, 비어있는지 확인합니다.
                if (tile != null && tile.characterOnTile == null)
                {
                    // 3. 적 프리팹을 생성하고 배치합니다.
                    GameObject charInstance = Instantiate(enemyPrefabs[i]);
                    CharacterInfo characterInfo = charInstance.GetComponent<CharacterInfo>();
                    characterInfo.faction = Faction.Enemy;

                    mouseController.PositionCharacterOnTile(characterInfo, tile);
                    enemyCharacters.Add(characterInfo);
                }
                else
                {
                    Debug.LogWarning("스폰 좌표 " + spawnPos + "에 타일이 없거나 다른 캐릭터가 있어서 적을 생성할 수 없습니다.");
                }
            }
        }
    }
}

