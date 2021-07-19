using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//The cursor consists of two cursors, each of which will capture the block it's resting on. To make this work,
//I use a 2D array
public class Cursor : MonoBehaviour
{
    public SpriteRenderer[] cursorSprites;
    const float INPUT_DELAY = 0.1f;        //used to prevent rapid movement of left stick.
    public float Z_Value { get; } = -2f;              //ensures that cursors is always displayed over blocks.
    float currentTime = 0;
    public bool isAIControlled = false;            //if true, AI moves the cursor, not the controller.
    const float xOffset = -2.5f;
    const float yOffset = -6.5f;
    bool shoulderButtonHeld;                      

    //int CurrentIndex;                           //tracks the current index in the block list.

    public int CurrentRow { get; set; }         //used to get a block's position in block list.
    public int CurrentCol { get; set; }
    public int TotalRows { get; } = 12;
    public int TotalCols { get; } = 6;

    public int CurrentIndex { get; set; }       //tracks the current index in the block list.

    private void Start()
    {
        //cursor is initially placed in centre of well
        CurrentRow = TotalRows / 2 - 1;
        CurrentCol = TotalCols / 2 - 1;

        //transform.position = new Vector3(GameManager.instance.playerWells[0].transform.position.x + CurrentCol + xOffset, 
            //GameManager.instance.playerWells[0].transform.position.y + CurrentRow + yOffset, Z_Value);

        //position the second sprite so that it's to the right of the main cursor.
        cursorSprites[1].transform.position = new Vector3(cursorSprites[1].transform.position.x + 1, cursorSprites[1].transform.position.y, Z_Value);

        shoulderButtonHeld = false;
    }
    //the cursor rises at the same rate as the blocks.
    private void Update()
    {
        CurrentIndex = (TotalCols * CurrentRow) + CurrentCol;

        //transform.position = new Vector3(GameManager.instance.playerWells[0].transform.position.x + CurrentCol + xOffset,
            //GameManager.instance.playerWells[0].transform.position.y + CurrentRow + yOffset, Z_Value);

        //get controls for shoulder buttons on gamepad (button can be held down)
        var pad = Gamepad.current;

        if (pad != null && (pad.rightShoulder.isPressed || pad.leftShoulder.isPressed))
        {
            //change rise rate to 0.02
            GameManager.instance.playerWells[GameManager.instance.PlayerOne].RiseRate = 0.02f;
        }
        else
        {
            //reset rise rate
            GameManager.instance.playerWells[GameManager.instance.PlayerOne].RiseRate = GameManager.instance.InitRiseRate;
        }
    }

    //Gets the block data that the cursor is currently resting on.
    public Block[,] GetBlocks(List<Block> blocks)
    {
        //Get the cursor's current row and column position, and get the block data both cursors are resting on
        //Add the block data from block list.
        Block[,] blockArray = new Block[1, 2];
        CurrentIndex = (TotalCols * CurrentRow) + CurrentCol;
        blockArray[0, 0] = blocks[CurrentIndex];
        blockArray[0, 1] = blocks[CurrentIndex + 1];


        return blockArray;
    }

    #region Controls
    public void MoveUp(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            if (/*CurrentRow - 1 >= 0*/CurrentRow + 1 <= GameManager.instance.playerWells[0].TotalRows - 1)
            {
                CurrentRow++;
                transform.position = new Vector3(transform.position.x, transform.position.y + 1, Z_Value);
                CurrentIndex = (TotalCols * CurrentRow) + CurrentCol;
                //Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[CurrentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[CurrentIndex + 1].blockType);
                Debug.Log("Row: " + CurrentRow + " Col: " + CurrentCol);
            }
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            if (CurrentRow - 1 >= 1/*CurrentRow + 1 <= GameManager.instance.playerWells[0].RowDepth - 1*/)  //we ignore the bottom row because it's reserved for new blocks.
            {
                CurrentRow--;
                transform.position = new Vector3(transform.position.x, transform.position.y - 1, Z_Value);
                CurrentIndex = (TotalCols * CurrentRow) + CurrentCol;
                //Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[CurrentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[CurrentIndex + 1].blockType);
                Debug.Log("Row: " + CurrentRow + " Col: " + CurrentCol);

            }
        }
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            if (CurrentCol - 1 >= 0)
            {
                CurrentCol--;
                transform.position = new Vector3(transform.position.x - 1, transform.position.y, Z_Value);
                CurrentIndex = (TotalCols * CurrentRow) + CurrentCol;
                // Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[CurrentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[CurrentIndex + 1].blockType);
                Debug.Log("Row: " + CurrentRow + " Col: " + CurrentCol);

            }
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            if (CurrentCol + 1 <= TotalCols - 2)   //subtract 2 so that I can capture the block to the right of the cursor without worrying about capturing a space outside of the well.
            {
                CurrentCol++;
                transform.position = new Vector3(transform.position.x + 1, transform.position.y, Z_Value);
                CurrentIndex = (TotalCols * CurrentRow) + CurrentCol;
                //Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[CurrentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[CurrentIndex + 1].blockType);
                Debug.Log("Row: " + CurrentRow + " Col: " + CurrentCol);

            }
        }
    }

    public void MoveBlocks(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        if (context.phase == InputActionPhase.Performed)
        {
            int playerOne = GameManager.instance.PlayerOne;
            Well playerWell = GameManager.instance.playerWells[playerOne];         
            //Debug.Log("Original Blocks: " + GameManager.instance.playerWells[playerOne].blockList[CurrentIndex].blockType + " & " + GameManager.instance.playerWells[playerOne].blockList[CurrentIndex + 1].blockType);

           
            Block temp = playerWell.blockList[CurrentIndex];       
            Vector2 tempPos = new Vector2(temp.transform.position.x, temp.transform.position.y);    //need a separate copy of temp position so we don't refer directly to the temp object's position
            playerWell.blockList[CurrentIndex].transform.position = playerWell.blockList[CurrentIndex + 1].transform.position;
            playerWell.blockList[CurrentIndex] = playerWell.blockList[CurrentIndex + 1];

            playerWell.blockList[CurrentIndex + 1].transform.position = tempPos;
            playerWell.blockList[CurrentIndex + 1] = temp;

            //check for a match
            //GameManager.instance.CheckForMatches(playerWell);

           //Debug.Log("New Blocks: " + GameManager.instance.playerWells[playerOne].blockList[CurrentIndex].blockType + " & " + GameManager.instance.playerWells[playerOne].blockList[CurrentIndex + 1].blockType);
        }
    }

    public void RaiseBlocksManually(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        /*if (context.phase == InputActionPhase.Performed)
        {
            shoulderButtonHeld = true;
            Debug.Log("Shoulder button held: " + shoulderButtonHeld);
        }

        if (context.phase == InputActionPhase.Canceled) //button is released
        {
            //Debug.Log("Letting go of button");
            shoulderButtonHeld = false;
            Debug.Log("Shoulder button held: " + shoulderButtonHeld);
        }*/
    }

        #endregion
    }
