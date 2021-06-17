using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is the base class for all character classes in the game.
public class CharacterClass
{
    public int Level { get; set; }           //LV for short
    public int HealthPoints { get; set; }    //HP
    public int[] ManaPoints { get; set; }      //MP
    public int AttackPower { get; set; }     //ATP for short
    public int DefensePower { get; set; }    //DFP
    public int[] ElementalPower { get; set; }  //ELP. There are five elements.
    public int ExperiencePoints { get; set; }    //XP. Max is always 1000 XP

    public enum Element
    {
        Fire,       //red
        Water,      //blue
        Earth,      //green
        Light,      //gold
        Shadow      //purple
    }

    public List<string> Skills { get; set; }     //TODO: should be a collection of Skill objects instead of string

}
