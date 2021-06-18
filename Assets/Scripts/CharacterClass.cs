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

    public int level = 1;          //LV for short
    public string className;
    public int healthPoints;    //HP
    public int maxHealthPoints;
    public int[] manaPoints = new int[MAX_ELEMENTS];     //MP
    public int[] maxManaPoints = new int[MAX_ELEMENTS];
    public int attackPower;     //ATP for short
    public int defensePower;    //DFP
    public float[] elementalPower = new float[MAX_ELEMENTS]; //ELP. There are five elements.
    public int ExperiencePoints = MAX_XP;  //XP. Max is always 1000 XP



    public List<Skill> Skills = new List<Skill>();     //TODO: should be a collection of Skill objects instead of string

    public void InitializeHealth(int maxHealth)
    {
        if (maxHealth > MAX_HP)
            maxHealth = MAX_HP;
        if (maxHealth < 1)
            maxHealth = 1;

        maxHealthPoints = maxHealth;
        healthPoints = maxHealthPoints;     
    }

    public void InitializeMana(int maxRedMana, int maxBlueMana, int maxGreenMana, int maxGoldMana, int maxPurpleMana)
    {
        maxManaPoints[(int)Element.Fire] = maxRedMana;
        maxManaPoints[(int)Element.Water] = maxBlueMana;
        maxManaPoints[(int)Element.Earth] = maxGreenMana;
        maxManaPoints[(int)Element.Light] = maxGoldMana;
        maxManaPoints[(int)Element.Shadow] = maxPurpleMana;

        for (int i = 0; i < MAX_ELEMENTS; i++)
            manaPoints[i] = 0;
    }

    public void InitializeElementalPower(float fire, float water, float earth, float light, float shadow)
    {
        elementalPower[(int)Element.Fire] = fire;
        elementalPower[(int)Element.Water] = water;
        elementalPower[(int)Element.Earth] = earth;
        elementalPower[(int)Element.Light] = light;
        elementalPower[(int)Element.Shadow] = shadow;
    }


}
