using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public CharacterClass[] player;
    //public CharacterClass target;
    public Well[] playerWells;
    public Cursor[] cursors;
    List<int> idList;                   //keeps record of block IDs that match for destruction later.
    List<int> comboEnderList;           //tracks which blocks end a match combo so the combo counter can be reset.
    public byte comboCounter;           //counter for matches of 4 or greater
    public byte chainCounter;           //counter for chains of 2 or greater

    public float riseRate;                  //controls the rate at which blocks rise. Rate is in seconds.
    public float riseValue;                 //how much the blocks rise.
    float riseTotal = 0;                //whenever this is 1, increases row count by 1.
    float[] currentTime;                      //timestamp
    [HideInInspector] public int blockID;   //assigns unique block IDs when a new block is generated.

    //constants
    const int MAX_BLOCKTYPES = 9;
    const int PLAYER_WELL_1 = 0;
    const int PLAYER_WELL_2 = 1;
    const float INIT_RISE_RATE = 1f;
    public float InitRiseRate { get; } = 1f; 
    const float INIT_RISE_VALUE = 0.05f;
    
    public int PlayerOne { get; } = 0;
    public int PlayerTwo { get; } = 1;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(34472);        //TODO: Used for testing block matches. Must be removed when game is ready.
        blockID = 0;
        
        //ensure both wells have the same initial blocks. Any blocks generated afterwards can be different.
        playerWells[PlayerOne].GenerateBlocks(3);
        playerWells[PlayerTwo].blockList = playerWells[PlayerTwo].CopyBlockList(playerWells[PlayerOne].blockList, 3);

        //cursor set up
        for (int i = 0; i < cursors.Length; i++)
        {
            int row = cursors[i].CurrentRow;
            int col = cursors[i].CurrentCol;
            float xOffset = -2.5f;
            float yOffset = -6.5f;
            cursors[i].transform.position = new Vector3(playerWells[i].transform.position.x + col + xOffset, playerWells[i].transform.position.y + row + yOffset, cursors[i].Z_Value);
        }

        //rise rate setup
        riseRate = InitRiseRate;
        riseValue = INIT_RISE_VALUE;

        foreach (Well well in playerWells)
        {
            SetRiseRate(well, riseRate);
            well.RiseValue = riseValue;
        }

        currentTime = new float[2];
        for (int i = 0; i < currentTime.Length; i++)
            currentTime[i] = 0;
       


        //player set up
        player[PlayerOne].playerID = PlayerOne;
        GameUI.instance.playerNameText[PlayerOne].text = player[PlayerOne].className;
        GameUI.instance.healthPointText[PlayerOne].text = player[PlayerOne].healthPoints + "/" + player[PlayerOne].maxHealthPoints;
        GameUI.instance.healthBars[PlayerOne].SetMaxValue(player[PlayerOne].healthPoints);


        player[PlayerTwo].playerID = PlayerTwo;
        GameUI.instance.playerNameText[PlayerTwo].text = player[PlayerTwo].className;
        GameUI.instance.healthPointText[PlayerTwo].text = player[PlayerTwo].healthPoints + "/" + player[PlayerTwo].maxHealthPoints;
        GameUI.instance.healthBars[PlayerTwo].SetMaxValue(player[PlayerTwo].healthPoints);
        cursors[PlayerTwo].isAIControlled = true;

        //list set up
        idList = new List<int>();
        comboEnderList = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {

        //raise blocks and cursors
        for (int i = 0; i < playerWells.Length; i++)
        {
            if (Time.time > currentTime[i] + playerWells[i].RiseRate)
            {               
                playerWells[i].RaiseBlocks(playerWells[i].RiseValue);
                cursors[i].transform.position = new Vector3(cursors[i].transform.position.x, cursors[i].transform.position.y + riseValue, cursors[i].Z_Value);

                //must adjust cursor row position since cursors are always rising.
                if (i == PlayerOne)
                {
                    riseTotal += riseValue;
                    if (riseTotal >= 1)
                    {
                        riseTotal = 0;
                        if (cursors[i].CurrentRow >= cursors[i].TotalRows - 1)
                        {
                            //we're at the top of the well; reduce row position and screen position.
                            cursors[i].CurrentRow--;
                            cursors[i].transform.position = new Vector3(cursors[i].transform.position.x, cursors[i].transform.position.y - 2, cursors[i].Z_Value);
                        }
                        else
                        {
                            cursors[PlayerOne].CurrentRow++;
                        }
                        Debug.Log("Row is now " + cursors[PlayerOne].CurrentRow);
                    }
                }
                currentTime[i] = Time.time;
            }
        }

        //update cursor positions       
       /*for (int i = 0; i < cursors.Length; i++)
       {
            int currentIndex = cursors[i].CurrentIndex;
            int row = cursors[i].CurrentRow;
            int col = cursors[i].CurrentCol;
            float xOffset = -2.5f;
            float yOffset = -6.4f;
            //cursors[i].transform.position = new Vector3(playerWells[i].blockList[currentIndex].transform.position.x, playerWells[i].blockList[currentIndex].transform.position.y, cursors[i].Z_Value);
            //cursors[i].transform.position = new Vector3(playerWells[i].transform.position.x + col + xOffset, playerWells[i].transform.position.y + row + yOffset, cursors[i].Z_Value);
       }*/

        //check for block matches, both vertical and horizontal
        CheckForMatches(playerWells[PlayerOne]);

        //remove any matching blocks
        if (idList.Count > 0)
        {
            RemoveMatchingBlocks(playerWells[PlayerOne]);           
        }

        //check for any blocks that should be falling
        for (int i = 0; i < playerWells[PlayerOne].blockList.Count; i++)
        {
            Well playerWell = playerWells[PlayerOne];

            int totalCols = playerWells[PlayerOne].TotalCols;
            //int colIterator = playerWell.blockList.Count - totalCols + i;    //iterates through the blocklist by column, starting from bottom.
            //Block block = playerWells[PlayerOne].blockList[colIterator];
            int nullCount = 0;              //tracks the number of null blocks

            //I take care to check if we're on the topmost row because there won't be any blocks to drop.
            if (playerWell.blockList[i].row < playerWell.RowDepth - 1 && playerWell.blockList[i].blockType == Block.BlockType.Null)
            {
                for (int j = i; j >= 0; j -= totalCols)
                {
                    int indexOfBlockAbove = j - totalCols;

                    if (indexOfBlockAbove < 0)
                        break; //can't proceed as we've reached topmost row.

                    if (playerWell.blockList[indexOfBlockAbove].blockType != Block.BlockType.Null)
                    {
                        //shift this block down to the null block.
                        Block tempBlock = playerWell.blockList[j];
                        Vector2 tempPos = new Vector2(tempBlock.transform.position.x, tempBlock.transform.position.y);
                        playerWell.blockList[j].transform.position = playerWell.blockList[indexOfBlockAbove].transform.position;
                        playerWell.blockList[j] = playerWell.blockList[indexOfBlockAbove];
                        playerWell.blockList[indexOfBlockAbove].transform.position = tempPos;
                        playerWell.blockList[indexOfBlockAbove] = tempBlock;
                        //playerWell.blockList[indexOfBlockAbove].NullifyBlock(/*playerWell.blockList[indexOfBlockAbove]*/);
                        //Debug.Log("New Block: " + playerWell.blockList[j].blockType);
                    }
                }
            }

            //start from the bottom of each column and check each block to see if it's null
            /*while (colIterator >= 0)
            {
                //if null, then all blocks above it must fall. Use for loop here
                if (colIterator >= 0 && playerWell.blockList[colIterator].blockType == Block.BlockType.Null)
                {
                    for (int j = playerWell.blockList[colIterator].row; j < playerWell.RowDepth; j++)
                    {
                        if (playerWell.blockList[colIterator - totalCols].blockType != Block.BlockType.Null)
                        {
                            //shift this block down to the null block.
                            playerWell.blockList[colIterator - totalCols].transform.position = playerWell.blockList[colIterator].transform.position;
                            playerWell.blockList[colIterator] = playerWell.blockList[colIterator - totalCols];
                            playerWell.blockList[colIterator - totalCols].NullifyBlock(playerWell.blockList[colIterator - totalCols]);
                        }
                        colIterator -= totalCols;
                    }
                }
                else
                {
                    colIterator -= totalCols;
                }
            }*/





            /*while (colIterator < playerWell.blockList.Count)
            {
                Block block = playerWell.blockList[colIterator];
                if (playerWell.BlockIsFalling(block))
                {
                    colIterator += playerWells[PlayerOne].TotalCols;
                    //count number of null blocks in the column
                    //while (colIterator < playerWell.blockList.Count)
                    //{
                        if (playerWells[PlayerOne].blockList[colIterator].blockType == Block.BlockType.Null)
                            nullCount++;
                        //colIterator += playerWells[PlayerOne].TotalCols;
                    //}

                    //reduce block's y pos by 1 for each null block
                    block.transform.position = new Vector2(block.transform.position.x, block.transform.position.y - nullCount);
                    block.row -= nullCount;

                    //update block's position in list.
                    Block temp = block;
                    //Vector2 tempPos = new Vector2(temp.transform.position.x, temp.transform.position.y);    //need a separate copy of temp position so we don't refer directly to the temp object's position
                    //block.transform.position = new Vector2(block.transform.position.x, block.transform.position.y - nullCount);
                    int newRow = (playerWell.TotalCols * block.row) + block.col;
                    block = playerWell.blockList[newRow];

                    //playerWell.blockList[newRow].transform.position = tempPos;
                    //playerWell.blockList[newRow] = null;
                }
                else
                {
                    colIterator += playerWells[PlayerOne].TotalCols;
                }
            }*/

            /*if (playerWells[PlayerOne].BlockIsFalling(block))
            {
                //count number of null blocks in the column
               
                   
                    //reduce block's y pos by 1 for each null block
                    //do this for each non-null block in column
                    //update block's position in list.

                    //set the block's position to the block below
                    int indexOfBlockBelow = (playerWells[PlayerOne].TotalCols * (block.row - 1)) + block.col;
                //Block tempBlock = playerWells[PlayerOne].blockList[indexOfBlockBelow];
                block.transform.position = playerWells[PlayerOne].blockList[indexOfBlockBelow].transform.position;
                playerWells[PlayerOne].blockList[indexOfBlockBelow] = block;
                block = null;

                //drop the block until it lands on another block
                //block.transform.position = new Vector2(block.transform.position.x, block.transform.position.y - 1 * Time.deltaTime);
                Debug.Log("Block at [" + block.row + "," + block.col + "] is falling");
            }*/
        }

    }

    public void SetRiseRate(Well well, float value)
    {
       well.RiseRate = value;
    }
    private void RemoveMatchingBlocks(Well playerWell)
    {
        byte comboCounter = 0;
        //TODO: Can I use lambda expression to remove all the matching blocks?
        for (int i = 0; i < playerWell.blockList.Count; i++)
        {
            if (idList.Count <= 0)
                break;     //nothing else to match, quit

            if (playerWell.blockList[i] == null)
                continue;

            int j = 0;
            while (j < idList.Count)
            {
                
                if (playerWell.blockList[i].blockID == idList[j])
                {
                    //delete this block from the well
                    //Destroy(playerWell.blockList[j].gameObject);
                    //playerWell.blockList[j] = null;
                    comboCounter++;
                    playerWell.blockList[i].NullifyBlock(/*playerWell.blockList[i]*/);
                    //Debug.Log("Block ID " + playerWell.blockList[i].blockID);
                    
                    

                    //are we at the end of the combo?
                    foreach (int endValue in comboEnderList)
                    {
                        if (endValue == idList[j])
                        {
                            //combo ends here, display combo Counter to screen
                            Debug.Log("Combo Counter: " + comboCounter);
                            //reset combo value afterwards
                            comboCounter = 0;
                         
                        }
                    }
                    idList.Remove(idList[j]);
                    break;

                }
                else
                {
                    j++;
                }
            }
           
        }
        //block removal complete
        idList.Clear();
        comboEnderList.Clear();
        idList.Capacity = 0;
        comboEnderList.Capacity = 0;

        //scan all rows. If a row consists only of null blocks, remove it.
        //for (int i = 0; )
    }

    public void CheckForMatches(Well playerWell)
    {
        //cannot proceed with this method in certain conditions
        if (playerWell.blockList.Count < 3 || playerWell.RowDepth < 3)
            return;
  
        int matchCount = 0;                 //tracks how many matches were made between different block types
        bool hMatchFound = false;
        bool currentBlockHMatching = false;         //used for when the current block breaks an existing match combo.
        List<Block> hMatchList = new List<Block>();

        //add the first two blocks to match list, we'll need them for comparison later.
        if (playerWell.blockList[0] != null)
            hMatchList.Add(playerWell.blockList[0]);
        if (playerWell.blockList[1] != null)
            hMatchList.Add(playerWell.blockList[1]);
        int x = hMatchList.Count - 1;                              //iterator for hMatchList

       

        for (int i = hMatchList.Count; i < playerWell.blockList.Count; i++)
        {
            //add current block for comparison
            if (playerWell.blockList[i] == null)
                continue;

            hMatchList.Add(playerWell.blockList[i]);
            x++;
            
            //check if the current block is on a new row, as we don't want to compare it against blocks on a different row.
            if (x % playerWell.TotalCols == 0)
            {
                if (i + 2 < playerWell.blockList.Count)
                {
                    hMatchList.Add(playerWell.blockList[i + 1]);
                    hMatchList.Add(playerWell.blockList[i + 2]);
                    i += 2;
                    x += 2;
                }
            }

            //horizontal check
            if (hMatchList.Count < 3)
                continue;   //not enough blocks, add next block.

            if (hMatchList[x].blockType != Block.BlockType.Null && 
                hMatchList[x].blockType == hMatchList[x - 1].blockType && hMatchList[x].blockType == hMatchList[x - 2].blockType)
            {
                currentBlockHMatching = true;
                //we have a horizontal match
                if (!hMatchFound)
                {
                    matchCount++;
                    //track the IDs of the blocks that match
                    for (int k = 0; k < 3; k++)
                        idList.Add(hMatchList[x - k].blockID);
                    hMatchFound = true;
                }
                else
                {
                    idList.Add(hMatchList[x].blockID);  //This block is part of an ongoing match, i.e. a combo
                }
               
            }
            else
            {
                currentBlockHMatching = false;
            }

            if (!hMatchFound)
            {
                //remove the leftmost block as we no longer need it.
                hMatchList.RemoveAt(x - 2);
                x--;
            }
            else if (hMatchFound && !currentBlockHMatching)
            {
                //we found a previous match but the current block doesn't match, so we start a new comparison by adding the next two blocks to
                //the match list. The second block should get added on the next iteration.

                comboEnderList.Add(hMatchList[x - 1].blockID);  //we get the previously matched block for combo counting later.

                if (i + 2 < playerWell.blockList.Count)
                {
                    hMatchList.Add(playerWell.blockList[i + 1]);
                    i++;
                    x++;
                }
                hMatchFound = false;
            }

        }


        //if we're on the last block and there's no match, delete the last 2 blocks. These 2 blocks do not
        //match with anything and can be safely removed.
        if (x >= hMatchList.Count - 1 && !currentBlockHMatching)
        {
            //delete current block.
            hMatchList.RemoveAt(hMatchList.Count - 1);
            if (!hMatchFound)
            {
                //also remove the second last block since we had a match previously and don't want to remove a matching block.
                hMatchList.RemoveAt(hMatchList.Count - 1);
            }
        }

        /******VERTICAL MATCH CHECK**********/
        //add the current block to match list, then the block 6 blocks ahead from the current. Repeat until we reach end of block list.
        //It makes sense when you look at the blocks in game and see what it takes to check for vertical matches.

        int vMatchCount = 0;                //tracks how many times a vertical match was found. Does not include matches of 4 or more.
        const int COLS = 6;                 //used for vertical matching
        List<Block> vMatchList = new List<Block>();

        const int INIT_INDEX = 2;
        int y = INIT_INDEX;              //y always begins at 2 because we always compare a block against the previous two blocks in match list.
        int matchingBlockCount = 0;     //counts the number of blocks that match so that y can be re-positioned correctly in the match list.

        for (int i = 0; i < COLS; i++)
        {
            int colIterator = i;            //iterates through the blocklist to add blocks to match list.
            int currentColBlockCount = 0;   //tracks number of blocks in a column. Must not be less than 3.          
            bool vMatchFound = false;            //we're on a new column so this must be reset.
            bool currentBlockVMatching = false;  //this too

            //Once we've reached the end of the previous column, we need to reset y as it will be out of bounds.
            //Also need to clear the match list if necessary to prevent invalid matches with old columns.
            if (y >= vMatchList.Count - 1)
            {
                if (matchingBlockCount > 0)
                    y = INIT_INDEX + matchingBlockCount; //we advance by the number of matched blocks to avoid comparing any matched blocks.
                else
                {
                    //clear the entire match list
                    vMatchList.Clear();
                    y = INIT_INDEX;
                }
            }

            while (colIterator < playerWell.blockList.Count)
            {
                if (playerWell.blockList[colIterator] != null)
                {
                    vMatchList.Add(playerWell.blockList[colIterator]);
                    currentColBlockCount++;
                }
                colIterator += COLS;
                             
            }

            if (currentColBlockCount < 3)
            {
                //delete the blocks that were added as vertical match is impossible in current column
                int j = 0;
                while (j < currentColBlockCount)
                {
                    vMatchList.RemoveAt(vMatchList.Count - 1);
                    j++;
                }
                continue;
            }

            //loop through all blocks in column
            while (y < vMatchList.Count) 
            {
                if (y < INIT_INDEX)  
                    y = INIT_INDEX; //we must always be comparing three blocks, starting from the 3rd.

                //compare block that y points to against the previous two blocks
                if (vMatchList[y].blockType != Block.BlockType.Null &&
                    vMatchList[y].blockType == vMatchList[y - 1].blockType && vMatchList[y].blockType == vMatchList[y - 2].blockType)
                {
                    currentBlockVMatching = true;
                    //we have a vertical match
                    if (!vMatchFound)
                    {
                        vMatchCount++;
                        matchingBlockCount += 3;    //at least 3 blocks must match.
                        //track the IDs of the blocks that match
                        for (int k = 0; k < 3; k++)
                            idList.Add(vMatchList[y - k].blockID);
                        vMatchFound = true;
                    }
                    else
                    {
                        idList.Add(vMatchList[y].blockID);
                        matchingBlockCount++;   //we matched previously, so we just add 1 more block that matched.
                    }
                }
                else
                {
                    currentBlockVMatching = false;
                }

                //if there's no match, delete the block at beginning of list and advance y by 1
                if (!vMatchFound)
                {
                    //remove the leftmost block as we no longer need it.
                    vMatchList.RemoveAt(y - 2);
                    y = matchingBlockCount + INIT_INDEX;    //we set y in a way that it avoids any matching blocks in match list.
                }
                else if (vMatchFound && !currentBlockVMatching)
                {
                    //we found a previous match but the current block doesn't match, so we start a new comparison to avoid the matched blocks by advancing y 2 times.
                    comboEnderList.Add(vMatchList[y - 1].blockID);  //grab the previously matched block's ID for combo counting later.
                    y += 2;
                    vMatchFound = false;
                }
                else
                {
                    y++; //we currently have a match, so we want to see if the next block is a match (combo).
                }

              
            }          

        }


        if (vMatchCount > 0)
        {
            //Due to how the vertical match check is performed, we always check if the last 2 blocks are part of
            //a match. They're removed if their IDs are not in the ID list.

            for (int i = 0; i < 2; i++)
            {
                bool idFound = false;
                foreach (int id in idList)
                {
                    if (id == vMatchList[vMatchList.Count - (i + 1)].blockID)
                    {
                        //value is in the list
                        idFound = true;
                        break;
                    }

                }
                //remove block if its ID is not in the ID list.
                if (!idFound)
                {
                    vMatchList.RemoveAt(vMatchList.Count - (i + 1));
                    i--;
                }
            }


        }
        else
        {
            //clear the list, no match found
            vMatchList.Clear();
        }

        //once we get here, there should only be matching blocks in the lists. Clear all horizontal and vertical matches
        //hMatchList.TrimExcess();
        //vMatchList.TrimExcess();

        //if (matchCount > 0)
        //{
            /*string list = "Horizontal matches: ";
            //Debug.Log("Total matches: " + matchCount);
            foreach (Block b in hMatchList)
            {
                if (b == null)
                    continue;
                list += b.blockType + ", ";
            }
            Debug.Log(list);

            list = "Vertical matches: ";
            foreach (Block b in vMatchList)
            {
                if (b == null)
                    continue;
                list += b.blockType + ", ";
            }
            Debug.Log(list);*/
        //}
    }

        

    //left mouse button action will be context sensitive
    public void OnLeftMouseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //player[PlayerOne].skills[0].UseSkill(player[PlayerOne]);
            //player[PlayerTwo].skills[0].UseSkill(player[PlayerTwo], player[PlayerOne]);
            //int totalCols = playerWells[PlayerOne].TotalCols;
            //int currentIndex = (totalCols * cursors[PlayerOne].CurrentRow) + cursors[PlayerOne].CurrentCol;
            Block[,] b = cursors[PlayerOne].GetBlocks(playerWells[PlayerOne].blockList);

            for (int i = 0; i < b.GetLength(0); i++)
                for (int j = 0; j < b.GetLength(1); j++)
                    Debug.Log(b[i,j].blockType + ": " + b[i, j].transform.position);

        }
    }
}
