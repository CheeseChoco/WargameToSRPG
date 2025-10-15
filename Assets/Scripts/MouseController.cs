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

        private UnitInfo selectedCharacter;

        private PathFinder pathFinder;
        private RangeFinder rangeFinder;
        private ArrowTranslator arrowTranslator;

        private List<OverlayTile> moveAbleTiles = new List<OverlayTile>();
        // korean: [추가됨] 공격 가능 범위 타일을 저장하기 위한 리스트입니다.
        private List<OverlayTile> attackRangeTiles = new List<OverlayTile>();
        private HashSet<OverlayTile> allAttackRangeTiles = new HashSet<OverlayTile>();

        private bool isMoving = false;


        private void Start()
        {
            pathFinder = new PathFinder();
            rangeFinder = new RangeFinder();
            arrowTranslator = new ArrowTranslator();
        }

        void LateUpdate()
        {

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
                    if (focusedTile.unitOnTile != null && focusedTile.unitOnTile.faction == Faction.Player)
                    {
                        selectedCharacter = focusedTile.unitOnTile;
                    } 
                }
                else
                {
                    if (GameManager.Instance.placementAreaTiles.Contains(focusedTile) && focusedTile.unitOnTile == null)
                    {
                        PositionunitOnTile(selectedCharacter, focusedTile);
                        selectedCharacter = null;
                    }
                    else if (focusedTile.unitOnTile == selectedCharacter)
                    {
                        selectedCharacter = null;
                    }
                }
            }
        }

        private void HandlePlayerTurnPhase(OverlayTile focusedTile)
        {
            if (selectedCharacter != null && moveAbleTiles.Contains(focusedTile))
            {
                var previewPath = pathFinder.FindPath(selectedCharacter.standingOnTile, focusedTile, moveAbleTiles);
                ShowPathArrows(previewPath);
            }
            else
            {
                ShowPathArrows(new List<OverlayTile>()); // 경로 미리보기 초기화
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (focusedTile.unitOnTile != null && focusedTile.unitOnTile.faction == Faction.Player)
                {
                    if (!focusedTile.unitOnTile.hasActedThisTurn)
                    {
                        SelectCharacter(focusedTile.unitOnTile);
                    }
                }
                else if (selectedCharacter != null)
                {
                    var targetCharacter = focusedTile.unitOnTile;

                    if (targetCharacter != null && targetCharacter.faction != Faction.Player && allAttackRangeTiles.Contains(targetCharacter.standingOnTile))
                    {
                        if (selectedCharacter.hasMovedThisTurn)
                        {
                            GameManager.Instance.UnitAttack(selectedCharacter, focusedTile);
                        }
                        else
                        {
                            //Debug.Log("공격 클릭");
                            GameManager.Instance.UnitMoveNAttack(selectedCharacter, focusedTile, moveAbleTiles);
                        }
                    }
                    else if (moveAbleTiles.Contains(focusedTile))
                    {
                        GameManager.Instance.UnitMove(selectedCharacter, focusedTile, moveAbleTiles);
                    }
                    HideRangeAndPath();
                }
            }
        }

        private void SelectCharacter(UnitInfo unit)
        {
            if (isMoving) return;
            DeselectCharacter();

            selectedCharacter = unit;
            allAttackRangeTiles.Clear();

            if (!unit.hasMovedThisTurn)
            {
                moveAbleTiles = rangeFinder.GetTilesInRange(unit.standingOnTile.grid2DLocation, unit.movementRange);
                foreach (var tile in moveAbleTiles)
                {
                    var tempTiles = rangeFinder.GetTilesInPureRange(tile.grid2DLocation, unit.attackRange);
                    allAttackRangeTiles.UnionWith(tempTiles);
                }
                attackRangeTiles = allAttackRangeTiles.Except(moveAbleTiles).ToList();
            }
            else if (!unit.hasActedThisTurn)
            {
                attackRangeTiles = rangeFinder.GetTilesInPureRange(unit.standingOnTile.grid2DLocation, unit.attackRange);
                allAttackRangeTiles.UnionWith(attackRangeTiles);
            }



            foreach (var tile in attackRangeTiles)
            {
                if (tile == unit.standingOnTile) continue;
                tile.ShowAsAttackable();
            }

            foreach (var tile in moveAbleTiles)
            {
                if (tile == unit.standingOnTile) continue;
                tile.ShowTile();
            }
        }


        public void PositionunitOnTile(UnitInfo character, OverlayTile newTile)
        {
            if (character.standingOnTile != null)
            {
                character.standingOnTile.unitOnTile = null;
            }
            character.transform.position = new Vector3(newTile.transform.position.x, newTile.transform.position.y + 0.0001f, newTile.transform.position.z);
            character.GetComponent<SpriteRenderer>().sortingOrder = newTile.GetComponent<SpriteRenderer>().sortingOrder;
            character.standingOnTile = newTile;
            newTile.unitOnTile = character;
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
            foreach (var item in moveAbleTiles)
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
            foreach (var tile in moveAbleTiles)
            {
                tile.ResetColor();
                tile.SetSprite(ArrowDirection.None);
            }
            moveAbleTiles.Clear();
            attackRangeTiles.Clear();
        }

        public void DeselectCharacter()
        {
            HideRangeAndPath();
            selectedCharacter = null;
        }
    }
}

