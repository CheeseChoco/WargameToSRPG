using CheeseChoco.WargameToSRPG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CostPanel : MonoBehaviour
{
    [Header("UI 요소 연결")]
    [SerializeField] private TextMeshProUGUI CostText;


    private void Start()
    {
        if(EditArmyManager.Instance != null)
        {
            EditArmyManager.Instance.OnPartyUpdated += UpdateCost;
        }

        UpdateCost();
    }



    private void UpdateCost()
    {
        CostText.text = $"{EditArmyManager.Instance.currentCost} / {EditArmyManager.Instance.maxCost}";
    }
}
