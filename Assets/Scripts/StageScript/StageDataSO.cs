using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage_", menuName = "ScriptableObjects/Stage Data", order = 1)]
public class StageDataSO : ScriptableObject
{
    [Header("Stage Info")]
    public int stageId;

    [Header("Enemy Spawn List")]
    public List<EnemySpawnData> enemySpawnList;
}