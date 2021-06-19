using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterClass player;
    public CharacterClass target;
    public Well[] playerWells;

    List<Block> blockList;             

    //constants
    const int MAX_BLOCKTYPES = 9;
    const int PLAYER_WELL_1 = 0;
    const int PLAYER_WELL_2 = 1;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ensure both wells have the same initial blocks. Any blocks generated afterwards can be different.
        playerWells[PLAYER_WELL_1].InitializeBlocks();
        
        float offset = 0.5f;
        float xBounds = playerWells[PLAYER_WELL_2].GetComponentInChildren<SpriteRenderer>().bounds.min.x + offset;
        float yBounds = playerWells[PLAYER_WELL_2].GetComponentInChildren<SpriteRenderer>().bounds.min.y + offset;
        int x = 0;          //iterator 
        for (int i = 0; i < playerWells[PLAYER_WELL_2].WellRows() / 4; i++)
        {
            for (int j = 0; j < playerWells[PLAYER_WELL_2].WellCols(); j++)
            {
                playerWells[PLAYER_WELL_2].blockList.Add(Instantiate(playerWells[PLAYER_WELL_1].blockList[x], new Vector2(xBounds + j, yBounds + i), Quaternion.identity));
                x++;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            //TODO: Can I use delegates to select multiple skills with different overloaded methods?
            player.skills[0].UseSkill(player, target);  //heal skill
           // target.skills[0].UseSkill(target);
        }*/
    }
}
