using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CharacterClass player;
    public CharacterClass target;

    // Start is called before the first frame update
    void Start()
    {
        // player = new Angel();
        Debug.Log("Player HP " + player.healthPoints);
        Debug.Log("Target HP " + target.healthPoints);

        Debug.Log("Player skills:");
        for (int i = 0; i < player.skills.Count; i++)
            Debug.Log(player.skills[i].skillName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.skills[0].UseSkill(player, target);  //heal skill
           // target.skills[0].UseSkill(target);
        }
    }
}
