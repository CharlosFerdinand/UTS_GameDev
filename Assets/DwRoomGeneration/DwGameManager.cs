using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum Ability
{
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
        //if game manager already exist, destroy this to prevent duplicate
        if (gameManager != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //first time created, set name, get the game manager into static var, dont destroy
            this.gameObject.name = "gameManager";
            gameManager = GameObject.Find("gameManager");
            DontDestroyOnLoad(gameManager);

            //initialize ability component
            if (gameObject.GetComponent<DwDash>() == null)
            {
                gameObject.AddComponent<DwDash>();
                abilityScripts.Add(gameObject.GetComponent<DwDash>());
            }
            if (gameObject.GetComponent<DwHaste>() == null)
            {
                gameObject.AddComponent<DwHaste>();
                abilityScripts.Add(gameObject.GetComponent<DwHaste>());
            }/*
            if (gameObject.GetComponent<DwTimeStop>() == null)
            {
                gameObject.AddComponent<DwTimeStop>();
                abilityScripts.Add(gameObject.GetComponent<DwTimeStop>());
            }*/
            //apply default ability
            abilityScript = abilityScripts[0];
        }
    }

    
    void Start()
    {
        
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

    //checks the chosen ability and update the current ability script.
    //default ability script is DwDash
    private void AbilityCheck()
    {
        //find ability according to current ability
        DwAbility targetAbility = FindAbility(this.ability);

        //if null
        if (targetAbility == null) targetAbility = abilityScripts[0];

        //check level validity
        if (targetAbility.LevelValidityCheck())
        {
            //if valid, apply script
            this.abilityScript = targetAbility;
        }
        else
        {
            //if invalid, apply dash ability
            this.ability = Ability.Dash;
            this.abilityScript = abilityScripts[0];
        }
    }




    //public method ===================================================================

    //return ability script with the corresponding ability name from list of ability script (abilityScripts)
    public DwAbility FindAbility(Ability target)
    {
        DwAbility targetAbility = null;
        foreach (DwAbility possibleAbilityScript in abilityScripts)
        {
            if (possibleAbilityScript.getName() == target)
            {
                targetAbility = possibleAbilityScript;
            }
        }
        return targetAbility;
    }


    //Button handler method ===========================================================

    //call this method from a button handler
    public void UpgradeAbility(Ability abilityName)
    {
        //find ability script
        DwAbility targetAbility = FindAbility(abilityName);

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

    //call this method from button handler
    public void EquipAbility(Ability abilityName)
    {
        ability = abilityName;
    }
}
