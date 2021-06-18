using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is the base class for all character classes in the game.
public class CharacterClass : MonoBehaviour
{
    public enum Element
    {
        Fire,       //red
        Water,      //blue
        Earth,      //green
        Light,      //gold
        Shadow      //purple
    }

    //constants
    const int MAX_ELEMENTS = 5;
    const int MAX_XP = 1000;
    const int MAX_HP = 10000;
    const float MAX_ELP = 1000;

    public int Level = 1;          //LV for short
    public string ClassName;
    public int HealthPoints;    //HP
    public int MaxHealthPoints;
    public int[] ManaPoints = new int[MAX_ELEMENTS];     //MP
    public int[] MaxManaPoints = new int[MAX_ELEMENTS];
    public int AttackPower;     //ATP for short
    public int DefensePower;    //DFP
    public float[] ElementalPower = new float[MAX_ELEMENTS]; //ELP. There are five elements.
    public int ExperiencePoints = MAX_XP;  //XP. Max is always 1000 XP



    public List<Skill> Skills = new List<Skill>();     //TODO: should be a collection of Skill objects instead of string

    public void InitializeHealth(int maxHealth)
    {
        MaxHealthPoints = maxHealth;
        HealthPoints = MaxHealthPoints;     
    }

    public void InitializeMana(int maxRedMana, int maxBlueMana, int maxGreenMana, int maxGoldMana, int maxPurpleMana)
    {
        MaxManaPoints[(int)Element.Fire] = maxRedMana;
        MaxManaPoints[(int)Element.Water] = maxBlueMana;
        MaxManaPoints[(int)Element.Earth] = maxGreenMana;
        MaxManaPoints[(int)Element.Light] = maxGoldMana;
        MaxManaPoints[(int)Element.Shadow] = maxPurpleMana;

        for (int i = 0; i < MAX_ELEMENTS; i++)
            ManaPoints[i] = 0;
    }

    public void InitializeElementalPower(float fire, float water, float earth, float light, float shadow)
    {
        ElementalPower[(int)Element.Fire] = fire;
        ElementalPower[(int)Element.Water] = water;
        ElementalPower[(int)Element.Earth] = earth;
        ElementalPower[(int)Element.Light] = light;
        ElementalPower[(int)Element.Shadow] = shadow;
    }


}
