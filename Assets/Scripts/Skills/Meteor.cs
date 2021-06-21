using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Skill
{
    bool isCoroutineRunning = false;

   void Start()
    {
        skillName = "Meteor Rain";
        description = "Summons a hail of fiery meteors.";
        InitializeCosts(0, 0, 0, 0, 0);
        //cooldown = 10;
        multiplier = 0.12f;
        hitCount = 8;
        skillElement = Element.Fire;
        skillAction = SkillAction.Attack;
        currentTime = 0;
    }

    //this skill strikes multiple times. Needs a coroutine to handle each hit.
    public override void UseSkill(CharacterClass caster, CharacterClass target)
    {
        if (!CanUseSkill(caster))
        {
            Debug.Log("Can't cast " + skillName);
            return;
        }

        if (!isCoroutineRunning)
           StartCoroutine(DealDamage(caster, target, hitCount));

       
       /* int damage = (int)(caster.elementalPower[(int)skillElement] * multiplier + Random.Range(0, caster.elementalPower[(int)skillElement] * 0.1f));

        target.healthPoints -= damage;
        if (target.healthPoints < 0)
            target.healthPoints = 0;

        //TODO: display the amount of HP lost in game
     
        Debug.Log(damage + " damage dealt ");*/
          
    }

    IEnumerator DealDamage(CharacterClass caster, CharacterClass target, int hitCount)
    {
        isCoroutineRunning = true;
        currentTime = Time.time;
        GameUI.instance.skillActivated[GameUI.instance.Damage] = true;

        int i = 0;
        while (i < hitCount)
        {
            int damage = (int)(caster.elementalPower[(int)skillElement] * multiplier + Random.Range(0, caster.elementalPower[(int)skillElement] * 0.1f));

            target.healthPoints -= damage;
            if (target.healthPoints < 0)
                target.healthPoints = 0;

            //TODO: display the amount of HP lost in game
            GameUI.instance.damageTextList.Add(Instantiate(GameUI.instance.damageText, GameUI.instance.transform));
            GameUI.instance.damageTextList[GameUI.instance.damageTextList.Count - 1].text = damage.ToString();
            GameUI.instance.UpdateHealthUI(target, -damage);
            i++;
            //Debug.Log(damage + " damage dealt ");
            yield return new WaitForSeconds(0.2f);
        }

        //skill effect over
        isCoroutineRunning = false;
    }
}
