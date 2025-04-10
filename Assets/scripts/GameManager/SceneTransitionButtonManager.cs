using UnityEngine;
using System.Collections.Generic;

public class SceneTransitionButtonManager : MonoBehaviour
{
    public static SceneTransitionButtonManager Instance;

    [HideInInspector]
    public List<ButtonState> buttonStates = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetButtonState(string dialogueFile, string nodeId, bool isActive)
    {
        buttonStates.RemoveAll(s =>
            s.dialogueFile == dialogueFile &&
            s.nodeId == nodeId);

        buttonStates.Add(new ButtonState
        {
            dialogueFile = dialogueFile,
            nodeId = nodeId,
            isActive = isActive
        });
    }

    public ButtonState GetButtonState(string dialogueFile, string nodeId)
    {
        return buttonStates.Find(s =>
            s.dialogueFile == dialogueFile &&
            s.nodeId == nodeId);
    }
}