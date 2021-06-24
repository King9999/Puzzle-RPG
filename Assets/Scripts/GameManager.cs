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
        //Debug.Log("Row Depth: " + playerWell.RowDepth);
        //cannot proceed with this method in certain conditions
        if (playerWell.blockList.Count < 3 && playerWell.RowDepth < 3)
            return;

        /*****horizontal check*****/       
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

        //add first block, then add the block 6 blocks ahead of the first. For vertical comparisons we always look
        //X blocks ahead, where X is the total number of columns. It makes sense when you look at the blocks in game.
        //Need to check for null reference since we jump ahead a lot.
        vMatchList.Add(playerWell.blockList[0]);
        if (playerWell.blockList[COLS] != null) 
            vMatchList.Add(playerWell.blockList[COLS]);
        int y = vMatchList.Count - 1;

        for (int i = hMatchList.Count; i < playerWell.blockList.Count; i++)
        {
            //add current block for comparison
            hMatchList.Add(playerWell.blockList[i]);
            x++;
            if (i + COLS < playerWell.blockList.Count)
            {
                vMatchList.Add(playerWell.blockList[i + COLS]);
                y++;
            }

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
                //continue;   //we're done here, move to next iteration
            }
            else
            {
                currentBlockHMatching = false;
            }

            if (!hMatchFound)
            {
                //remove the leftmost block as we no longer need it.
                //hMatchList[x - 2] = null;
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
                    //hMatchList.Add(playerWell.blockList[i + 2]);
                    i++;
                    x++;
                }
                hMatchFound = false;
            }

            //vertical check
            if (vMatchList.Count < 3) 
                continue;   //vertical match is impossible

            if (vMatchList[y].blockType == vMatchList[y - 1].blockType && vMatchList[y].blockType == vMatchList[y - 2].blockType)
            {
                currentBlockHMatching = true;
                //we have a horizontal match
                if (!hMatchFound)
                {
                    matchCount++;
                    hMatchFound = true;
                }
                //continue;   //we're done here, move to next iteration
            }
            else
            {
                currentBlockHMatching = false;
            }

            if (!hMatchFound)
            {
                //remove the leftmost block as we no longer need it.
                //hMatchList[x - 2] = null;
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
                    //hMatchList.Add(playerWell.blockList[i + 2]);
                    i++;
                    x++;
                }
                hMatchFound = false;
            }

        }

        //once we get here, clear all horizontal matches
        hMatchList.TrimExcess();     //why doesn't this work?

        /*if (matchCount > 0)
        {
            Debug.Log("Total matches: " + matchCount);
            foreach (Block b in hMatchList)
            {
                if (b == null)
                    continue;

                Debug.Log(b.blockType);
            }
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
