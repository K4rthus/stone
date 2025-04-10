using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public void SaveDialogueData(string dialogueFile, string startNode)
    {
        PlayerPrefs.SetString("DialogueFile", dialogueFile);
        PlayerPrefs.SetString("TargetNode", startNode);
        PlayerPrefs.Save();
    }

    public void ResetGame()
    {
        SceneStateManager.Instance.sceneStates.Clear();
        ReturnButtonManager.Instance.returnButtonStates.Clear();
        SaveManager.Instance.DeleteSave();
        FlagManager.Instance.ResetFlags();

        SceneStateManager.Instance.sceneStates[SceneStateManager.Instance.startSceneName] = new SceneState
        {
            playerPosition = SceneStateManager.Instance.initialPlayerPosition,
            usedInteractables = new List<string>()
        };
    }
}