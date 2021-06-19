using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class Block : MonoBehaviour
{
    //public BlockData block;

    [Header("Blocks")]
    public Sprite blockSprite;
    /*public Sprite redBlock;
    public Sprite blueBlock;
    public Sprite greenBlock;
    public Sprite goldBlock;
    public Sprite purpleBlock;
    public Sprite attackBlock;
    public Sprite shieldBlock;
    public Sprite multiBlock;
    public Sprite trashBlock;*/

    //[System.Serializable]
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

    BlockType blockType;

   public void CreateBlock(BlockData block)
    {
        blockSprite = block.blockSprite;
        blockType = block.blockType;
        GetComponent<SpriteRenderer>().sprite = blockSprite;
    }
}
