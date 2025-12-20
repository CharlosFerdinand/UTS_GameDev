using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopButtonHandlerScript : MonoBehaviour
{
    //UI
    [SerializeField] private TMP_Text pointUI;
    [SerializeField] private GameObject dashUI;
    [SerializeField] private GameObject hasteUI;
    [SerializeField] private GameObject timeStopUI;

    //game manager
    private DwGameManager gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get the game manager
        gameManager = DwGameManager.gameManager.GetComponent<DwGameManager>();

        //update value of point
        pointUI.text = "Point: " + gameManager.point.ToString();

        //update dash ability description
        dashUI.transform.Find("Description").gameObject.GetComponent<TMP_Text>().text =
            //level
            "Level " + gameManager.FindAbility(Ability.Dash).getLevel().ToString() + "\n" +

            //upgrade cost
            "Upgrade Cost: " + (
            //check upgrade cost
            gameManager.FindAbility(Ability.Dash).getUpgradeCost() < 0 ?
            //if < -1, update ui to "Maxed"
            "Max" :
            //otherwise show the upgrade cost amount
            gameManager.FindAbility(Ability.Dash).getUpgradeCost()
            ) + "\n\n" +

            
            "Flung player forward"
            ;


        //update haste ability description
        hasteUI.transform.Find("Description").gameObject.GetComponent<TMP_Text>().text =
            //level
            "Level " + gameManager.FindAbility(Ability.Haste).getLevel().ToString() + "\n" +

            //upgrade cost
            "Upgrade Cost: " + (
            //check upgrade cost
            gameManager.FindAbility(Ability.Haste).getUpgradeCost() < 0 ?
            //if < -1, update ui to "Maxed"
            "Max" :
            //otherwise show the upgrade cost amount
            gameManager.FindAbility(Ability.Haste).getUpgradeCost()
            ) + "\n\n" +


            "Become faster, increased movement speed"
            ;

        //update time stop ability description
        timeStopUI.transform.Find("Description").gameObject.GetComponent<TMP_Text>().text =
            //level
            "Level " + gameManager.FindAbility(Ability.TimeStop).getLevel().ToString() + "\n" +

            //upgrade cost
            "Upgrade Cost: " + (
            //check upgrade cost
            gameManager.FindAbility(Ability.TimeStop).getUpgradeCost() < 0 ?
            //if < -1, update ui to "Maxed"
            "Max" :
            //otherwise show the upgrade cost amount
            gameManager.FindAbility(Ability.TimeStop).getUpgradeCost()
            ) + "\n\n" +

            //description
            "Stop the time momentarily, but player can still move"
            ;

        //set title equip status
        switch (gameManager.ability)
        {
            case Ability.Dash:
                dashUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                    "Dash (Equipped)";
                break;
            case Ability.Haste:
                hasteUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                    "Haste (Equipped)";
                break;
            case Ability.TimeStop:
                timeStopUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                    "Time Stop (Equipped)";
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Private Method ==================================================================
    //update ui
    private void UnequipAbilityUI()
    {
        switch (gameManager.ability)
        {
            case Ability.Dash:
                dashUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                    "Dash";
                break;
            case Ability.Haste:
                hasteUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                    "Haste";
                break;
            case Ability.TimeStop:
                timeStopUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                    "Time Stop";
                break;
        }
    }


    //Navigation ======================================================================
    
    public void NavigateMainMenuBtn()
    {
        SceneManager.LoadScene("DwMainMenu");
    }

    public void NavigatePlayBtn()
    {
        SceneManager.LoadScene("DwScene");
    }



    //Equip Button ====================================================================

    public void EquipDashBtn()
    {
        //unequip current ability
        UnequipAbilityUI();

        //equip the ability
        gameManager.EquipAbility(Ability.Dash);

        //update ui title to show which one is equipped
        dashUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
            "Dash (Equipped)";
    }
    public void EquipHasteBtn()
    {
        //if haste level is not valid
        if (!gameManager.FindAbility(Ability.Haste).LevelValidityCheck())
        {
            //tell user that the ability is still locked
            hasteUI.transform.Find("AbilityName").gameObject.GetComponent<TMP_Text>().text =
                "Haste (Locked)";
        }
        else
        {
            //unequip current ability
            UnequipAbilityUI();

            //equip the ability
            gameManager.EquipAbility(Ability.Haste);

            //update ui title to show which one is equipped
            hasteUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                "Haste (Equipped)";
        }
    }
    public void EquipTimeStopBtn()
    {
        //if level is invalid
        if (!gameManager.FindAbility(Ability.TimeStop).LevelValidityCheck())
        {
            //tell user that the ability is still locked
            timeStopUI.transform.Find("AbilityName").gameObject.GetComponent<TMP_Text>().text =
                "Time Stop (Locked)";
        }
        else
        {
            //unequip current ability
            UnequipAbilityUI();

            //equip the ability
            gameManager.EquipAbility(Ability.TimeStop);

            //update ui title to show which one is equipped
            timeStopUI.transform.Find("AbilityName").GetComponent<TMP_Text>().text =
                "Time Stop (Equipped)";
        }
    }


    //Upgrade Button ==================================================================
    public void UpgradeDashBtn()
    {
        //upgrade ability
        gameManager.UpgradeAbility(Ability.Dash);

        //update value of point
        pointUI.text = "Point: " + gameManager.point.ToString();

        //update dash ability description
        dashUI.transform.Find("Description").GetComponent<TMP_Text>().text =
            //level
            "Level " + gameManager.FindAbility(Ability.Dash).getLevel().ToString() + "\n" +

            //upgrade cost
            "Upgrade Cost: " + (
            //check upgrade cost
            gameManager.FindAbility(Ability.Dash).getUpgradeCost() < 0 ?
            //if < -1, update ui to "Maxed"
            "Max" :
            //otherwise show the upgrade cost amount
            gameManager.FindAbility(Ability.Dash).getUpgradeCost()
            ) + "\n\n" +

            //ability description
            "Flung player forward"
            ;
    }
    public void UpgradeHasteBtn()
    {
        //if haste level is currently 0
        if (gameManager.FindAbility(Ability.Haste).getLevel() == 0)
        {
            //upgrade ability
            gameManager.UpgradeAbility(Ability.Haste);
            //and then haste level turn from 0 to 1, change from haste locked to haste
            if (gameManager.FindAbility(Ability.Haste).getLevel() == 1)
            {
                hasteUI.transform.Find("AbilityName").gameObject.GetComponent<TMP_Text>().text =
                    "Haste";
            }
        }
        else
        { //upgrade normally
            //upgrade ability
            gameManager.UpgradeAbility(Ability.Haste);
        }

        //update value of point
        pointUI.text = "Point: " + gameManager.point.ToString();

        //update haste ability description
        hasteUI.transform.Find("Description").GetComponent<TMP_Text>().text =
            //level
            "Level " + gameManager.FindAbility(Ability.Haste).getLevel().ToString() + "\n" +

            //upgrade cost
            "Upgrade Cost: " + (
            //check upgrade cost
            gameManager.FindAbility(Ability.Haste).getUpgradeCost() < 0 ?
            //if < -1, update ui to "Maxed"
            "Max" :
            //otherwise show the upgrade cost amount
            gameManager.FindAbility(Ability.Haste).getUpgradeCost()
            ) + "\n\n" +

            //ability description
            "Become faster, increased movement speed"
            ;
    }
    public void UpgradeTimeStopBtn()
    {
        //if time stop level is currently 0
        if (gameManager.FindAbility(Ability.TimeStop).getLevel() == 0)
        {
            //upgrade ability
            gameManager.UpgradeAbility(Ability.TimeStop);
            //if then time stop level turn from 0 to 1, change from haste locked to haste
            if (gameManager.FindAbility(Ability.TimeStop).getLevel() == 1)
            {
                //make the title no longer say Locked
                timeStopUI.transform.Find("AbilityName").gameObject.GetComponent<TMP_Text>().text =
                    "Time Stop";
            }
        }
        else
        { //upgrade normally
            //upgrade ability
            gameManager.UpgradeAbility(Ability.TimeStop);
        }

        //update value of point
        pointUI.text = "Point: " + gameManager.point.ToString();

        //update time stop ability description
        timeStopUI.transform.Find("Description").gameObject.GetComponent<TMP_Text>().text =
            //level
            "Level " + gameManager.FindAbility(Ability.TimeStop).getLevel().ToString() + "\n" +

            //upgrade cost
            "Upgrade Cost: " + (
            //check upgrade cost
            gameManager.FindAbility(Ability.TimeStop).getUpgradeCost() < 0 ?
            //if < -1, update ui to "Maxed"
            "Max" :
            //otherwise show the upgrade cost amount
            gameManager.FindAbility(Ability.TimeStop).getUpgradeCost()
            ) + "\n\n" +

            //description
            "Stop the time momentarily, but player can still move"
            ;
    }
}
