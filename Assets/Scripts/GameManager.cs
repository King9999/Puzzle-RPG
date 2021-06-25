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
        bool vMatchFound = false;
        bool currentBlockHMatching = false; //used for when the current block breaks an existing match combo.
        bool currentBlockVMatching = false;
        const int COLS = 6;                 //used for vertical matching
        List<Block> hMatchList = new List<Block>();
        List<Block> vMatchList = new List<Block>();

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
                string list = "Horizontal matches: ";
                foreach (Block b in hMatchList)
                {
                    if (b == null)
                        continue;
                    list += b.blockType + ", ";
                }
                Debug.Log(list);
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

        /******VERTICAL MATCH CHECK**********/
        //add the block 6 blocks ahead from the first, then add the block 12 blocks ahead of the first. For vertical comparisons we always look
        //X blocks ahead, where X is the total number of columns. It makes sense when you look at the blocks in game. We iterate from the first block and continue until we reach
        //the end of the column. Need to check for null reference since we jump ahead a lot.
        int colIterator = 0;
        vMatchList.Add(playerWell.blockList[colIterator]);
        colIterator += COLS;
        if (playerWell.blockList[COLS] != null)
        {
            vMatchList.Add(playerWell.blockList[colIterator]);
            colIterator += COLS;
        }
        int y = vMatchList.Count - 1;

        //run through block list again to do vertical check. Unlike horizontal search, we only need to check every 
        //6 blocks ahead, and we only need to iterate through each column
        for (int i = 0; i < COLS; i++)
        {
            if (colIterator + COLS < playerWell.blockList.Count)
            {
                vMatchList.Add(playerWell.blockList[colIterator]);
                colIterator += COLS;
                y++;
            }
            //Debug.Log("Row Depth: " + playerWell.RowDepth);
            if (vMatchList.Count < 3) 
                continue;   //vertical match is impossible

            if (vMatchList[y].blockType == vMatchList[y].blockType && playerWell.blockList[i].blockType == vMatchList[y + 1].blockType)
            {
                currentBlockVMatching = true;
                //we have a vertical match
                if (!vMatchFound)
                {
                    matchCount++;
                    vMatchFound = true;
                }
                string list = "Vertical matches: ";
                foreach (Block b in vMatchList)
                {
                    if (b == null)
                        continue;
                    list += b.blockType + ", ";
                }
                Debug.Log(list);
            }
            else
            {
                currentBlockVMatching = false;
            }

            if (!vMatchFound)
            {
                //remove the leftmost block as we no longer need it.
                vMatchList.RemoveAt(y - 2);
                y--;
            }
            else if (vMatchFound && !currentBlockVMatching)
            {
                //we found a previous match but the current block doesn't match, so we start a new comparison by adding the next two blocks to
                //the match list. The second block should get added on the next iteration.
                if (i + (COLS * 2) < playerWell.blockList.Count)
                {
                    vMatchList.Add(playerWell.blockList[i + COLS]);
                    i += COLS;
                    y++;
                }
                vMatchFound = false;
            }

        }

        //once we get here, clear all horizontal matches
        hMatchList.TrimExcess();

        /*if (matchCount > 0)
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
        }*/

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
