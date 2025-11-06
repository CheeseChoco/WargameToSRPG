
using System.Collections.Generic;
using UnityEngine;

public class UnitDatabaseManager : MonoBehaviour
{
    public static UnitDatabaseManager Instance { get; private set; }

    [Header("C# 데이터베이스 원본")]
    public List<UnitSO> allUnitSO;

    public Dictionary<string, UnitSO> unitDatabase { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            unitDatabase = new Dictionary<string, UnitSO>();
            foreach (UnitSO so in allUnitSO)
            {
                if (!unitDatabase.ContainsKey(so.unitID))
                {
                    unitDatabase.Add(so.unitID, so);
                }
            }
        }
    }
}