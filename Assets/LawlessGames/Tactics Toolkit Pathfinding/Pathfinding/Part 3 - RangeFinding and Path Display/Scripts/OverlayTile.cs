using System.Collections.Generic;
using UnityEngine;
using static finished3.ArrowTranslator;

namespace finished3
{
    public class OverlayTile : MonoBehaviour
    {
        public int G, H;
        public int F { get { return G + H; } }
        public bool isBlocked = false;
        public OverlayTile Previous;
        public Vector3Int gridLocation;
        public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }
        public CharacterInfo characterOnTile;
        public List<Sprite> arrows;
        private SpriteRenderer mainSpriteRenderer;
        private SpriteRenderer arrowSpriteRenderer;

        private void Awake()
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            mainSpriteRenderer = spriteRenderers[0];
            arrowSpriteRenderer = spriteRenderers[1];
        }

        public void ShowTile()
        {
            mainSpriteRenderer.color = new Color(0, 1, 1, 0.4f); // 청록색 (이동)
        }

        // korean: [추가됨] 공격 가능한 타일을 표시하기 위한 함수입니다.
        public void ShowAsAttackable()
        {
            mainSpriteRenderer.color = new Color(1, 0, 0, 0.4f); // 빨간색 (공격)
        }

        public void SetColor(Color color)
        {
            mainSpriteRenderer.color = color;
        }

        public void ResetColor()
        {
            mainSpriteRenderer.color = new Color(1, 1, 1, 0);
        }

        public void SetSprite(ArrowDirection d)
        {
            if (d == ArrowDirection.None)
            {
                arrowSpriteRenderer.color = new Color(1, 1, 1, 0);
            }
            else
            {
                arrowSpriteRenderer.color = new Color(1, 1, 1, 1);
                arrowSpriteRenderer.sprite = arrows[(int)d];
                arrowSpriteRenderer.sortingOrder = mainSpriteRenderer.sortingOrder + 1;
            }
        }
    }
}

