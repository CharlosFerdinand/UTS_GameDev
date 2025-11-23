using UnityEngine;
using UnityEngine.SceneManagement;

public class DwButtonHandler : MonoBehaviour
{
    private GameObject canvas;

    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }
    public void continueBtn()
    {
        Time.timeScale = 1f; //continue the time
    }

    public void playAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    public void returnToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }




    //SETTING MENU ===================================================================
    
    public void settingButton()
    { //open setting
        GameObject settingMenu = canvas.transform.Find("PauseScreen").Find("SettingMenu").gameObject;
        settingMenu.SetActive(true);
    }

    public void closeSettingButton()
    { //close settin
        canvas.transform.Find("PauseScreen").Find("SettingMenu").gameObject.SetActive(false);
    }
}
