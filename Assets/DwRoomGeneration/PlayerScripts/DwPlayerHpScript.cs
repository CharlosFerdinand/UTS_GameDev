using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DwPlayerHpScript : MonoBehaviour, DwInterfaceDamageAble
{
    [Header("Stats")]
    [SerializeField] private float startingMaxHp = 100f;
    [SerializeField] private float regen = 1;
    public bool isAlive = true;
    private float playerMaxHp;
    private float hp;
    private float regenTimer;


    [Header("UI")]
    [SerializeField] private TMP_Text uiHpText;
    [SerializeField] private Slider uiHpBar;
    [SerializeField] private GameObject deathScreen;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathScreen.SetActive(false); //hide death screen in case of accidentally activating it on the editor.
        playerMaxHp = startingMaxHp; //set max hp
        hp = playerMaxHp; //apply health
    }

    // Update is called once per frame
    void Update()
    {
        //notify live status if dead
        if (hp<=0)
        {
            isAlive = false;
            deathScreen.SetActive(true); //show death screen
            Time.timeScale = 0f; //stop time
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
