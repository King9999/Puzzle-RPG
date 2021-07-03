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
    float vy = 0;                   //controls how fast block falls.

    //constant
    float FallSpeed { get; } = -7f;
   
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

    //Checks if block is on top of another block. If it returns false, then block falls.
    public bool IsTouchingBlock()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.35f, 0), Vector2.down, 0.2f);

        return hit.collider != null;
    }

    private void Update()
    {
        if (IsTouchingBlock())
            vy = 0;
        else /*if (!IsTouchingBlock() && blockType != BlockType.Null)*/
            vy = FallSpeed;

        transform.position = new Vector2(transform.position.x, transform.position.y + vy * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
