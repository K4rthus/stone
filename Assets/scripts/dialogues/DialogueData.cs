using System.Collections.Generic;

[System.Serializable]
public class DialogueData
{
    public string startNode;
    public List<DialogueNode> nodes = new();
}