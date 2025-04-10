using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [TextArea(1, 3)] public string text;
    public string targetNode;
    public bool activateReturnButton;
    public bool activateSceneTransition;
    public string sound;
    public List<FlagCondition> conditions = new();
    public List<FlagOperation> flagOperations = new();
}