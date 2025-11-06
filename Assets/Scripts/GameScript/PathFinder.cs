using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace finished3
{
    public class PathFinder
    {
        public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> inRangeTiles)
        {
            var searchableTiles = new Dictionary<Vector2Int, OverlayTile>();

            // korean: [수정됨] 중복된 타일이 들어오더라도 에러가 나지 않도록 안전하게 처리합니다.
            foreach (var item in inRangeTiles)
            {
                item.G = int.MaxValue;
                item.Previous = null;
                if (!searchableTiles.ContainsKey(item.grid2DLocation))
                {
                    searchableTiles.Add(item.grid2DLocation, item);
                }
            }

            var openList = new List<OverlayTile>();
            var closedList = new HashSet<OverlayTile>();

            start.G = 0;
            openList.Add(start);

            while (openList.Count > 0)
            {
                OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

                openList.Remove(currentOverlayTile);
                closedList.Add(currentOverlayTile);

                if (currentOverlayTile == end)
                {
                    return GetFinishedList(start, end);
                }

                foreach (var tile in GetNeightbourOverlayTiles(currentOverlayTile, searchableTiles))
                {
                    if ((tile.unitOnTile != null && tile != end) || tile.isBlocked || closedList.Contains(tile))
                    {
                        continue;
                    }

                    int tentativeG = currentOverlayTile.G + 1;
                    if (tentativeG < tile.G)
                    {
                        tile.Previous = currentOverlayTile;
                        tile.G = tentativeG;
                        tile.H = GetManhattanDistance(end, tile);

                        if (!openList.Contains(tile))
                        {
                            openList.Add(tile);
                        }
                    }
                }
            }

            return new List<OverlayTile>();
        }

        private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        {
            List<OverlayTile> finishedList = new List<OverlayTile>();
            OverlayTile currentTile = end;

            while (currentTile != start)
            {
                finishedList.Add(currentTile);
                currentTile = currentTile.Previous;
            }

            finishedList.Reverse();

            return finishedList;
        }

        private int GetManhattanDistance(OverlayTile start, OverlayTile tile)
        {
            return Mathf.Abs(start.gridLocation.x - tile.gridLocation.x) + Mathf.Abs(start.gridLocation.y - tile.gridLocation.y);
        }

        private List<OverlayTile> GetNeightbourOverlayTiles(OverlayTile currentOverlayTile, Dictionary<Vector2Int, OverlayTile> searchableTiles)
        {
            List<OverlayTile> neighbours = new List<OverlayTile>();
            Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

            foreach (var dir in directions)
            {
                Vector2Int locationToCheck = currentOverlayTile.grid2DLocation + dir;

                if (searchableTiles.ContainsKey(locationToCheck))
                {
                    if (Mathf.Abs(currentOverlayTile.gridLocation.z - searchableTiles[locationToCheck].gridLocation.z) <= 1)
                        neighbours.Add(searchableTiles[locationToCheck]);
                }
            }

            return neighbours;
        }
    }
}

