using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Skill
{
    public Heal()
    {
        Name = "Heal";
        ManaCost[(int)Element.Fire] = 0;
        ManaCost[(int)Element.Water] = 0;
        ManaCost[(int)Element.Earth] = 0;
        ManaCost[(int)Element.Light] = 20;
        ManaCost[(int)Element.Shadow] = 0;
        Cooldown = 10;
        Multiplier = 2.2f;
        SkillElement = Element.Light;
    }

    public override void UseSkill(CharacterClass self)
    {
        if (!CanUseSkill(self))
        {
            Debug.Log("Can't use skill");
            return;
        }
        
        int restoreAmount = (int)(self.ElementalPower[(int)SkillElement] * Multiplier + Random.Range(0, self.ElementalPower[(int)SkillElement] * 0.25f));
        self.HealthPoints += restoreAmount;
        if (self.HealthPoints > self.MaxHealthPoints)
            self.HealthPoints = self.MaxHealthPoints;

        //TODO: display the amount of HP restored in game

        Debug.Log("Restored " + restoreAmount + " HP");
        CurrentTime = Time.time;
    }

    
}
