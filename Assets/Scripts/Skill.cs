using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for all skills in the game
public class Skill : MonoBehaviour
{
    public string skillName;
    public string description;
    public int[] manaCost  = new int[MAX_ELEMENTS]; //5 different values, one for each colour (element)
    public float cooldown;    //Amount of time in seconds that must pass before player can use skill again

    public float multiplier  = 1;   //adjusts the strength of ELP

    public int hitCount  = 1;       //determines how many times a skill executes

    protected float currentTime = 0;        //gets current time to enable cooldowns
    public enum SkillType
    {
        Active,     
        Passive     //passives have no cost and are always on.
    }


    public SkillType SkillProperty  = SkillType.Active;

    public enum Element
    {
        Fire,
        Water,
        Earth,
        Light,
        Shadow
    }

    public Element skillElement;

    //SkillAction is used to determine which UseSkill method to use since there's more than one.
    public enum SkillAction
    {
        Attack,
        Heal,
        Support         //buffs and debuffs
    }

    public SkillAction skillAction;

    //constants
    const int MAX_ELEMENTS = 5;
    

    /*public Skill(string name, int[] cost, float cooldown)
    {
        manaCost = new int[MAX_ELEMENTS];
        Name = name;
        Cooldown = cooldown;

        for (int i = 0; i < MAX_ELEMENTS; i++)
        {
            manaCost[i] = cost[i];
        }
    }*/

    protected bool CanUseSkill(CharacterClass player)
    {
        bool skillAvailable = false;

        if (Time.time > currentTime + cooldown)
        {
            int matchCount = 0;
            //check if player has required mana
            for (int i = 0; i < MAX_ELEMENTS; i++)
            {
                if (player.manaPoints[i] >= manaCost[i])
                    matchCount++;
            }

            //Must have a complete match for skill to be available
            if (matchCount >= MAX_ELEMENTS)
                skillAvailable = true;
        }

        return skillAvailable;
    }

    // The following method must be overloaded because different skills have different targets.
    public virtual void UseSkill(CharacterClass caster, CharacterClass target)
    {
        //get timestamp to start the cooldown
        //currentTime = Time.time;
    }

    public virtual void UseSkill(CharacterClass caster)  //used when skill is being used on the caster only
    {
        //get timestamp to start the cooldown
        //currentTime = Time.time;
    }

    public virtual void UseSkill(CharacterClass caster, CharacterClass[] target) //used against all targets. Probably not using this one yet.
    {
        //get timestamp to start the cooldown
        //currentTime = Time.time;
    }

    public void InitializeCosts(int redMana, int blueMana, int greenMana, int goldMana, int purpleMana)
    {
        manaCost[(int)Element.Fire] = redMana;
        manaCost[(int)Element.Water] = blueMana;
        manaCost[(int)Element.Earth] = greenMana;
        manaCost[(int)Element.Light] = goldMana;
        manaCost[(int)Element.Shadow] = purpleMana;
    }

    //TODO: can I make a couroutine that works for all skills?
    /*IEnumerator DealDamage(int hitCount)
    {

    }*/
}
