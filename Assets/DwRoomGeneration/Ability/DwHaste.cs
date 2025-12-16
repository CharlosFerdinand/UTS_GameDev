using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//list of function that can be used to interact with ability:
//public getName - get ability name in Ability enum type.
//getBaseCooldown - get base cooldown
//setCooldown - set this everytime you upgrade (if each upgrade decrease the cooldown)
//getCooldown - return the cooldown for current level
//setLevel - modify level
//getLevel - get level
//public getUpgradeCost - will return upgrade cost when not max level yet, otherwise return -1

public class DwHaste : DwAbility
{
    private bool isReady = true;
    private bool isActive = false;
    private float strength = 0.15f;
    private float abilityDuration = 5f;
    private DwGameManager gameManager;

    //reset
    private int runningCoroutine = 0;




    //Lifecycle =======================================================================

    //initialization
    private void Awake()
    {
        //initialize game manager
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();

        //initialize upgrade cost
        List<int> upgradeCost = new List<int>();
        upgradeCost.Add(2); //to lv 1
        upgradeCost.Add(5); //to lv 2
        upgradeCost.Add(13); //to lv 3
        upgradeCost.Add(25); //to lv 4
        upgradeCost.Add(37); //to lv 5

        //set attribute from DwAbility
        RegisterAbility(
            Ability.Haste,
            15f, //base cooldown
            15f, //cooldown
            0, //start level
            upgradeCost //list of upgrade cost
            );
    }


    private void FixedUpdate()
    {
        //during the duration the ability is active
        if (isActive)
        {
            //get player movement component and adjust the speed
            gameManager.player.GetComponent<DwPlayerMovementScript>().speedMultiplier
                = 1 + strength;
        }
    }




    //Mandatory Method ================================================================

    //ability
    override public void ActivateAbility()
    {
        //only run ability effect when ability is ready and is not active
        if (isReady && !isActive)
        {
            isReady = false;
            isActive = true;
            Invoke("AbilityFinish", abilityDuration);
            Invoke("CooldownFinish", getCooldown());
            
            //run coroutine for fov effect
            StartCoroutine(HasteFovCoroutine());

            //play audio
            /*
            Camera.main.gameObject.GetComponent<AudioSource>().clip = AudioBankScript.dash;
            Camera.main.gameObject.GetComponent<AudioSource>().loop = false;
            Camera.main.gameObject.GetComponent<AudioSource>().time = 0.6f;
            Camera.main.gameObject.GetComponent<AudioSource>().Play();*/
        }
    }

    //upgrade the ability, called by the game manager.
    override public void UpgradeAbility()
    {
        //set the level and then the cooldown
        setLevel(getLevel() + 1);
        switch (getLevel())
        {
            case 1:
                setCooldown(getBaseCooldown());
                abilityDuration = 5f;
                strength = 0.8f; break;
            case 2:
                setCooldown(0.9f * getBaseCooldown());
                abilityDuration = 5.2f;
                strength = 1f; break;
            case 3:
                setCooldown(0.8f * getBaseCooldown());
                abilityDuration = 5.4f;
                strength = 1.2f; break;
            case 4:
                setCooldown(0.7f * getBaseCooldown());
                abilityDuration = 5.7f;
                strength = 1.6f; break;
            case 5:
                setCooldown(0.6f * getBaseCooldown());
                abilityDuration = 6f;
                strength = 2f; break;
            default:
                break;
        }
    }
    public override bool LevelValidityCheck()
    {
        //if level is lower than 1, return invalid
        if (getLevel() < 1) return false;
        return true;
    }

    public override void NotifyAbilityRuntimeReset()
    {
        //stops all coroutine and reset
        runningCoroutine = 0;
        CooldownFinish();
        AbilityFinish();
    }

    //when cooldown is done
    private void CooldownFinish()
    {
        isReady = true;
    }

    //when duration is done
    private void AbilityFinish()
    {
        //reset
        gameManager.player.GetComponent<DwPlayerMovementScript>().speedMultiplier = 1;
        isActive = false;
    }




    //Coroutine =======================================================================

    //fov effect coroutine
    public IEnumerator HasteFovCoroutine()
    {
        //declare variable
        float timestamp = Time.time;
        float baseFov = Camera.main.fieldOfView;
        float fov = baseFov;
        
        //linearly increase fov within 0.5 second
        while (Time.time - timestamp <= 0.5f && Time.timeScale != 0)
        {
            //check for reset
            if (runningCoroutine <= 0)
            {
                //reset modification
                Camera.main.fieldOfView = baseFov;
                yield break;
            }
            fov += 0.3f * baseFov * Time.deltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(fov, baseFov, 1.15f * baseFov);
            yield return null;
        }

        //wait for duration - 1 second
        yield return new WaitForSeconds(abilityDuration - 1);
        //check for reset
        if (runningCoroutine <= 0)
        {
            //reset modification
            Camera.main.fieldOfView = baseFov;
            yield break;
        }

        //return fov by linearly decreasing fov within 0.5 second
        timestamp = Time.time;
        while (Time.time - timestamp <= 0.5f && Time.timeScale != 0)
        {
            //check for reset
            if (runningCoroutine <= 0)
            {
                //reset modification
                Camera.main.fieldOfView = baseFov;
                yield break;
            }
            fov = fov - 0.5f * baseFov * Time.deltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(fov, baseFov, 1.15f * baseFov);
            yield return null;
        }
        //snap back to normal
        Camera.main.fieldOfView = baseFov;
    }
}
