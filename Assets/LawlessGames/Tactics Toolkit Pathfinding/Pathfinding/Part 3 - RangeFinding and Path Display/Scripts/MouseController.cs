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
        // --- [추가] 이동 후 공격할 대상을 저장하기 위한 변수 ---
        private CharacterInfo targetToAttack;

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
            if (selectedCharacter != null && rangeFinderTiles.Contains(focusedTile))
            {
                var previewPath = pathFinder.FindPath(selectedCharacter.standingOnTile, focusedTile, rangeFinderTiles);
                ShowPathArrows(previewPath);
            }
            else
            {
                ShowPathArrows(new List<OverlayTile>()); // 경로 미리보기 초기화
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
                    var targetCharacter = focusedTile.characterOnTile;

                    // 2. 적군 유닛 '이동 후 공격'
                    if (targetCharacter != null && targetCharacter.faction != Faction.Player)
                    {
                        // 공격 가능한 모든 타일 찾기 (적 주변)
                        var attackableTiles = rangeFinder.GetTilesInPureRange(targetCharacter.standingOnTile.grid2DLocation, selectedCharacter.attackRange);

                        // 이동 가능한 타일과 공격 가능한 타일의 교집합 찾기
                        var reachableAttackTiles = rangeFinderTiles.Intersect(attackableTiles).ToList();

                        if (reachableAttackTiles.Count > 0)
                        {
                            // 가장 가까운 공격 위치 찾기
                            OverlayTile bestTile = reachableAttackTiles.OrderBy(t => Vector2Int.Distance(t.grid2DLocation, selectedCharacter.standingOnTile.grid2DLocation)).First();

                            path = pathFinder.FindPath(selectedCharacter.standingOnTile, bestTile, rangeFinderTiles);
                            targetToAttack = targetCharacter; // 공격 대상 저장
                            isMoving = true;
                            HideRangeAndPath();
                        }
                        else
                        {
                            Debug.Log("공격할 수 있는 위치로 이동할 수 없습니다.");
                        }
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

            target.TakeDamage(attacker.attackDamage);

            attacker.hasActedThisTurn = true;
            DeselectCharacter();
        }

        private void SelectCharacter(CharacterInfo character)
        {
            if (isMoving) return;
            DeselectCharacter();

            selectedCharacter = character;

            // 1. 실제 이동 가능 범위(파란색)를 먼저 계산합니다.
            rangeFinderTiles = rangeFinder.GetTilesInRange(character.standingOnTile.grid2DLocation, character.movementRange);

            // 2. 이동+공격의 최대 범위(빨간색)를 계산합니다.
            int totalRange = character.movementRange + character.attackRange;
            // attackRangeTiles는 이제 순수 공격 범위만을 담게 됩니다.
            attackRangeTiles = rangeFinder.GetTilesInPureRange(character.standingOnTile.grid2DLocation, totalRange);

            // 3. 최대 범위에서 이동 범위를 제외한 부분만 빨간색으로 표시합니다.
            // LINQ의 Except를 사용하여 차집합을 구합니다.
            foreach (var tile in attackRangeTiles.Except(rangeFinderTiles))
            {
                if (tile == character.standingOnTile) continue;
                tile.ShowAsAttackable();
            }

            // 4. 이동 가능 범위는 파란색으로 표시합니다.
            foreach (var tile in rangeFinderTiles)
            {
                if (tile == character.standingOnTile) continue;
                tile.ShowTile();
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

                // 이동이 끝났고, 공격할 대상이 있다면 공격 실행
                if (targetToAttack != null)
                {
                    PerformAttack(selectedCharacter, targetToAttack);
                    targetToAttack = null; // 공격 후 타겟 정보 초기화
                }
                else // 단순 이동이었을 경우
                {
                    selectedCharacter.hasActedThisTurn = true;
                    selectedCharacter = null;
                }
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

