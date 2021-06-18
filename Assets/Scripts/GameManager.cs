using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterClass player;

    // Start is called before the first frame update
    void Start()
    {
       // player = new Angel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.Skills[0].UseSkill(player);  //heal skill
        }
    }
}
