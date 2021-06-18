using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for all skills in the game
public class Skill
{
   public string Name { get; set; }
    public int[] ManaCost { get; set; } = new int[MAX_ELEMENTS]; //5 different values, one for each colour (element)
   public float Cooldown { get; set; }   //Amount of time in seconds that must pass before player can use skill again

    public float Multiplier { get; set; } = 1;   //adjusts the strength of ELP

    public int HitCount { get; set; } = 1;       //determines how many times a skill executes

    protected float CurrentTime { get; set; }       //gets current time to enable cooldowns
    public enum SkillType
    {
        Active,     
        Passive     //passives have no cost and are always on.
    }

    public SkillType SkillProperty { get; set; } = SkillType.Active;

    public enum Element
    {
        Fire,
        Water,
        Earth,
        Light,
        Shadow
    }

    public Element SkillElement { get; set; }

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

        if (Time.time > CurrentTime + Cooldown)
        {
            int matchCount = 0;
            //check if player has required mana
            for (int i = 0; i < MAX_ELEMENTS; i++)
            {
                if (player.ManaPoints[i] >= ManaCost[i])
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
        CurrentTime = Time.time;
    }

    public virtual void UseSkill(CharacterClass caster)  //used when skill is being used on the caster only
    {
        //get timestamp to start the cooldown
        CurrentTime = Time.time;
    }

    public virtual void UseSkill(CharacterClass caster, CharacterClass[] target) //used against all targets. Probably not using this one yet.
    {
        //get timestamp to start the cooldown
        CurrentTime = Time.time;
    }
}
