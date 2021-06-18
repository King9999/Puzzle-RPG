using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Angels are focused on light-elemental skills. */
public class Angel : CharacterClass
{
    void Start()
    {
        className = "Angel";
        description = "A divine being with an assortment of light-based magic.";
        InitializeElementalPower(0, 0, 0, 40, 0);
        InitializeHealth(120);
        InitializeMana(0, 0, 0, 30, 0);
        manaPoints[(int)Element.Light] = 30;
    }

}
