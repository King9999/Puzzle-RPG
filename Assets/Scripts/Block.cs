using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Blocks")]
    public Sprite redBlock;
    public Sprite blueBlock;
    public Sprite greenBlock;
    public Sprite goldBlock;
    public Sprite purpleBlock;
    public Sprite attackBlock;
    public Sprite shieldBlock;
    public Sprite multiBlock;
    public Sprite trashBlock;

    public enum BlockType
    {
        Red,        //fire
        Blue,       //water
        Green,      //earth
        Gold,       //light
        Purple,     //shadow
        Attack,
        Shield,
        Multi,       //gives player mana of all colours
        Trash       //cannot be moved
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
