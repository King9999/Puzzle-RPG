using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class Block : MonoBehaviour
{
    //public BlockData block;

    [Header("Blocks")]
    public Sprite blockSprite;
    public int blockID;         //each block will have a unique number. Makes it easy to locate and destroy it.
    
   
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
        Trash,       //cannot be moved
        Null = -1         //invisible and doesn't interact with other blocks, except to switch places with other blocks.
    }

    public BlockType blockType;

   public void CreateBlock(BlockData block)
    {
        blockSprite = block.blockSprite;
        blockType = block.blockType;
        GetComponent<SpriteRenderer>().sprite = blockSprite;
    }

    public void NullifyBlock(Block b)
    {
        b.blockType = BlockType.Null;
        b.GetComponent<SpriteRenderer>().enabled = false;
        b.GetComponent<BoxCollider2D>().enabled = false;
        b = null;
    }
}
