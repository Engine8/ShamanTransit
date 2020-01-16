using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockGenerator))]
public class BlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BlockGenerator block = target as BlockGenerator;

        block.GeneratorBlock();
    }
}
