using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

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

        InitializeFromAssets();
    }

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