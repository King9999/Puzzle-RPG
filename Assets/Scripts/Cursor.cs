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
    const float Z_VALUE = -2f;              //ensures that cursors is always displayed over blocks.
    float currentTime = 0;
    public bool isAIControlled = false;            //if true, AI moves the cursor, not the controller.

    //the cursor rises at the same rate as the blocks.
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + GameManager.instance.riseRate * Time.deltaTime, Z_VALUE);
    }
    public void MoveUp(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, Z_VALUE);
        }
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, Z_VALUE);
        }
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, Z_VALUE);
        }
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        if (Time.time > currentTime + INPUT_DELAY && context.phase == InputActionPhase.Performed)
        {
            currentTime = Time.time;
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, Z_VALUE);
        }
    }
}
