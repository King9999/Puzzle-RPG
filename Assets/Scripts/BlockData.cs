using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block", menuName = "Block")]
public class BlockData : ScriptableObject
{
    public Sprite blockSprite;
    public Block.BlockType blockType;
}
