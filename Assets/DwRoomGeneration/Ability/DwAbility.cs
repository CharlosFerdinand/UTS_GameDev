using System.Collections.Generic;
using UnityEngine;




//how to implement this.
//first, add ability to the Ability Enum in DwGameManager
//second, inherit this to the ability script
//third, use RegisterAbility
//fourth, override ActivateAbility

//list of function that can be used to interact with ability:
//public getName - get ability name in Ability enum type.
//getBaseCooldown - get base cooldown
//setCooldown - set this everytime you upgrade (if each upgrade decrease the cooldown)
//getCooldown - return the cooldown for current level
//setLevel - modify level
//getLevel - get level
//public getUpgradeCost - will return upgrade cost when not max level yet, otherwise return -1
public abstract class DwAbility : MonoBehaviour
{
    private Ability abilityName = Ability.None;
    private float abilityBaseCooldown = 1f;
    private float abilityCooldown = 1f;
    private int abilityLevel = 0;
    private List<int> upgradeCost = new List<int>();
    protected MonoBehaviour monoBehaviour;

    //ability name
    private void setName(Ability ability)
    {
        abilityName = ability;
    }

    public Ability getName()
    {
        return abilityName;
    }

    //ability base cooldown
    private void setBaseCooldown(float newBaseCooldown)
    {
        abilityBaseCooldown = newBaseCooldown;
    }

    protected float getBaseCooldown()
    {
        return abilityBaseCooldown;
    }

    //ability cooldown
    protected void setCooldown(float newCooldown)
    {
        abilityCooldown = newCooldown;
    }

    protected float getCooldown()
    {
        return abilityCooldown;
    }

    //ability level
    protected void setLevel(int newLevel)
    {
        abilityLevel = newLevel;
    }

    protected int getLevel()
    {
        return abilityLevel;
    }

    //adding ability
    private void setUpgradeCost(List<int> newCost)
    {
        upgradeCost.Clear();
        foreach (int cost in newCost)
        {
            upgradeCost.Add(cost);
        }
    }

    public int getUpgradeCost()
    {
        //if ability is already at max level, return -1 which must not be within range
        //so if getUpgradeCost return -1, do not upgrade.
        if (abilityLevel >= upgradeCost.Count)
        {
            return -1;
        }
        else
        {
            return upgradeCost[abilityLevel];
        }
    }


    //registering =====================================================================
    protected void RegisterAbility(
        Ability abilityName,
        float abilityBaseCooldown,
        float abilityCooldown,
        int abilityLevel,
        List<int> upgradeCost
        )
    {
        setName(abilityName);
        setBaseCooldown(abilityBaseCooldown);
        setCooldown(abilityCooldown);
        setLevel(abilityLevel);
        setUpgradeCost(upgradeCost);
    }

    public abstract void ActivateAbility();
    public abstract void UpgradeAbility();
}
