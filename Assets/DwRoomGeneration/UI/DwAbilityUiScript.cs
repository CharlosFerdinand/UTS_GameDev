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

    //holder
    [SerializeField] RawImage uiAbilityIcon;
    [SerializeField] TMP_Text uiAbilityText;

    //interaction (internal)
    private bool onCooldown = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();
        uiAbilityIcon.texture = dashReady;
        uiAbilityText.text = "Q";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !onCooldown)
        {
            onCooldown = true;
            StartCoroutine(CooldownCoroutine());
        }
    }

    //update ui ability cooldown
    public IEnumerator CooldownCoroutine()
    {
        //declare the ability, cooldown, and stamp the time
        DwAbility ability = gameManager.abilityScript;
        float cooldown = ability.getCooldown();
        float timeStamp = Time.time;

        //change icon to cooldown state
        uiAbilityIcon.texture = dashCooldown;

        //update ui text according to amount of cooldown left (made it so that if cooldown left is 9.1, it will show 10 because of math ceiling)
        while (Time.time < timeStamp + cooldown)
        {
            //get amount of time passed
            float timePassed = Time.time - timeStamp;
            float timeLeft = cooldown - timePassed;
            //update ui text
            uiAbilityText.text = "" + Mathf.Ceil(timeLeft);
            yield return null;//wait till next frame
        }

        //change icon back to it's ready state, and revert text back to "Q"
        uiAbilityIcon.texture = dashReady;
        uiAbilityText.text = "Q";

        //is no longer on cooldown
        onCooldown = false;
    }
}
