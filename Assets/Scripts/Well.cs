using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script will generate blocks. The blocks can be manipulated here.
public class Well : MonoBehaviour
{
    public List<Block> blockList;        //holds all blocks in player well
    public Block blockPrefab;
    public BlockData[] blocks;
    LineRenderer line;                  //used to create new blocks when blocks cross this line

    byte matchCount;        //used to track if there's a match
    bool drawReady;         //used to draw new blocks offscreen.
    public float RiseRate { get; set; }        //used to control how fast blocks rise

    //consts
    const int BLOCK_ROWS = 12;     //the total number of blocks that can fill the well before overflow.
    const int BLOCK_COLS = 6;        //total number of blocks from side to side

    // Start is called before the first frame update
    void Start()
    {
        //Vector3 screenPos = Camera.main.WorldToViewportPoint(GameManager.instance.transform.position);
        //RiseRate = INIT_BLOCK_SPEED;
        blockList = new List<Block>();
        matchCount = 0;
        drawReady = false;

        //create line
        line = new LineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        //blocks rise at a regular rate. New blocks are genereated one line at a time.
    }

    public void RaiseBlocks(float riseRate)
    {
        float yBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.y * 2;  //needed to draw new blocks

        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].transform.position = new Vector2(blockList[i].transform.position.x, blockList[i].transform.position.y + (riseRate * Time.deltaTime));

            //check if i is within the last 6 blocks in the list, which is the bottom row
            if (i >= blockList.Count - BLOCK_COLS && !drawReady)
            {
                if (blockList[i].transform.position.y + blockList[i].GetComponent<SpriteRenderer>().bounds.min.y - 0.5f > yBounds)
                    drawReady = true;
            }
        }

        //when the bottom row of blocks are halfway visible, generate a new row of blocks. Bottom row is always the last 6 blocks in the list.
        if (drawReady == true)
        {
            GenerateBlocks(1, true, -1);
            drawReady = false;
        }

    }

    /* Summary:
     *      Generate rows of blocks.
     * drawBottomToTop: 
     *      if true, blocks are generated starting with the bottom row. */
    public void GenerateBlocks(int rowCount, bool drawBottomToTop = false, int offScreenRowValue = 0)   //offScreenRowValue must be -1 to draw blocks offscreen and line up properly.
    {
        float offset = 0.5f;
        float xBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.x + offset;
        float yBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.y + offset;
        int blockType = 0;          //used for random block generation
        int previousBlockType = -1;  //used to prevent the same colour being used twice in a row

        if (drawBottomToTop == false)
        {
            for (int i = rowCount - 1; i >= 0; i--)    //going from top to bottom
            {
                for (int j = 0; j < BLOCK_COLS; j++)
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
                    Block block = Instantiate(blockPrefab, new Vector2(xBounds + j, yBounds + i), Quaternion.identity);
                    block.transform.parent = transform;     //creates the object as a child of the well object in hierarchy
                    blockList.Add(block);
                }
            }
        }
        else
        {
            if (offScreenRowValue < -1) offScreenRowValue = -1;

            //instantiate blocks starting from bottom of well
            for (int i = 0 + offScreenRowValue; i < rowCount + offScreenRowValue; i++)  
            {
                for (int j = 0; j < BLOCK_COLS; j++)
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
                    Block block = Instantiate(blockPrefab, new Vector2(xBounds + j, yBounds + i), Quaternion.identity);
                    block.transform.parent = transform;     //creates the object as a child of the well object in hierarchy
                    blockList.Add(block);
                }
            }
        }
    }
    public int WellRows() { return BLOCK_ROWS; }
    public int WellCols() { return BLOCK_COLS; }
   

    public List<Block> CopyBlockList(List<Block> listToCopy, int rowCount)
    {
        //ensure the new list is empty
        List<Block> newList = new List<Block>();

        float offset = 0.5f;
        float xBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.x + offset;
        float yBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.y + offset;
        int x = 0;          //iterator
        for (int i = rowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < BLOCK_COLS; j++)
            {
                Block b = Instantiate(listToCopy[x], new Vector2(xBounds + j, yBounds + i), Quaternion.identity);
                b.transform.parent = transform;
                newList.Add(b);
                x++;
            }
        }

        return newList;
    }

    public bool BlocksMatching(Block.BlockType blockType)
    {
        bool isMatching = false;

        //check if there's a horizontal or vertical match. At least 3 blocks must match.

        return isMatching;
    }
}
