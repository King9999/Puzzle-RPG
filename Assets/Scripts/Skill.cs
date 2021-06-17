using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for all skills in the game
public class Skill
{
   protected string Name { get; set; }
   protected int[] ManaCost { get; set; }  //5 different values, one for each colour (element)
   protected float Cooldown { get; set; }   //How much time must pass before player can use skill again

    protected float CurrentTime { get; set; }
    protected enum SkillType
    {
        Active,     
        Passive     //passives have no cost and are always on.
    }

    protected SkillType SkillProperty { get; set; }

    protected enum Element
    {
        Fire,
        Water,
        Earth,
        Light,
        Shadow
    }

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
    protected virtual void UseSkill(CharacterClass caster, CharacterClass target)
    {
        //get timestamp to start the cooldown
        CurrentTime = Time.time;
    }

    protected virtual void UseSkill(CharacterClass caster)  //used when skill is being used on the caster only
    {
        //get timestamp to start the cooldown
        CurrentTime = Time.time;
    }

    protected virtual void UseSkill(CharacterClass caster, CharacterClass[] target) //used against all targets. Probably not using this one yet.
    {
        //get timestamp to start the cooldown
        CurrentTime = Time.time;
    }
}
