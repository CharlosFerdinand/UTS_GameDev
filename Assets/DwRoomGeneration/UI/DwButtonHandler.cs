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
    public void continueBtn()
    {
        gameManager.ContinueGame(canvas.transform.Find("PauseScreen").gameObject);
    }

    public void playAgain()
    {
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



    //Upgrade
    public void dashUpgradeBtn()
    {
        //upgrade ability and update ui
        gameManager.AbilityUpgrade(Ability.Dash);
        canvas.transform.Find("DeathScreen").Find("UpgradeAbility").Find("CurrentPoint")
            .GetChild(0).gameObject.GetComponent<TMP_Text>().text =
            "Your Point: " + gameManager.point;
        canvas.transform.Find("DeathScreen").transform.Find("UpgradeAbility").Find("UpgradeCost")
            .GetChild(0).gameObject.GetComponent<TMP_Text>().text =
            "UpgradeCost: " + (
            //if upgrade cost is lower than 0, return max lvl
            gameManager.abilityScript.getUpgradeCost()<0 ? "Maxed" : gameManager.abilityScript.getUpgradeCost()
            );
    }
}
