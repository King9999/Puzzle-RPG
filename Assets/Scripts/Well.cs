using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script will generate blocks. The blocks can be manipulated here.
public class Well : MonoBehaviour
{
    public List<Block> blockList;        //holds all blocks in player well
    public Block blockGenerator;
    public BlockData[] blocks;

    byte matchCount;    //used to track if there's a match

    // Start is called before the first frame update
    void Start()
    {
        blockList = new List<Block>();
        matchCount = 0;
        blockGenerator.CreateBlock(blocks[(int)Block.BlockType.Red]);
        blockList.Add(Instantiate(blockGenerator, new Vector3(0,0,-1), Quaternion.identity));
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
