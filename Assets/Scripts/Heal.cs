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
    }
}
