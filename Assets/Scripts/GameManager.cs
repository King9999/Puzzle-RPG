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

    public float riseRate;              //controls the rate at which blocks rise. Rate is in seconds.
    public float riseValue;             //how much the blocks rise.
    float currentTime;                  //timestamp
    [HideInInspector] public int blockID;   //assigns unique block IDs when a new block is generated.

    //constants
    const int MAX_BLOCKTYPES = 9;
    const int PLAYER_WELL_1 = 0;
    const int PLAYER_WELL_2 = 1;
    const float INIT_RISE_RATE = 1f;
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
        blockID = 0;
        riseRate = INIT_RISE_RATE;
        riseValue = INIT_RISE_VALUE;
        currentTime = 0;
        //ensure both wells have the same initial blocks. Any blocks generated afterwards can be different.
        //playerWells[PLAYER_WELL_1].InitializeBlocks();
        playerWells[PLAYER_WELL_1].GenerateBlocks(3);
        playerWells[PLAYER_WELL_2].blockList = playerWells[PLAYER_WELL_2].CopyBlockList(playerWells[PLAYER_WELL_1].blockList, 3);

        foreach (Well well in playerWells)
            well.RiseValue = riseValue;

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
    }

    // Update is called once per frame
    void Update()
    {
        //raise blocks
        if (Time.time > currentTime + riseRate)
        {
            for (int i = 0; i < playerWells.Length; i++)
            {
                playerWells[i].RaiseBlocks(playerWells[i].RiseValue);
                cursors[i].transform.position = new Vector3(cursors[i].transform.position.x, cursors[i].transform.position.y + riseValue, cursors[i].Z_Value);
            }

            currentTime = Time.time;
        }

        //update cursor positions       
       for (int i = 0; i < cursors.Length; i++)
        {
            //int totalCols = playerWells[i].TotalCols;
            int currentIndex = cursors[i].CurrentIndex;
            //int currentIndex = (totalCols * cursors[i].CurrentRow) + cursors[i].CurrentCol; //converts index of a 2D array or list to an equal index from a linear list/array.
            cursors[i].transform.position = new Vector3(playerWells[i].blockList[currentIndex].transform.position.x, playerWells[i].blockList[currentIndex].transform.position.y, cursors[i].Z_Value);
        }

        //check for block matches, both vertical and horizontal
        //CheckForMatches(playerWells[PlayerOne]);
    }

    public void CheckForMatches(Well playerWell)
    {
        //cannot proceed with this method in certain conditions
        if (playerWell.blockList.Count < 3 && playerWell.RowDepth < 3)
            return;
  
        int matchCount = 0;                 //tracks how many matches were made between different block types
        bool hMatchFound = false;
        bool currentBlockHMatching = false;         //used for when the current block breaks an existing match combo.
        List<Block> hMatchList = new List<Block>();

        //add the first two blocks to match list, we'll need them for comparison later.
        hMatchList.Add(playerWell.blockList[0]);
        hMatchList.Add(playerWell.blockList[1]);
        int x = hMatchList.Count - 1;                              //iterator for hMatchList

       

        for (int i = hMatchList.Count; i < playerWell.blockList.Count; i++)
        {
            //add current block for comparison
            hMatchList.Add(playerWell.blockList[i]);
            x++;
           

            //horizontal check
            if (hMatchList.Count < 3)
                continue;   //not enough blocks
            if (hMatchList[x].blockType == hMatchList[x - 1].blockType && hMatchList[x].blockType == hMatchList[x - 2].blockType)
            {
                currentBlockHMatching = true;
                //we have a horizontal match
                if (!hMatchFound)
                {
                    matchCount++;
                    hMatchFound = true;
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

        bool currentBlockVMatching = false;
        bool vMatchFound = false;
        const int COLS = 6;                 //used for vertical matching
        List<Block> vMatchList = new List<Block>();

        const int INIT_INDEX = 2;
        int y = INIT_INDEX;              //y always begins at 2 because we always compare a block against the previous two blocks in match list.
        for (int i = 0; i < COLS; i++)
        {
            int colIterator = i;            //iterates through the blocklist to add blocks to match list.
            int currentColBlockCount = 0;   //tracks number of blocks in a column. Must not be less than 3.          
            int matchingBlockCount = 0;     //counts the number of blocks that match so that y can be re-positioned correctly in the match list.

            //reset y to prevent out of bounds errors.
            if (y >= vMatchList.Count - 1)
            {
                if (matchingBlockCount > 0)
                    y = matchingBlockCount + 2; //we advance 2 ahead to avoid comparing any matched blocks.
                else
                    y = INIT_INDEX;
            }

            while (colIterator < playerWell.blockList.Count)
            {
                vMatchList.Add(playerWell.blockList[colIterator]);
                colIterator += COLS;
                currentColBlockCount++;
                
            }

            if (currentColBlockCount < 3)
            {
                //delete the blocks that were added as vertical match is impossible in current column
                int j = 0;
                while (j < currentColBlockCount)
                {
                    vMatchList.RemoveAt(vMatchList.Count - 1);
                    j++;
                    //y--;
                }
                continue;
            }

            //continue checking the match list until
            while (y < vMatchList.Count && y <= playerWell.blockList.Count - 1) //TODO: Need a condition here that will let me check all blocks in the match list.
            {
                //compare block that y points to against the previous two blocks
                if (vMatchList[y].blockType == vMatchList[y - 1].blockType && vMatchList[y].blockType == vMatchList[y - 2].blockType)
                {
                    currentBlockVMatching = true;
                    //we have a vertical match
                    if (!vMatchFound)
                    {
                        matchCount++;
                        matchingBlockCount += 3;    //at least 3 blocks must match.
                        vMatchFound = true;
                    }
                    else
                    {
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
                    y++;
                }
                else if (vMatchFound && !currentBlockVMatching)
                {
                    //we found a previous match but the current block doesn't match, so we start a new comparison to avoid the matched blocks. Advance y 2 times.
                    //vMatchList.RemoveAt(y); //y does not move
                    //y = matchingBlockCount + 3;
                    y += 2;
                    vMatchFound = false;
                }
                else
                {
                    y++; //y is increased so that the matched blocks are not compared against in the future, potentially deleting valid blocks.
                }

              
            }          

        }

        //if we're on the last block and there's no match, delete the last 2 blocks. These 2 blocks do not
        //match with anything and can be safely removed.
        if (y >= vMatchList.Count - 1 && !currentBlockVMatching)
        {
            //delete current block.
            vMatchList.RemoveAt(vMatchList.Count - 1);
            if (!vMatchFound)
            {
                //also remove the second last block since we had a match previously and don't want to remove a matching block.
                vMatchList.RemoveAt(vMatchList.Count - 1);
            }
        }

        //once we get here, there should only be matching blocks in the lists. Clear all horizontal and vertical matches
        hMatchList.TrimExcess();
        vMatchList.TrimExcess();

        if (matchCount > 0)
        {
            string list = "Horizontal matches: ";
            Debug.Log("Total matches: " + matchCount);
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
            Debug.Log(list);
        }
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
