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
    public float riseRate;        //used to control how fast blocks rise

    //consts
    const int WELL_ROWS = 12;     //the total number of blocks that can fill the well before overflow.
    const int WELL_COLS = 6;        //total number of blocks from side to side
    const float INIT_BLOCK_SPEED = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        //Vector3 screenPos = Camera.main.WorldToViewportPoint(GameManager.instance.transform.position);
        riseRate = INIT_BLOCK_SPEED;
        blockList = new List<Block>();
        matchCount = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        //blocks rise at a regular rate. New blocks are genereated one line at a time.
    }

    public void RaiseBlocks(float riseRate)
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            blockList[i].transform.position = new Vector2(blockList[i].transform.position.x, blockList[i].transform.position.y + (riseRate * Time.deltaTime));
        }
    }
    public int WellRows() { return WELL_ROWS; }
    public int WellCols() { return WELL_COLS; }

    //used to setup blocks at start of game. Should be called in game manager.
    public void InitializeBlocks()
    {
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

    public List<Block> CopyBlockList(List<Block> listToCopy)
    {
        //ensure the new list is empty
        List<Block> newList = new List<Block>();

        float offset = 0.5f;
        float xBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.x + offset;
        float yBounds = GetComponentInChildren<SpriteRenderer>().bounds.min.y + offset;
        int x = 0;          //iterator
        for (int i = 0; i < WELL_ROWS / 4; i++)
        {
            for (int j = 0; j < WELL_COLS; j++)
            {
                newList.Add(Instantiate(listToCopy[x], new Vector2(xBounds + j, yBounds + i), Quaternion.identity));
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
