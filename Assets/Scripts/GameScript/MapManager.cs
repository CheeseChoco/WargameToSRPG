using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace finished3
{
    public class MapManager : MonoBehaviour
    {
        private static MapManager _instance;
        public static MapManager Instance { get { return _instance; } }

        public GameObject overlayPrefab;
        public GameObject overlayContainer;

        public Dictionary<Vector2Int, OverlayTile> map;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            var tileMaps = gameObject.transform.GetComponentsInChildren<Tilemap>().OrderByDescending(x => x.GetComponent<TilemapRenderer>().sortingOrder);
            map = new Dictionary<Vector2Int, OverlayTile>();

            foreach (var tm in tileMaps)
            {
                BoundsInt bounds = tm.cellBounds;

                for (int z = bounds.max.z; z >= bounds.min.z; z--)
                {
                    if (z == 0) { continue; }

                    for (int y = bounds.min.y; y < bounds.max.y; y++)
                    {
                        for (int x = bounds.min.x; x < bounds.max.x; x++)
                        {
                            if (tm.HasTile(new Vector3Int(x, y, z)))
                            {
                                if (!map.ContainsKey(new Vector2Int(x, y)))
                                {
                                    var overlayTile = Instantiate(overlayPrefab, overlayContainer.transform);
                                    var cellWorldPosition = tm.GetCellCenterWorld(new Vector3Int(x, y, z));
                                    overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tm.GetComponent<TilemapRenderer>().sortingOrder;

                                    var overlayTileComponent = overlayTile.GetComponent<OverlayTile>();
                                    overlayTileComponent.gridLocation = new Vector3Int(x, y, z);

                                    map.Add(new Vector2Int(x, y), overlayTileComponent);
                                }
                            }
                        }
                    }
                }
            }
        }

        void Start()
        {

        }

        public OverlayTile GetTileAt(Vector2Int gridPos)
        {
            if (map.TryGetValue(gridPos, out OverlayTile tile))
            {
                return tile;
            }
            return null;
        }

        public List<OverlayTile> GetSurroundingTiles(Vector2Int originTile)
        {
            var surroundingTiles = new List<OverlayTile>();
            Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };

            if (!map.ContainsKey(originTile)) { return surroundingTiles; }

            foreach (var dir in directions)
            {
                Vector2Int tileToCheckPos = originTile + dir;
                if (map.TryGetValue(tileToCheckPos, out OverlayTile tile))
                {
                    if (Mathf.Abs(tile.gridLocation.z - map[originTile].gridLocation.z) <= 1)
                        surroundingTiles.Add(tile);
                }
            }
            return surroundingTiles;
        }
    }
}

