using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is the base class for all character classes in the game.
public abstract class CharacterClass
{
    protected int Level { get; set; }           //LV for short
    protected int HealthPoints { get; set; }    //HP
    protected int ManaPoints { get; set; }      //MP
    protected int AttackPower { get; set; }     //ATP for short
    protected int DefensePower { get; set; }    //DFP
    protected int[] ElementalPower { get; set; }  //ELP. There are five elements.
    protected int ExperiencePoints { get; set; }    //XP. Max is always 1000 XP

    protected enum Element
    {
        Fire,       //red
        Water,      //blue
        Earth,      //green
        Light,      //gold
        Shadow      //purple
    }

}
