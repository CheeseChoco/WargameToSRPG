using UnityEngine;
using UnityEngine.UI; // Button을 사용하기 위해 꼭 필요합니다.

// 이 스크립트는 각 스테이지 버튼에 하나씩 붙여줄 겁니다.
public class StageButtonHelper : MonoBehaviour
{
    [Header("Stage 번호")]
    public int stageNumber;


    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    // 버튼이 클릭되었을 때 실행될 함수
    void OnButtonClick()
    {
        // StageSelectManager의 OnClickStageButton 함수를 호출하면서,
        // 파라미터로 '나 자신(gameObject)'과 '나의 스테이지 번호(stageNumber)'를 넘겨줍니다.
        StageSelectManager.Instance.OnClickStageButton(gameObject, stageNumber);
    }
}