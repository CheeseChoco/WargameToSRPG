using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace finished3
{
    public class RangeFinder
    {
        // korean: 장애물을 고려하여 실제 '이동' 가능한 범위를 BFS로 찾습니다.
        public List<OverlayTile> GetTilesInRange(Vector2Int location, int range)
        {
            var startingTile = MapManager.Instance.GetTileAt(location);
            if (startingTile == null) { return new List<OverlayTile>(); }

            var inRangeTiles = new List<OverlayTile>();
            var visited = new HashSet<OverlayTile>();
            var distance = new Dictionary<OverlayTile, int>();
            var queue = new Queue<OverlayTile>();

            queue.Enqueue(startingTile);
            visited.Add(startingTile);
            distance.Add(startingTile, 0);

            inRangeTiles.Add(startingTile);

            while (queue.Count > 0)
            {
                var currentTile = queue.Dequeue();

                if (distance[currentTile] < range)
                {
                    foreach (var neighbour in MapManager.Instance.GetSurroundingTiles(currentTile.grid2DLocation))
                    {
                        if (visited.Contains(neighbour)) { continue; }
                        visited.Add(neighbour);

                        if (neighbour.isBlocked || neighbour.unitOnTile != null) { continue; }

                        distance.Add(neighbour, distance[currentTile] + 1);
                        queue.Enqueue(neighbour);
                        inRangeTiles.Add(neighbour);
                    }
                }
            }
            return inRangeTiles;
        }

        // korean: [추가됨] 장애물을 무시하고 순수 맨해튼 거리만 계산하여 '공격' 범위를 찾습니다.
        public List<OverlayTile> GetTilesInPureRange(Vector2Int location, int range)
        {
            List<OverlayTile> inRangeTiles = new List<OverlayTile>();

            foreach (var tile in MapManager.Instance.map.Values)
            {
                int distance = Mathf.Abs(location.x - tile.grid2DLocation.x) + Mathf.Abs(location.y - tile.grid2DLocation.y);
                if (distance <= range)
                {
                    inRangeTiles.Add(tile);
                }
            }
            return inRangeTiles;
        }
    }
}

