using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DwAbilityUiScript : MonoBehaviour
{
    //data (external)
    private DwGameManager gameManager;
    [SerializeField] Texture dashReady;
    [SerializeField] Texture dashCooldown;
    [SerializeField] Texture hasteReady;
    [SerializeField] Texture hasteCooldown;
    [SerializeField] Texture timeStopReady;
    [SerializeField] Texture timeStopCooldown;

    //holder
    [SerializeField] RawImage uiAbilityIcon;
    [SerializeField] TMP_Text uiAbilityText;

    //interaction (internal)
    private bool onCooldown = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get game manager
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();
        
        //set initial value
        uiAbilityText.text = "Q";
        switch (gameManager.ability)
        {
            case Ability.Dash:
                uiAbilityIcon.texture = dashReady;
                break;
            case Ability.Haste:
                uiAbilityIcon.texture = hasteReady;
                break;
            case Ability.TimeStop:
                uiAbilityIcon.texture = timeStopReady;
                break;
            default:
                uiAbilityIcon.texture = dashReady;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !onCooldown)
        {
            onCooldown = true;
            switch(gameManager.ability)
            {
                case Ability.Dash:
                    StartCoroutine(CooldownCoroutine(dashReady, dashCooldown));
                    break;
                case Ability.Haste:
                    StartCoroutine(CooldownCoroutine(hasteReady, hasteCooldown));
                    break;
                case Ability.TimeStop:
                    StartCoroutine(CooldownCoroutine(timeStopReady, timeStopCooldown));
                    break;
                default:
                    StartCoroutine(CooldownCoroutine(dashReady, dashCooldown));
                    break;
            }
        }
    }

    //update ui ability cooldown
    public IEnumerator CooldownCoroutine(Texture iconReady, Texture iconCooldown)
    {
        //declare the ability, cooldown, and timer
        DwAbility ability = gameManager.abilityScript;
        float cooldown = ability.getCooldown();
        float timer = cooldown;

        //change icon to cooldown state
        uiAbilityIcon.texture = iconCooldown;

        //update ui text according to amount of cooldown left (made it so that if cooldown left is 9.1, it will show 10 because of math ceiling)
        while (timer > 0)
        {
            //timer move when time is running (timescale is not 0)
            if (Time.timeScale != 0)
            {
                //count downward
                timer -= Time.unscaledDeltaTime;
                //update ui text
                if (uiAbilityText != null)
                {
                    uiAbilityText.text = "" + Mathf.Ceil(timer);
                }
            }
            yield return null;//wait till next frame
        }

        //change icon back to it's ready state, and revert text back to "Q"
        if (uiAbilityIcon != null && uiAbilityText != null)
        {
            uiAbilityIcon.texture = iconReady;
            uiAbilityText.text = "Q";
        }

        //is no longer on cooldown
        onCooldown = false;
    }
}
