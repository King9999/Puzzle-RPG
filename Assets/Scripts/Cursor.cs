using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "New Cursor", menuName = "Scriptable Objects/Cursor")]
public class Cursor : MonoBehaviour
{
    const int ROW = 12;
    const int COLUMN = 6;
    const float INPUT_DELAY = 0.16f;        //used to prevent rapid movement of left stick.
    public float Z_Value { get; } = -2f;              //ensures that cursors is always displayed over blocks.
    float currentTime = 0;
    public bool isAIControlled = false;            //if true, AI moves the cursor, not the controller.
    float xOffset = 0.1f;
    float yOffset = 0.55f;

    public int CurrentRow { get; set; }         //used to get a block's position in block list.
    public int CurrentCol { get; set; }

    private void Start()
    {
        //offset is applied so that cursor lines up with blocks.
        //transform.position = new Vector3(transform.position.x - xOffset, transform.position.y + yOffset, Z_Value);
        CurrentRow = 0;
        CurrentCol = 0;
    }
    //the cursor rises at the same rate as the blocks.
    /*private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + GameManager.instance.riseRate * Time.deltaTime, Z_Value);
    }*/
    public void MoveUp(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            //transform.position = new Vector3(transform.position.x, transform.position.y + 1, Z_Value);
            if (CurrentRow - 1 >= 0)
            {
                CurrentRow--;
                //get position of block in list, and take its world position.
            }
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            //transform.position = new Vector3(transform.position.x, transform.position.y - 1, Z_Value);
            if (CurrentRow + 1 <= ROW - 1)
            {
                CurrentRow++;
            }
        }
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            //transform.position = new Vector3(transform.position.x - 1, transform.position.y, Z_Value);
            if (CurrentCol - 1 >= 0)
            {
                CurrentCol--;
            }
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            //transform.position = new Vector3(transform.position.x + 1, transform.position.y, Z_Value);
            if (CurrentCol + 1 <= COLUMN - 1)
            {
                CurrentCol++;
            }
        }
    }
}
