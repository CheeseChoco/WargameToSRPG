using UnityEngine;

[CreateAssetMenu(fileName = "Unit_", menuName = "SRPG/Unit Definition")]
public class UnitSO : ScriptableObject
{
    [Header("Data Key")]
    public string unitID; 

    [Header("Basic Info")]
    public string unitName;
    public string description;
    public GameObject unitPrefab;

    [Header("Point-Buy System")]
    public int pointCost;

    [Header("In-Game Stats")]
    public int maxHealth;
    public int attackPower;
    public int movementRange;
    public int attackRange;
}