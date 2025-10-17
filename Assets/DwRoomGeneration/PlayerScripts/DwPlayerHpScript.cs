using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DwPlayerHpScript : MonoBehaviour, DwInterfaceDamageAble
{
    [Header("Stats")]
    [SerializeField] private float startingMaxHp = 100f;
    [SerializeField] private float regen = 1;
    private float playerMaxHp;
    private float hp;
    private float regenTimer;

    private DwPlayerMovementScript pMovementScript;

    [Header("UI")]
    public TMP_Text uiHpText;
    public Slider uiHpBar;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMaxHp = startingMaxHp; //set max hp
        hp = playerMaxHp; //apply health
        pMovementScript = GetComponent<DwPlayerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //notify live status if dead
        if (hp<=0)
        {
            pMovementScript.setAlive(false);
        }
        //apply regen while alive
        else
        {
            if (regenTimer > 0f)
            {
                regenTimer -= Time.deltaTime;
            }
            else
            {
                regenTimer = 5f;
                heal(regen);
            }
        }

        //update ui
        uiHpText.text = hp.ToString();
        uiHpBar.value = hp/playerMaxHp;
    }


    //heal hp
    private void heal(float healing)
    {
        hp += healing;
        hp = Mathf.Clamp(hp, -1, playerMaxHp); //heal cannot exceed max hp
    }

    //take damage
    public void takeDamage(float damage)
    {
        hp -= damage;
    }
}
