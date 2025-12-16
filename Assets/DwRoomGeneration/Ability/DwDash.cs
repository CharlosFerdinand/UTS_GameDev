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

public class DwDash : DwAbility
{
    private bool isReady = true;
    private bool isActive = false;
    private float strength = 30f;
    private float dashDuration = 0.15f;
    private DwGameManager gameManager;

    //reset
    private int runningCoroutine = 0;

    //initialization
    private void Awake()
    {
        //initialize game manager
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();

        List<int> upgradeCost = new List<int>();
        upgradeCost.Add(0); //to lv 1
        upgradeCost.Add(2); //to lv 2
        upgradeCost.Add(4); //to lv 3
        upgradeCost.Add(8); //to lv 4
        upgradeCost.Add(16); //to lv 5

        RegisterAbility(
            Ability.Dash,
            10f, //base cooldown
            10f, //cooldown
            1, //start level
            upgradeCost //list of upgrade cost
            );
    }


    private void FixedUpdate()
    {
        if (isActive)
        {
            gameManager.player.GetComponent<Rigidbody>().AddRelativeForce(
                Vector3.forward * strength, ForceMode.Impulse);
        }
    }


    //ability
    override public void ActivateAbility()
    {
        //only run ability effect when ability is ready and is not active
        if (isReady && !isActive)
        {
            isReady = false;
            isActive = true;
            StartCoroutine(DashFovCoroutine());
            Invoke("AbilityFinish", dashDuration);
            Invoke("CooldownFinish", getCooldown());

            //play audio
            Camera.main.gameObject.GetComponent<AudioSource>().clip = AudioBankScript.dash;
            Camera.main.gameObject.GetComponent<AudioSource>().loop = false;
            Camera.main.gameObject.GetComponent<AudioSource>().time = 0.6f;
            Camera.main.gameObject.GetComponent<AudioSource>().Play();
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
                strength = 30f; break;
            case 2:
                setCooldown(getBaseCooldown());
                strength = 37.5f; break;
            case 3:
                setCooldown(getBaseCooldown());
                strength = 45f; break;
            case 4:
                setCooldown(getBaseCooldown());
                strength = 52.5f; break;
            case 5:
                setCooldown(getBaseCooldown());
                strength = 60f; break;
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
        //stop coroutine and reset everything to normal
        runningCoroutine = 0;
        //isActive, isReady
        CooldownFinish();
        AbilityFinish();
    }

    private void CooldownFinish()
    {
        isReady = true;
    }


    private void AbilityFinish()
    {
        isActive = false;
    }




    //Coroutine =======================================================================

    //coroutine for setting up the fov while dashing
    public IEnumerator DashFovCoroutine()
    {
        //inform component running coroutine amount update
        runningCoroutine++;
        float fov = Camera.main.fieldOfView;
        //increase fov for 30% of the dash duration
        for (float i = 0; i < dashDuration * 0.3f; i += Time.deltaTime)
        {
            //reset checks
            if (runningCoroutine == 0)
            {
                //stops the effect from continueing and reset back to normal
                Camera.main.fieldOfView = fov;
                yield break;
            }
            Camera.main.fieldOfView = Mathf.Clamp(
                fov + (0.20f * fov * i / (dashDuration * 0.3f)),
                fov,
                1.2f * fov
                );
            yield return null;
        }

        //fov stays for 40% of the dash duration
        yield return new WaitForSeconds(dashDuration * 0.4f);
        //reset checks
        if (runningCoroutine == 0)
        {
            //stops the effect from continueing and reset back to normal
            Camera.main.fieldOfView = fov;
            yield break;
        }

        //decrease fov for 30% of the dash duration
        for (float i = 0; i < dashDuration * 0.3f; i += Time.deltaTime)
        {
            //reset checks
            if (runningCoroutine == 0)
            {
                //stops the effect from continueing and reset back to normal
                Camera.main.fieldOfView = fov;
                yield break;
            }
            Camera.main.fieldOfView = Mathf.Clamp(
                fov - (0.20f * fov * i / (dashDuration * 0.3f)),
                fov,
                1.2f * fov
                );
            yield return null;
        }

        //snap back to normal
        Camera.main.fieldOfView = fov;

        //finish coroutine
        runningCoroutine--;
    }
}
