using finished3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAction : MonoBehaviour
{
    public float speed; //유닛의 이동 속도


    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> rangeTiles = new List<OverlayTile>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    // unit이랑 tile 받아서 해당 유닛 타일에서 이동
    public void UnitMove(UnitInfo unit, OverlayTile tile, List<OverlayTile> rangeTiles, Action callback)
    {
        path = pathFinder.FindPath(unit.standingOnTile, tile, rangeTiles);
        StartCoroutine(MoveCoroutine(unit, null, callback));
    }

    //적 유닛 클릭해서 공격까지 한 번에 하는 경우
    public void UnitMoveNAttack(UnitInfo unit, OverlayTile tile, List<OverlayTile> rangeTiles, Action callback)
    {

        // 공격 가능한 모든 타일 찾기 (적 주변)
        var attackableTiles = rangeFinder.GetTilesInPureRange(tile.unitOnTile.standingOnTile.grid2DLocation, unit.attackRange);

        // 이동 가능한 타일과 공격 가능한 타일의 교집합 찾기
        var reachableAttackTiles = rangeTiles.Intersect(attackableTiles).ToList();

        OverlayTile bestTile = reachableAttackTiles.OrderBy(t => Vector2Int.Distance(t.grid2DLocation, unit.standingOnTile.grid2DLocation)).First();

        path = pathFinder.FindPath(unit.standingOnTile, bestTile, rangeTiles);
        StartCoroutine(MoveCoroutine(unit, tile.unitOnTile , callback));
    }

    // 이동 코드, 공격 x
    private IEnumerator MoveCoroutine(UnitInfo unit, UnitInfo target, Action callback)
    {
        foreach (var tile in path)
        {
            var targetPosition = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.0001f, tile.transform.position.z);
            while (true)
            {
                var step = speed * Time.deltaTime;
                unit.transform.position = Vector3.MoveTowards(unit.transform.position, targetPosition, step);
                if (Vector3.Distance(unit.transform.position, targetPosition) < 0.0001f)
                {
                    PositionUnitOnTile(unit, tile);
                    break;
                }
                yield return null;
            }
        }
        if (target != null)
        {
            Attack(unit, target);
        }

        callback.Invoke();
    }

    //단순 공격
    public void Attack(UnitInfo attacker, UnitInfo defender)
    {
        defender.TakeDamage(attacker.attackDamage);
    }

    //유닛 위치 변경 시 타일, 유닛에 붙은 값 변경
    public void PositionUnitOnTile(UnitInfo unit, OverlayTile newTile)
    {
        if (unit.standingOnTile != null)
        {
            unit.standingOnTile.unitOnTile = null;
        }
        unit.transform.position = new Vector3(newTile.transform.position.x, newTile.transform.position.y + 0.0001f, newTile.transform.position.z);
        unit.GetComponent<SpriteRenderer>().sortingOrder = newTile.GetComponent<SpriteRenderer>().sortingOrder;
        unit.standingOnTile = newTile;
        newTile.unitOnTile = unit;
    }
}
