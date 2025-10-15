using UnityEngine;

// 이 클래스는 StageData 에셋 안에서 사용될 것이므로 변경할 필요가 없습니다.
[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public Vector2Int spawnPosition;
}