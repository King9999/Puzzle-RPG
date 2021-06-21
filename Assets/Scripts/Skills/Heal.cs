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
        //cooldown = 10;
        multiplier = 2.2f;
        skillElement = Element.Light;
        skillAction = SkillAction.Heal;
    }

    public override void UseSkill(CharacterClass self)
    {
        /*if (!CanUseSkill(self))
        {
            Debug.Log(skillName + " not ready");
            return;
        }*/
        
        int restoreAmount = (int)(self.elementalPower[(int)skillElement] * multiplier + Random.Range(0, self.elementalPower[(int)skillElement] * 0.15f));
        self.healthPoints += restoreAmount;
        if (self.healthPoints > self.maxHealthPoints)
            self.healthPoints = self.maxHealthPoints;

        //TODO: display the amount of HP restored in game
        GameUI.instance.skillActivated[GameUI.instance.Heal] = true;
        GameUI.instance.healText.text = restoreAmount.ToString();
        //GameUI.instance.StartCoroutine(GameUI.instance.DisplayDamageText(GameUI.instance.healText));

        //Debug.Log("Restored " + restoreAmount + " HP");
        currentTime = Time.time;
    }

    
}
