using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//The cursor consists of two cursors, each of which will capture the block it's resting on. To make this work,
//I use a 2D array
public class Cursor : MonoBehaviour
{
    public SpriteRenderer[] cursorSprites;
    const int ROW = 12;
    const int COLUMN = 6;
    const float INPUT_DELAY = 0.16f;        //used to prevent rapid movement of left stick.
    public float Z_Value { get; } = -2f;              //ensures that cursors is always displayed over blocks.
    float currentTime = 0;
    public bool isAIControlled = false;            //if true, AI moves the cursor, not the controller.
    float xOffset = 0.1f;
    float yOffset = 0.55f;

    int currentIndex;                           //tracks the current index in the block list.

    public int CurrentRow { get; set; }         //used to get a block's position in block list.
    public int CurrentCol { get; set; }

    private void Start()
    {
        //offset is applied so that cursor lines up with blocks.
        //transform.position = new Vector3(transform.position.x - xOffset, transform.position.y + yOffset, Z_Value);
        CurrentRow = 0;
        CurrentCol = 0;

        //position the second sprite so that it's to the right of the main cursor.
        cursorSprites[1].transform.position = new Vector2(cursorSprites[1].transform.position.x + 1, cursorSprites[1].transform.position.y);
    }
    //the cursor rises at the same rate as the blocks.
    private void Update()
    {
        currentIndex = (COLUMN * CurrentRow) + CurrentCol;
    }

    //Gets the block data that the cursor is currently resting on.
    public Block[,] GetBlocks(List<Block> blocks)
    {
        //Get the cursor's current row and column position, and get the block data both cursors are resting on
        //Add the block data from block list.
        Block[,] blockArray = new Block[1, 2];
        int currentIndex = (COLUMN * CurrentRow) + CurrentCol;
        blockArray[0, 0] = blocks[currentIndex];
        blockArray[0, 1] = blocks[currentIndex + 1];


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
            //transform.position = new Vector3(transform.position.x, transform.position.y + 1, Z_Value);
            if (CurrentRow - 1 >= 0)
            {
                CurrentRow--;
                currentIndex = (COLUMN * CurrentRow) + CurrentCol;
                Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[currentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[currentIndex + 1].blockType);
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
            //transform.position = new Vector3(transform.position.x, transform.position.y - 1, Z_Value);
            if (CurrentRow + 1 <= ROW - 1)
            {
                CurrentRow++;
                currentIndex = (COLUMN * CurrentRow) + CurrentCol;
                Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[currentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[currentIndex + 1].blockType);
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
            //transform.position = new Vector3(transform.position.x - 1, transform.position.y, Z_Value);
            if (CurrentCol - 1 >= 0)
            {
                CurrentCol--;
                currentIndex = (COLUMN * CurrentRow) + CurrentCol;
                Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[currentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[currentIndex + 1].blockType);
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
            if (CurrentCol + 1 <= COLUMN - 2)   //subtract 2 so that I can capture the block to the right of the cursor without worrying about capturing a space outside of the well.
            {
                CurrentCol++;
                currentIndex = (COLUMN * CurrentRow) + CurrentCol;
                Debug.Log("Current Blocks: " + GameManager.instance.playerWells[0].blockList[currentIndex].blockType + ", " + GameManager.instance.playerWells[0].blockList[currentIndex + 1].blockType);
            }
        }
    }

    public void MoveBlocks(InputAction.CallbackContext context)
    {
        if (isAIControlled)
            return;

        if (context.phase == InputActionPhase.Performed)
        {
            //int currentIndex = (COLUMN * CurrentRow) + CurrentCol;
            //swap the two blocks that the cursors are resting on.
            //Block[,] tempBlocks = new Block[1, 2];
            //tempBlocks = GetBlocks(GameManager.instance.playerWells[0].blockList);
            //Sprite blockSprite = GetComponent<SpriteRenderer>().sprite;

            Debug.Log("Original Blocks: " + GameManager.instance.playerWells[0].blockList[currentIndex].blockType + " & " + GameManager.instance.playerWells[0].blockList[currentIndex + 1].blockType);

            Block temp = GameManager.instance.playerWells[0].blockList[currentIndex];
            //GameManager.instance.playerWells[0].blockList[currentIndex].transform.position = GameManager.instance.playerWells[0].blockList[currentIndex + 1].transform.position;
            GameManager.instance.playerWells[0].blockList[currentIndex] = GameManager.instance.playerWells[0].blockList[currentIndex + 1];
            GameManager.instance.playerWells[0].blockList[currentIndex + 1] = temp;

            //change cursor position back to its original position because it's pointing to newly positioned block
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, Z_Value);


            Debug.Log("New Blocks: " + GameManager.instance.playerWells[0].blockList[currentIndex].blockType + " & " + GameManager.instance.playerWells[0].blockList[currentIndex + 1].blockType);
        }
    }

    #endregion
}
