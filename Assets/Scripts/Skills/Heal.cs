using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill
{
    void Start()
    {
        skillName = "Heal";
        description = "Restores HP to single target.";
        InitializeCosts(0, 0, 0, 20, 0);
        cooldown = 10;
        multiplier = 2.2f;
        skillElement = Element.Light;
    }

    public override void UseSkill(CharacterClass self)
    {
        if (!CanUseSkill(self))
        {
            Debug.Log("Can't use skill");
            return;
        }
        
        int restoreAmount = (int)(self.elementalPower[(int)skillElement] * multiplier + Random.Range(0, self.elementalPower[(int)skillElement] * 0.25f));
        self.healthPoints += restoreAmount;
        if (self.healthPoints > self.maxHealthPoints)
            self.healthPoints = self.maxHealthPoints;

        //TODO: display the amount of HP restored in game

        Debug.Log("Restored " + restoreAmount + " HP");
        currentTime = Time.time;
    }

    
}
