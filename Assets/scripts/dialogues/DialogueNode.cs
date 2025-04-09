using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string id;
    [TextArea(3, 10)] public string text;
    public List<DialogueOption> options = new();
    public bool activateReturnButton;
}