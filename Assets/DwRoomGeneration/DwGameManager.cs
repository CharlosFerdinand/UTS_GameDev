using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum Ability
{
    None,
    Dash,
    TimeStop,
    Haste
}

public class DwGameManager : MonoBehaviour
{
    //attribute =======================================================================
    //singleton object
    public static GameObject gameManager;

    //attributes
    public int point = 0;
    public int score = 0;
    public bool isPaused = false;
    public Ability ability = Ability.Dash;
    public List<DwAbility> abilityScripts = new List<DwAbility>();
    public DwAbility abilityScript;
    public GameObject player;





    //Lifecycle =======================================================================
    private void Awake()
    {
        if (gameManager != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.name = "gameManager";
            gameManager = GameObject.Find("gameManager");
            DontDestroyOnLoad(gameManager);
        }
    }

    
    void Start()
    {
        if (gameObject.GetComponent<DwDash>() == null)
        {
            gameObject.AddComponent<DwDash>();
            abilityScripts.Add(gameObject.GetComponent<DwDash>());
        }
        abilityScript = abilityScripts[0];
    }


    void Update()
    {
        if (isPaused)
        { //when game is paused, stop time.
            Time.timeScale = 0f;
        }
        else if (Time.timeScale == 0f)
        { //when game is not paused, yet time is running.
            Time.timeScale = 1f;
        }
        //activate ability on key click
        if (Input.GetKeyDown(KeyCode.Q) && abilityScript != null)
        {
            abilityScript.ActivateAbility();
        }
    }




    //lifecycle ======================================================================
    public void GameOver(GameObject uiDeathScreen)
    {
        //put score into point and update ui
        point += score;
        uiDeathScreen.transform.Find("UpgradeAbility").Find("UpgradeCost")
            .GetChild(0).gameObject.GetComponent<TMP_Text>().text
            = "UpgradeCost: " + abilityScript.getUpgradeCost();
        //show death screen, release mouse lock, pause the time
        uiDeathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void StartGame(GameObject uiDeathScreen, GameObject uiPauseScreen)
    {
        //ensure ui panel is off and lock the mouse
        uiDeathScreen.SetActive(false);
        uiPauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;

        //set up the initial value
        score = 0;
        AbilityCheck();
        player = GameObject.Find("Player");
    }

    public void PauseGame(GameObject uiPauseScreen)
    {
        //show pause ui, release mouse, pause the time
        uiPauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }
    public void ContinueGame(GameObject uiPauseScreen)
    {
        //show pause ui, release mouse, pause the time
        uiPauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }




    //method ==========================================================================

    //checks the chosen ability and return the corresponding chosen ability.
    //will return null as default value.
    private void AbilityCheck()
    {
        DwAbility targetAbility = null;
        foreach( DwAbility abilityScript in abilityScripts)
        {
            if (abilityScript.getName() == this.ability)
            {
                targetAbility = abilityScript;
            }
        }
        this.abilityScript = targetAbility;
    }




    //public method ===================================================================

    //call this method on button. Ability abilityName is the one that gets upgraded.
    public void AbilityUpgrade(Ability abilityName)
    {
        DwAbility targetAbility = null;
        foreach (DwAbility abilityScript in abilityScripts)
        {
            if (abilityScript.getName() == abilityName)
            {
                targetAbility = abilityScript;
            }
        }

        if (targetAbility.getUpgradeCost() == -1)
        {//check if target ability is already max level
            //throw exception or show popup
        }
        else if (point >= targetAbility.getUpgradeCost())
        {//if player can afford it, buy the upgrade
            point -= targetAbility.getUpgradeCost();
            targetAbility.UpgradeAbility();
        }
    }
}
