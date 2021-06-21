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

    public float riseRate;              //controls block speed            

    //constants
    const int MAX_BLOCKTYPES = 9;
    const int PLAYER_WELL_1 = 0;
    const int PLAYER_WELL_2 = 1;
    const float INIT_BLOCK_SPEED = 0.1f;
    int PlayerOne { get; } = 0;
    int PlayerTwo { get; } = 1;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        riseRate = INIT_BLOCK_SPEED;
        //ensure both wells have the same initial blocks. Any blocks generated afterwards can be different.
        //playerWells[PLAYER_WELL_1].InitializeBlocks();
        playerWells[PLAYER_WELL_1].GenerateBlocks(3);
        playerWells[PLAYER_WELL_2].blockList = playerWells[PLAYER_WELL_2].CopyBlockList(playerWells[PLAYER_WELL_1].blockList, 3);

        foreach (Well well in playerWells)
            well.RiseRate = riseRate;

        //player set up
        player[PlayerOne].playerID = PlayerOne;
        GameUI.instance.playerNameText[PlayerOne].text = player[PlayerOne].className;
        GameUI.instance.healthPointText[PlayerOne].text = player[PlayerOne].healthPoints.ToString();
        GameUI.instance.healthBars[PlayerOne].SetMaxValue(player[PlayerOne].healthPoints);

        player[PlayerTwo].playerID = PlayerTwo;
        GameUI.instance.playerNameText[PlayerTwo].text = player[PlayerTwo].className;
        GameUI.instance.healthPointText[PlayerTwo].text = player[PlayerTwo].healthPoints.ToString();
        GameUI.instance.healthBars[PlayerTwo].SetMaxValue(player[PlayerTwo].healthPoints);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Well well in playerWells)
            well.RaiseBlocks(well.RiseRate);

        //playerWells[PLAYER_WELL_2].RaiseBlocks(riseRate);
        /*if (Input.GetMouseButtonDown(0))
        {
            //TODO: Can I use delegates to select multiple skills with different overloaded methods?
           // player.skills[0].UseSkill(player, target);  //heal skill
            //target.skills[0].UseSkill(target);
        }*/
    }

    //left mouse button action will be context sensitive
    public void OnLeftMouseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            player[PlayerOne].skills[0].UseSkill(player[PlayerOne]);
            player[PlayerTwo].skills[0].UseSkill(player[PlayerTwo], player[PlayerOne]);
            
        }
    }
}
