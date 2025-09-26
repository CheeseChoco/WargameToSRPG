using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static finished3.ArrowTranslator;

namespace finished3
{
    public class MouseController : MonoBehaviour
    {
        public GameObject cursor;
        public float speed;

        private CharacterInfo selectedCharacter;

        private PathFinder pathFinder;
        private RangeFinder rangeFinder;
        private ArrowTranslator arrowTranslator;

        private List<OverlayTile> path = new List<OverlayTile>();
        private List<OverlayTile> rangeFinderTiles = new List<OverlayTile>();
        // korean: [추가됨] 공격 가능 범위 타일을 저장하기 위한 리스트입니다.
        private List<OverlayTile> attackRangeTiles = new List<OverlayTile>();

        private bool isMoving = false;

        private void Start()
        {
            pathFinder = new PathFinder();
            rangeFinder = new RangeFinder();
            arrowTranslator = new ArrowTranslator();
        }

        void LateUpdate()
        {
            if (path.Count > 0 && isMoving)
            {
                MoveAlongPath();
                return;
            }

            RaycastHit2D? hit = GetFocusedOnTile();
            if (!hit.HasValue) return;

            OverlayTile focusedTile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();
            UpdateCursorPosition(focusedTile);

            switch (GameManager.Instance.currentPhase)
            {
                case GameManager.GamePhase.CharacterPlacement: HandlePlacementPhase(focusedTile); break;
                case GameManager.GamePhase.PlayerTurn: HandlePlayerTurnPhase(focusedTile); break;
                case GameManager.GamePhase.EnemyTurn: break;
            }
        }

        private void HandlePlacementPhase(OverlayTile focusedTile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedCharacter == null)
                {
                    if (focusedTile.characterOnTile != null && focusedTile.characterOnTile.faction == Faction.Player)
                    {
                        selectedCharacter = focusedTile.characterOnTile;
                    }
                }
                else
                {
                    if (GameManager.Instance.placementAreaTiles.Contains(focusedTile) && focusedTile.characterOnTile == null)
                    {
                        PositionCharacterOnTile(selectedCharacter, focusedTile);
                        selectedCharacter = null;
                    }
                    else if (focusedTile.characterOnTile == selectedCharacter)
                    {
                        selectedCharacter = null;
                    }
                }
            }
        }

        private void HandlePlayerTurnPhase(OverlayTile focusedTile)
        {
            if (selectedCharacter != null && (rangeFinderTiles.Contains(focusedTile) || attackRangeTiles.Contains(focusedTile)))
            {
                var previewPath = pathFinder.FindPath(selectedCharacter.standingOnTile, focusedTile, rangeFinderTiles);
                ShowPathArrows(previewPath);
            }

            if (Input.GetMouseButtonDown(0))
            {
                // 1. 아군 유닛 선택
                if (focusedTile.characterOnTile != null && focusedTile.characterOnTile.faction == Faction.Player)
                {
                    if (!focusedTile.characterOnTile.hasActedThisTurn)
                    {
                        SelectCharacter(focusedTile.characterOnTile);
                    }
                }
                else if (selectedCharacter != null)
                {
                    // 2. 적군 유닛 공격
                    if (focusedTile.characterOnTile != null && focusedTile.characterOnTile.faction != Faction.Player && attackRangeTiles.Contains(focusedTile))
                    {
                        PerformAttack(selectedCharacter, focusedTile.characterOnTile);
                    }
                    // 3. 빈 타일로 이동
                    else if (rangeFinderTiles.Contains(focusedTile))
                    {
                        path = pathFinder.FindPath(selectedCharacter.standingOnTile, focusedTile, rangeFinderTiles);
                        if (path.Count > 0)
                        {
                            isMoving = true;
                            HideRangeAndPath();
                        }
                    }
                }
            }
        }

        // korean: [추가됨] 공격을 실행하는 함수입니다.
        private void PerformAttack(CharacterInfo attacker, CharacterInfo target)
        {
            Debug.Log(attacker.name + "가 " + target.name + "을(를) 공격합니다!");

            // (나중에 여기에 데미지 계산, HP 감소, 사망 처리 로직을 추가합니다.)

            attacker.hasActedThisTurn = true;
            DeselectCharacter();
        }

        private void SelectCharacter(CharacterInfo character)
        {
            if (isMoving) return;
            DeselectCharacter(); // 이전 선택을 깔끔하게 정리

            selectedCharacter = character;

            // 1. 순수 공격 범위 계산
            int totalRange = character.movementRange + character.attackRange;
            attackRangeTiles = rangeFinder.GetTilesInPureRange(character.standingOnTile.grid2DLocation, totalRange);

            // 2. 길찾기 이동 범위 계산
            rangeFinderTiles = rangeFinder.GetTilesInRange(character.standingOnTile.grid2DLocation, character.movementRange);

            // 3. 범위 시각화
            foreach (var tile in attackRangeTiles)
            {
                if (tile == character.standingOnTile) continue;
                tile.ShowAsAttackable(); // 먼저 모든 공격 범위를 빨간색으로 칠함
            }

            foreach (var tile in rangeFinderTiles)
            {
                if (tile == character.standingOnTile) continue;
                tile.ShowTile(); // 이동 가능한 곳을 청록색으로 덧칠함
            }
        }

        private void MoveAlongPath()
        {
            var step = speed * Time.deltaTime;
            var targetPosition = new Vector3(path[0].transform.position.x, path[0].transform.position.y + 0.0001f, path[0].transform.position.z);
            selectedCharacter.transform.position = Vector3.MoveTowards(selectedCharacter.transform.position, targetPosition, step);

            if (Vector3.Distance(selectedCharacter.transform.position, targetPosition) < 0.0001f)
            {
                PositionCharacterOnTile(selectedCharacter, path[0]);
                path.RemoveAt(0);
            }

            if (path.Count == 0)
            {
                isMoving = false;
                selectedCharacter.hasActedThisTurn = true;
                selectedCharacter = null;
            }
        }

        public void PositionCharacterOnTile(CharacterInfo character, OverlayTile newTile)
        {
            if (character.standingOnTile != null)
            {
                character.standingOnTile.characterOnTile = null;
            }
            character.transform.position = new Vector3(newTile.transform.position.x, newTile.transform.position.y + 0.0001f, newTile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = newTile.GetComponent<SpriteRenderer>().sortingOrder;
            character.standingOnTile = newTile;
            newTile.characterOnTile = character;
        }

        private void UpdateCursorPosition(OverlayTile tile)
        {
            cursor.transform.position = tile.transform.position;
            cursor.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        }

        private static RaycastHit2D? GetFocusedOnTile()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
            if (hits.Length > 0)
            {
                return hits.OrderByDescending(i => i.collider.transform.position.z).First();
            }
            return null;
        }

        private void ShowPathArrows(List<OverlayTile> pathToDisplay)
        {
            foreach (var item in rangeFinderTiles)
            {
                item.SetSprite(ArrowDirection.None);
            }
            for (int i = 0; i < pathToDisplay.Count; i++)
            {
                var previousTile = i > 0 ? pathToDisplay[i - 1] : selectedCharacter.standingOnTile;
                var futureTile = i < pathToDisplay.Count - 1 ? pathToDisplay[i + 1] : null;
                var arrow = arrowTranslator.TranslateDirection(previousTile, pathToDisplay[i], futureTile);
                pathToDisplay[i].SetSprite(arrow);
            }
        }

        private void HideRangeAndPath()
        {
            if (selectedCharacter != null && selectedCharacter.standingOnTile != null)
                selectedCharacter.standingOnTile.ResetColor();

            // korean: [수정됨] 이제 공격 범위 타일도 함께 숨깁니다.
            foreach (var tile in attackRangeTiles)
            {
                tile.ResetColor();
            }
            foreach (var tile in rangeFinderTiles)
            {
                tile.ResetColor();
                tile.SetSprite(ArrowDirection.None);
            }
            rangeFinderTiles.Clear();
            attackRangeTiles.Clear();
        }

        public void DeselectCharacter()
        {
            HideRangeAndPath();
            selectedCharacter = null;
        }
    }
}

