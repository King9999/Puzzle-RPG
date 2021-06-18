using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Angels are focused on light-elemental skills. */
public class Angel : CharacterClass
{
    public Angel()
    {
        level = 1;
        className = "Angel";
        maxHealthPoints = 120;
        InitializeElementalPower(0, 0, 0, 40, 0);
        InitializeHealth(120);
        InitializeMana(0, 0, 0, 20, 0);
        manaPoints[(int)Element.Light] = 30;

        //add skills
        //Skills.Add(new Heal());
    }

}
