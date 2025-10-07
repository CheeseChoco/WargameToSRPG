// EnemyAI.cs

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

        // RangeFinder와 PathFinder는 new로 생성하여 사용합니다.
        private readonly RangeFinder rangeFinder = new RangeFinder();
        private readonly PathFinder pathFinder = new PathFinder();

        void Awake()
        {
            unitInfo = GetComponent<UnitInfo>();
        }

        // GameManager에서 이 함수를 호출하여 적의 턴을 진행합니다.
        public IEnumerator ProcessTurn()
        {
            // 이 유닛이 행동할 차례임을 알립니다.
            Debug.Log(gameObject.name + "의 턴 시작.");
            unitInfo.hasActedThisTurn = false;

            // 1. 가장 가까운 플레이어 유닛을 목표로 설정합니다.
            UnitInfo target = FindClosestPlayer();
            if (target == null)
            {
                Debug.Log("공격할 대상이 없어 턴을 종료합니다.");
                unitInfo.hasActedThisTurn = true;
                yield break; // 코루틴을 즉시 종료합니다.
            }

            // 2. 현재 위치에서 바로 공격이 가능한지 확인합니다.
            // 공격 범위는 장애물을 무시하므로 'GetTilesInPureRange'를 사용합니다.
            List<OverlayTile> attackableTiles = rangeFinder.GetTilesInPureRange(unitInfo.standingOnTile.grid2DLocation, unitInfo.attackRange);

            if (attackableTiles.Contains(target.standingOnTile))
            {
                // 공격 범위에 있다면 바로 공격하고 턴을 종료합니다.
                Attack(target);
                yield return new WaitForSeconds(1f); // 시각적 효과를 위한 대기
            }
            else
            {
                // 3. 공격할 수 없다면, 목표를 향해 이동합니다.
                // 이동 범위는 장애물을 고려하므로 'GetTilesInRange'를 사용합니다.
                List<OverlayTile> movableTiles = rangeFinder.GetTilesInRange(unitInfo.standingOnTile.grid2DLocation, unitInfo.movementRange);

                // 이동 가능한 타일 중 목표와 가장 가까워지는 최적의 타일을 찾습니다.
                OverlayTile bestMoveTile = FindBestMoveTile(movableTiles, target.standingOnTile);

                if (bestMoveTile != null && bestMoveTile != unitInfo.standingOnTile)
                {
                    // --- 캐릭터 이동 로직 ---
                    // a. 원래 있던 타일에서 내 정보를 지웁니다.
                    if (unitInfo.standingOnTile != null)
                    {
                        unitInfo.standingOnTile.unitOnTile = null;
                    }

                    // b. 실제 게임 오브젝트의 위치(좌표)를 이동시킵니다.
                    transform.position = bestMoveTile.transform.position;

                    // c. 내가 서 있는 타일 정보를 새로운 타일로 갱신합니다.
                    unitInfo.standingOnTile = bestMoveTile;

                    // d. 새로 이동한 타일에 내 정보를 등록합니다.
                    bestMoveTile.unitOnTile = unitInfo;
                    // --- 이동 로직 끝 ---

                    Debug.Log(gameObject.name + "가 " + bestMoveTile.gridLocation + " 위치로 이동합니다.");
                    yield return new WaitForSeconds(1f); // 이동 효과를 위한 대기

                    // 4. 이동한 후에 공격이 가능한지 다시 확인합니다.
                    List<OverlayTile> attackableTilesAfterMove = rangeFinder.GetTilesInPureRange(unitInfo.standingOnTile.grid2DLocation, unitInfo.attackRange);
                    if (attackableTilesAfterMove.Contains(target.standingOnTile))
                    {
                        Attack(target);
                        yield return new WaitForSeconds(1f);
                    }
                }
                else
                {
                    Debug.Log(gameObject.name + "가 이동할 곳이 마땅치 않습니다.");
                }
            }

            // 모든 행동을 마쳤으므로 턴 종료 처리를 합니다.
            unitInfo.hasActedThisTurn = true;
            Debug.Log(gameObject.name + "의 턴을 종료합니다.");
        }

        private UnitInfo FindClosestPlayer()
        {
            // 씬에 있는 모든 UnitInfo 컴포넌트 중 Faction이 Player인 것만 찾습니다.
            return FindObjectsOfType<UnitInfo>()
                .Where(c => c.faction == Faction.Player)
                .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
                .ThenBy(c => c.health)
                .FirstOrDefault();
        }

        private OverlayTile FindBestMoveTile(List<OverlayTile> movableTiles, OverlayTile targetTile)
        {
            if (movableTiles == null || movableTiles.Count == 0)
            {
                return null;
            }

            // 이동 가능한 타일들 중에서, 목표 타일과의 거리가 가장 가까운 타일을 찾아 반환합니다.
            return movableTiles
                .OrderBy(t => Mathf.Abs(t.grid2DLocation.x - targetTile.grid2DLocation.x) + Mathf.Abs(t.grid2DLocation.y - targetTile.grid2DLocation.y))
                .FirstOrDefault();
        }

        private void Attack(UnitInfo target)
        {
            Debug.Log(gameObject.name + "가 " + target.name + "을(를) 공격! (" + unitInfo.attackDamage + " 데미지)");
            // UnitInfo에 이미 있는 TakeDamage 함수를 호출합니다.
            target.TakeDamage(unitInfo.attackDamage);
        }
    }
}