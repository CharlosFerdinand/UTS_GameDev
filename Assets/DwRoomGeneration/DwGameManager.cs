using System.Collections.Generic;
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
    private static GameObject gameManager;

    //attributes
    public int point = 0;
    public int score = 0;
    public Ability ability = Ability.None;
    public List<DwAbility> abilityScripts = new List<DwAbility>();
    public DwAbility abilityScript;




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
        gameObject.AddComponent<DwDash>();
        abilityScripts.Add(gameObject.GetComponent<DwDash>());
        abilityScript = abilityScripts[0];
    }


    void Update()
    {
        //activate ability on key click
        if (Input.GetKeyDown(KeyCode.Q))
        {
            abilityScript.ActivateAbility();
        }
    }




    //methods =========================================================================
    private void GameOver()
    {
        score = 0;

    }

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
