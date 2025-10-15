// cheesechoco/wargametosrpg/CheeseChoco-WargameToSRPG-584446910ddf6dc52d69d183aede42826dac4dc6/Assets/LawlessGames/Tactics Toolkit Pathfinding/Pathfinding/Part 3 - RangeFinding and Path Display/Scripts/EnemyAI.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace finished3
{
    [RequireComponent(typeof(UnitInfo))]
    public class EnemyAI : MonoBehaviour
    {
        private UnitInfo unitInfo;
        private UnitAction unitAction; // --- [추가] UnitAction 참조 ---

        private readonly RangeFinder rangeFinder = new RangeFinder();
        private bool isActionFinished = false; // --- [추가] 행동 완료를 체크하기 위한 플래그 ---

        void Awake()
        {
            unitInfo = GetComponent<UnitInfo>();
            // GameManager를 통해 UnitAction 인스턴스를 가져옵니다.
            unitAction = FindFirstObjectByType<GameManager>().GetComponent<UnitAction>();
        }

        public IEnumerator ProcessTurn()
        {
            Debug.Log(gameObject.name + "의 턴 시작.");
            unitInfo.hasActedThisTurn = false;
            isActionFinished = false;

            // 1. 목표 설정 (가장 가깝고 체력이 적은 플레이어)
            UnitInfo target = FindClosestPlayer();
            if (target == null)
            {
                Debug.Log("공격할 대상이 없어 턴을 종료합니다.");
                unitInfo.hasActedThisTurn = true;
                yield break;
            }

            // 이동 가능한 타일 범위 계산
            List<OverlayTile> movableTiles = rangeFinder.GetTilesInRange(unitInfo.standingOnTile.grid2DLocation, unitInfo.movementRange);

            // 2. 현재 위치에서 바로 공격 가능한지 확인
            List<OverlayTile> tilesInAttackRange = rangeFinder.GetTilesInPureRange(unitInfo.standingOnTile.grid2DLocation, unitInfo.attackRange);
            if (tilesInAttackRange.Contains(target.standingOnTile))
            {
                Debug.Log(gameObject.name + "가 즉시 공격합니다.");
                unitAction.Attack(unitInfo, target);
                isActionFinished = true;
            }
            else
            {
                // 3. 이동 후 공격할 최적의 위치 탐색
                var attackableTiles = rangeFinder.GetTilesInPureRange(target.standingOnTile.grid2DLocation, unitInfo.attackRange);
                var reachableAttackTiles = movableTiles.Intersect(attackableTiles).ToList();

                if (reachableAttackTiles.Any())
                {
                    Debug.Log(gameObject.name + "가 공격을 위해 이동합니다.");
                    // UnitAction의 이동 후 공격 메서드 호출
                    unitAction.UnitMoveNAttack(unitInfo, target.standingOnTile, movableTiles, () => {
                        isActionFinished = true;
                    });
                }
                // 4. 공격은 못하지만, 목표에게 다가갈 수 있는 경우
                else
                {
                    OverlayTile bestMoveTile = FindBestMoveTile(movableTiles, target.standingOnTile);
                    if (bestMoveTile != null && bestMoveTile != unitInfo.standingOnTile)
                    {
                        Debug.Log(gameObject.name + "가 목표에게 접근합니다.");
                        // UnitAction의 이동 메서드 호출
                        unitAction.UnitMove(unitInfo, bestMoveTile, movableTiles, () => {
                            isActionFinished = true;
                        });
                    }
                    else
                    {
                        Debug.Log(gameObject.name + "가 이동할 곳이 없어 턴을 종료합니다.");
                        isActionFinished = true; // 행동할 것이 없으므로 바로 종료
                    }
                }
            }

            // UnitAction의 행동(이동, 공격)이 완료될 때까지 대기
            yield return new WaitUntil(() => isActionFinished);

            unitInfo.hasActedThisTurn = true;
            Debug.Log(gameObject.name + "의 턴을 종료합니다.");
        }

        private UnitInfo FindClosestPlayer()
        {
            return FindObjectsOfType<UnitInfo>()
                .Where(c => c.faction == Faction.Player)
                .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                .ThenBy(c => c.health)
                .FirstOrDefault();
        }

        private OverlayTile FindBestMoveTile(List<OverlayTile> movableTiles, OverlayTile targetTile)
        {
            if (movableTiles == null || !movableTiles.Any())
            {
                return null;
            }

            // 맨해튼 거리(가로+세로)가 가장 가까운 타일을 목표로 설정
            return movableTiles
                .OrderBy(t => Mathf.Abs(t.grid2DLocation.x - targetTile.grid2DLocation.x) + Mathf.Abs(t.grid2DLocation.y - targetTile.grid2DLocation.y))
                .FirstOrDefault();
        }
    }
}