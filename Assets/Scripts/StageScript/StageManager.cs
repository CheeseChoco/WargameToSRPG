using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    // [인스펙터 창] 여기로 우리가 만든 StageDataSO 에셋들을 드래그 앤 드롭하면 됩니다.
    public List<StageDataSO> stageDataAssets;

    private Dictionary<int, StageDataSO> stageDatabase = new Dictionary<int, StageDataSO>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 하드코딩 대신, 인스펙터에 등록된 에셋들을 기반으로 데이터베이스를 구축합니다.
        InitializeFromAssets();
    }

    /// <summary>
    /// 인스펙터에 할당된 StageDataSO 에셋들로 데이터베이스를 초기화합니다.
    /// </summary>
    private void InitializeFromAssets()
    {
        foreach (var stageData in stageDataAssets)
        {
            if (stageData != null && !stageDatabase.ContainsKey(stageData.stageId))
            {
                stageDatabase.Add(stageData.stageId, stageData);
            }
            else
            {
                Debug.LogWarning($"[StageManager] Stage ID가 중복되거나({stageData.stageId}) 에셋이 비어있습니다.");
            }
        }
    }

    /// <summary>
    /// 스테이지 ID를 받아 해당 스테이지의 적 생성 정보를 반환합니다.
    /// (메서드 내용은 이전과 거의 동일하지만, 반환 타입이 StageDataSO 내부의 리스트입니다)
    /// </summary>
    public List<EnemySpawnData> GetEnemySpawnData(int stageId)
    {
        if (stageDatabase.TryGetValue(stageId, out StageDataSO stageData))
        {
            return stageData.enemySpawnList;
        }
        else
        {
            Debug.LogError($"[StageManager] Error: 요청하신 Stage ID({stageId})가 데이터베이스에 존재하지 않습니다.");
            return null;
        }
    }
}