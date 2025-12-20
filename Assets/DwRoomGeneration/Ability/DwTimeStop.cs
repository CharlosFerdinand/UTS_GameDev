using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DwTimeStop : DwAbility
{
    private bool isReady = true;
    private bool isActive = false;
    private float abilityDuration = 4f; //4-10 second
    private DwGameManager gameManager;
    private DwPlayerMovementScript player;
    private Vector3 velocity = Vector3.zero;
    TMP_Text uiDuration;

    //reset
    int runningCoroutine = 0;

    //Lifecycle =======================================================================

    //initialization
    private void Awake()
    {
        //initialize game manager
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();

        //initialize upgrade cost
        List<int> upgradeCost = new List<int>();
        upgradeCost.Add(4); //to lv 1
        upgradeCost.Add(10); //to lv 2
        upgradeCost.Add(18); //to lv 3
        upgradeCost.Add(28); //to lv 4
        upgradeCost.Add(40); //to lv 5

        //set attribute from DwAbility
        RegisterAbility(
            Ability.TimeStop,
            21f, //base cooldown
            21f, //cooldown
            0, //start level
            upgradeCost //list of upgrade cost
            );
    }

    //update
    private void Update()
    {
        //move characters
        if (isActive)
        {
            //stops time
            Time.timeScale = 0;
        }
    }




    //Mandatory method ================================================================

    //ability, got called from game manager. basically game manager is the one who activate the ability
    override public void ActivateAbility()
    {
        //only run ability effect when ability is ready and is not active
        if (isReady && !isActive)
        {
            //turn the ability status on
            isReady = false;
            isActive = true;

            //tell game manager that time has been stopped by ability
            gameManager.isTimeStopped = true;

            //get ui element for duration
            GameObject canvas = GameObject.Find("Canvas");
            uiDuration = canvas.transform.Find("TimeStopDurationTxt").GetComponent<TMP_Text>(); //this can run because game manager ensures ability are activated only while in game where player exist and abilityScript exist
            //apply duration that counts down while time is stopped
            StartCoroutine(UnscaledInvoke(AbilityFinish, abilityDuration));
            //apply cooldown that counts down when time is running
            Invoke("CooldownFinish", getCooldown());

            //run coroutine for fov effect
            StartCoroutine(TimeStopPostProcessingCoroutine());

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
                abilityDuration = 4f;
                break;
            case 2:
                setCooldown(0.95f * getBaseCooldown());
                abilityDuration = 5.5f;
                break;
            case 3:
                setCooldown(0.9f * getBaseCooldown());
                abilityDuration = 6f;
                break;
            case 4:
                setCooldown(0.85f * getBaseCooldown());
                abilityDuration = 7.5f;
                break;
            case 5:
                setCooldown(0.8f * getBaseCooldown());
                abilityDuration = 10f;
                break;
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
        //return from time stop to normal
        gameManager.isTimeStopped = false;
        isActive = false;
        Time.timeScale = 1f;
        Physics.simulationMode = SimulationMode.FixedUpdate;
    }




    //Coroutine =======================================================================

    //visual effect coroutine (running when time stop is active)
    public IEnumerator TimeStopPostProcessingCoroutine()
    {
        //start coroutine
        runningCoroutine++;

        //declare setup variable
        float saturationInTimeStop = -100f;

        //declare variable
        Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        ColorAdjustments colorAdjustments;
        float saturationInNormal;
        float saturation;
        float timer;
        float baseFov;
        float fov;

        //get base value
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        saturationInNormal = (float) colorAdjustments.saturation;
        saturation = saturationInNormal;
        baseFov = Camera.main.fieldOfView;
        fov = baseFov;


        //for 0.5 second, raise and reduce fov and gray out the world (part 1)
        timer = 0.25f;
        while (timer > 0)
        {
            //check for reset
            if (runningCoroutine <= 0)
            {
                //reset
                Camera.main.fieldOfView = baseFov;
                yield break;
            }

            //only counts down when game is not paused
            if (!gameManager.isPaused)
            {
                //ensure all required element is not null
                if (colorAdjustments == null)
                {
                    yield break;
                }
                timer -= Time.unscaledDeltaTime;

                //linearly increase fov by 30%
                fov = Mathf.Clamp(
                    baseFov + (0.25f - timer / 0.25f) * (0.3f * baseFov)
                    , baseFov, 1.3f * baseFov);
                Camera.main.fieldOfView = fov;
            }
            yield return null;
        }
        //snap expected value
        Camera.main.fieldOfView = 1.3f*baseFov;
        saturation = saturationInNormal + 0.5f*(saturationInTimeStop - saturationInNormal);
        colorAdjustments.saturation.value = saturation;


        //for 0.5 second, raise and reduce fov and gray out the world (part 2)
        timer = 0.25f;
        while (timer > 0)
        {
            //check for reset
            if (runningCoroutine <= 0)
            {
                //reset
                Camera.main.fieldOfView = baseFov;
                yield break;
            }

            //only counts down when game is not paused
            if (!gameManager.isPaused)
            {
                //ensure all required element is not null
                if (colorAdjustments == null)
                {
                    yield break;
                }
                timer -= Time.unscaledDeltaTime;

                //desaturate coloradjustment
                colorAdjustments.saturation.value = Mathf.Clamp(
                    saturation + (0.25f - timer / 0.25f) * (saturationInTimeStop - saturationInNormal)
                    , saturationInTimeStop, saturationInNormal);
                //linearly return fov to normal
                fov = Mathf.Clamp(
                    1.3f * baseFov - (0.25f - timer / 0.25f) * (0.3f * baseFov)
                    , baseFov, 1.3f * baseFov);
                Camera.main.fieldOfView = fov;
            }
            yield return null;
        }
        //snap variable to expected value
        Camera.main.fieldOfView = gameManager.fov;
        colorAdjustments.saturation.value = saturationInTimeStop;


        //for abilityDuration - 1 second, stay. (because 0.5 second is for entry effect, and 0.5 for outro effect)
        timer = abilityDuration - 1f;
        while (timer > 0)
        {
            //check for reset
            if (runningCoroutine <= 0)
            {
                //reset
                Camera.main.fieldOfView = baseFov;
                yield break;
            }

            //only counts down when game is not paused
            if (!gameManager.isPaused)
            {
                timer -= Time.unscaledDeltaTime;
            }
            yield return null;
        }


        //for 0.5 second, return saturation from time stop to normal
        timer = 0.5f;
        while (timer > 0)
        {
            //only counts down when game is not paused
            if (!gameManager.isPaused)
            {
                //ensure all required element is not null
                if (colorAdjustments == null)
                {
                    yield break;
                }
                timer -= Time.unscaledDeltaTime;

                //return color adjustment
                saturation = Mathf.Clamp(
                    saturationInTimeStop + (0.5f - timer / 0.5f) * (saturationInNormal - saturationInTimeStop)
                    , saturationInTimeStop, saturationInNormal);
                colorAdjustments.saturation.value = saturation;
            }
            yield return null;
        }
        //snap to expected value
        colorAdjustments.saturation.value = saturationInNormal;
        Camera.main.fieldOfView = baseFov;

        //end coroutine
        runningCoroutine--;
    }

    //Action<T> is for void, T is for input
    //Func<T1, T2, ..., Tresult>
    //remove <> and T for a function that takes no parameter
    //both are call MyFunction(parameters)
    //in this case, this can be called by UnscaledInvoke(CooldownFinish, 5f);
    public IEnumerator UnscaledInvoke(Action actionToFinish, float duration)
    {
        //start coroutine
        runningCoroutine++;

        float timer = duration;
        float timestamp = Time.unscaledTime;

        //if its from ability finish
        if (actionToFinish == AbilityFinish)
        {
            uiDuration.gameObject.SetActive(true);
        }

        while (timer > 0f)
        {
            //check for reset
            if (runningCoroutine <= 0)
            {
                //stop coroutine
                yield break;
            }

            //count downward the timer only when game is running (not paused)
            if (!gameManager.isPaused)
            {
                timer -= Time.unscaledDeltaTime;
            }

            //apply duration countdown to ui when the method is ability finish
            if (actionToFinish == AbilityFinish)
            {
                uiDuration.text = Mathf.Ceil(timer).ToString();
            }

            yield return null; //wait till next frame
        }

        //reset uiDuration
        if (actionToFinish == AbilityFinish)
        {
            uiDuration.text = ""+0;
            uiDuration.gameObject.SetActive(false);
        }

        //run method
        actionToFinish.Invoke();

        //finish coroutine
        runningCoroutine--;
    }
}
