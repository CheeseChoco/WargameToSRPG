using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void OnClickArmyEditButton()
    {
        SceneManager.LoadScene("SelectArmy");
    }
}
