using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : CharacterClass
{
    void Start()
    {
        className = "Summoner";
        description = "Magic user with the ability to call creatures or objects from the void.";
        InitializeElementalPower(50, 0, 50, 0, 30);
        InitializeHealth(100);
        InitializeMana(40, 0, 35, 0, 10);
        manaPoints[(int)Element.Fire] = 40;
        manaPoints[(int)Element.Earth] = 35;
    }
}
