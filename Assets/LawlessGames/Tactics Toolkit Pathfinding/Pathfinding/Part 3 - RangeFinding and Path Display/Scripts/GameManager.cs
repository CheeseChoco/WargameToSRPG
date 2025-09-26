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

        public List<CharacterInfo> playerCharacters { get; private set; }
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
            }
        }

        private void Start()
        {
            mouseController = FindFirstObjectByType<MouseController>();

            InitializePlacementArea();
            SpawnInitialCharacters();

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
    }
}

