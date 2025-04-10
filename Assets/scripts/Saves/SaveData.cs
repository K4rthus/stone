using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<SceneStateEntry> sceneStates = new();
    public List<FlagData> flags = new();
    public List<ReturnButtonState> returnButtonStates = new();
    public List<ButtonState> sceneTransitionStates = new();
    public string currentScene;
    public string previousScene;
    public string currentDialogueFile;
    public string currentDialogueNode;
}