using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterClass player;
    public CharacterClass target;
    public Well[] playerWells;

    List<Block> blockList;             

    //constants
    const int MAX_BLOCKTYPES = 9;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //TODO: Can I use delegates to select multiple skills with different overloaded methods?
            player.skills[0].UseSkill(player, target);  //heal skill
           // target.skills[0].UseSkill(target);
        }
    }
}
