using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script will generate blocks. The blocks can be manipulated here.
public class Well : MonoBehaviour
{
    public List<Block> blocks;
    byte matchCount;    //used to track if there's a match

    // Start is called before the first frame update
    void Start()
    {
        blocks = new List<Block>();
        matchCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool BlocksMatching(Block.BlockType blockType)
    {
        bool isMatching = false;

        //check if there's a horizontal or vertical match. At least 3 blocks must match.

        return isMatching;
    }
}
