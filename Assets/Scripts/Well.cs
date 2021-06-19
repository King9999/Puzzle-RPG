using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script will generate blocks. The blocks can be manipulated here.
public class Well : MonoBehaviour
{
    public List<Block> blockList;        //holds all blocks in player well
    public Block blockPrefab;
    public BlockData[] blocks;

    byte matchCount;        //used to track if there's a match
    Vector2 speedVector;    //used to control how fast blocks rise

    //consts
    const int WELL_ROWS = 12;     //the total number of blocks that can fill the well before overflow.
    const int WELL_COLS = 6;        //total number of blocks from side to side
    const float INIT_BLOCK_SPEED = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        //Vector3 screenPos = Camera.main.WorldToViewportPoint(GameManager.instance.transform.position);
        speedVector = new Vector2(0, INIT_BLOCK_SPEED);
        blockList = new List<Block>();
        matchCount = 0;
        

        //fill well with blocks TODO: don't fill all rows completely at the top
        float offset = 0.5f;
        float xBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.x + offset;
        float yBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.y + offset;
        int blockType = 0;          //used for random block generation
        int previousBlockType = -1;  //used to prevent the same colour being used twice in a row
        for (int i = 0; i < WELL_ROWS / 4; i++)
        {
            for (int j = 0; j < WELL_COLS; j++)
            {
                //randomize block
                blockType = Random.Range((int)Block.BlockType.Red, (int)Block.BlockType.Purple + 1);

                //check if either the previous colour or the block 6 blocks ahead are the same colour.
                while (previousBlockType == blockType)
                {
                    //ensure no two blocks are the same colour
                    blockType = Random.Range((int)Block.BlockType.Red, (int)Block.BlockType.Purple + 1);
                }

                previousBlockType = blockType;

                blockPrefab.CreateBlock(blocks[blockType]);
                blockList.Add(Instantiate(blockPrefab, new Vector2(xBounds + j, yBounds + i), Quaternion.identity));
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool BlocksMatching(Block.BlockType blockType)
    {
        bool isMatching = false;

        //check if there's a horizontal or vertical match. At least 3 blocks must match.

        return isMatching;
    }
}
