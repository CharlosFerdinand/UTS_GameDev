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
    [SerializeField] private GameObject uiDeathScreen;
    [SerializeField] private GameObject uiPauseScreen;
    [SerializeField] private TMP_Text uiHpText;
    [SerializeField] private Slider uiHpBar;

    [Header("ParticleEffect")]
    public GameObject bloodEffect;
    public GameObject coughBloodEffect;

    //game manager
    private DwGameManager gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //initialize game manager and start the game
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();
        gameManager.StartGame(uiDeathScreen, uiPauseScreen);
        playerMaxHp = startingMaxHp; //set max hp
        hp = playerMaxHp; //apply health
    }

    // Update is called once per frame
    void Update()
    {
        //change status to dead
        if (hp<=0 && isAlive)
        {
            isAlive = false;
            gameManager.GameOver(uiDeathScreen); //this add score to game manager
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
    public void takeDamage(float damage, GameObject damageSource)
    {
        hp -= damage;
        if (coughBloodEffect != null && damageSource.name == "Fog")
        {
            GameObject blood = Instantiate(
                coughBloodEffect,
                this.transform.position,
                Quaternion.identity
                );
            blood.transform.SetParent(this.transform);
            blood.transform.localRotation = Quaternion.identity;
        }
        else if (bloodEffect != null)
        {
            Instantiate(bloodEffect, this.transform.position, Quaternion.identity).transform.SetParent(this.transform);
        }
    }
}
