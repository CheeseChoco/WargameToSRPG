using UnityEngine;
using UnityEngine.UI;
//using TMPro;
using finished3;
public class UnitHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    //public TextMeshProUGUI healthText;

    private UnitInfo unitInfo;
    private Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unitInfo = GetComponentInParent<UnitInfo>();
        if (unitInfo == null)
        {
            Debug.LogError("유닛인포 스크립트 없음");
            return;
        }
        mainCamera = Camera.main;

        unitInfo.unitHealthUI = this;

        InitializeHealth();

    }

    // Update is called once per frame
    void LastUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }

    /// <summary>
    /// 체력 바의 최대치와 현재 값을 초기화합니다.
    /// </summary>
    public void InitializeHealth()
    {
        if (unitInfo == null) return;

        healthSlider.maxValue = unitInfo.maxHealth;
        healthSlider.value = unitInfo.health;

        //UpdateHealthText();
    }

    // <summary>
    // 현재 체력을 기반으로 UI를 업데이트합니다.
    // </summary>
    public void UpdateHealth()
    {
        if (unitInfo == null) return;

        healthSlider.value = unitInfo.health;
        //UpdateHealthText();
    }

    /// <summary>
    /// (선택 사항) 체력 텍스트를 업데이트합니다.
    /// </summary>
    //private void UpdateHealthText()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{unitInfo.health} / {unitInfo.maxHealth}";
    //    }
    //}
}
