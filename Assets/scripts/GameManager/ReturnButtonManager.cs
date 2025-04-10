using System.Collections.Generic;
using UnityEngine;

public class ReturnButtonManager : MonoBehaviour
{
    public static ReturnButtonManager Instance;

    [HideInInspector]
    public List<ReturnButtonState> returnButtonStates = new();

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

    public void SetReturnButtonState(string dialogueFile, string nodeId, bool isActive)
    {
        returnButtonStates.RemoveAll(s =>
            s.dialogueFile == dialogueFile && s.nodeId == nodeId);

        returnButtonStates.Add(new ReturnButtonState
        {
            dialogueFile = dialogueFile,
            nodeId = nodeId,
            isActive = isActive
        });
    }

    public bool IsReturnButtonActive(string dialogueFile, string nodeId)
    {
        var state = returnButtonStates.Find(s =>
            s.dialogueFile == dialogueFile && s.nodeId == nodeId);
        return state != null && state.isActive;
    }
}