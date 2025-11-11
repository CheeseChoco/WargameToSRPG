using finished3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAction : MonoBehaviour
{
    public float speed = 5;


    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> rangeTiles = new List<OverlayTile>();
    void Awake()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    public void UnitMove(UnitInfo unit, OverlayTile tile, List<OverlayTile> rangeTiles, Action callback)
    {
        path = pathFinder.FindPath(unit.standingOnTile, tile, rangeTiles);
        StartCoroutine(MoveCoroutine(unit, null, callback));
    }

    public void UnitMoveNAttack(UnitInfo unit, OverlayTile tile, List<OverlayTile> rangeTiles, Action callback)
    {

        var attackableTiles = rangeFinder.GetTilesInPureRange(tile.unitOnTile.standingOnTile.grid2DLocation, unit.attackRange);

        var reachableAttackTiles = rangeTiles.Intersect(attackableTiles).ToList();

        OverlayTile bestTile = reachableAttackTiles.OrderBy(t => Vector2Int.Distance(t.grid2DLocation, unit.standingOnTile.grid2DLocation)).First();

        path = pathFinder.FindPath(unit.standingOnTile, bestTile, rangeTiles);
        StartCoroutine(MoveCoroutine(unit, tile.unitOnTile , callback));
    }

    public void UnitAttack(UnitInfo unit, OverlayTile tile)
    {
        Attack(unit, tile.unitOnTile);
    }

    private IEnumerator MoveCoroutine(UnitInfo unit, UnitInfo target, Action callback)
    {

        //Debug.Log("이동 코루틴 시작");
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

    public void Attack(UnitInfo attacker, UnitInfo defender)
    {
        defender.TakeDamage(attacker.attackDamage);
    }

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
