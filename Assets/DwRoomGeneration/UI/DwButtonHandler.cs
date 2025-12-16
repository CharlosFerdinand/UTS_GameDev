using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DwButtonHandler : MonoBehaviour
{
    private GameObject canvas;

    //game manager
    DwGameManager gameManager;

    private void Start()
    {
        canvas = GameObject.Find("Canvas");
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();
    }


    //Navigation ======================================================================
    public void NavigateContinueBtn()
    {
        gameManager.ContinueGame(canvas.transform.Find("PauseScreen").gameObject);
    }

    public void NavigatePlayBtn()
    {
        SceneManager.LoadScene("DwScene");
    }

    public void NavigateMainMenuBtn()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("DwMainMenu");
        Time.timeScale = 1f;
    }




    //SETTING MENU ===================================================================
    
    public void SettingOpenBtn()
    { //open setting
        GameObject settingMenu = canvas.transform.Find("PauseScreen").Find("SettingMenu").gameObject;
        settingMenu.SetActive(true);
    }

    public void SettingCloseBtn()
    { //close settin
        canvas.transform.Find("PauseScreen").Find("SettingMenu").gameObject.SetActive(false);
    }
}
