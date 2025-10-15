using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu] 어트리뷰트가 핵심입니다.
// 이 코드를 추가하면 Unity 에디터의 Assets/Create 메뉴에 "Stage Data" 항목이 생깁니다.
[CreateAssetMenu(fileName = "Stage_", menuName = "ScriptableObjects/Stage Data", order = 1)]
public class StageDataSO : ScriptableObject
{
    // [Header("...")]는 인스펙터 창에서 가독성을 높여줍니다.
    [Header("Stage Info")]
    public int stageId;

    [Header("Enemy Spawn List")]
    public List<EnemySpawnData> enemySpawnList;
}