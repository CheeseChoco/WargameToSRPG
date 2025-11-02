using UnityEditor.UI;
using UnityEngine;

public class SelectArmyManager : MonoBehaviour
{
    [SerializeField] private GameObject SelectArmyButtonPrefab;
    [SerializeField] private Transform contentParent;

    private PlayerArmy


    private void Start()
    {
        SaveLoadService.LoadArmy();
    }
}
