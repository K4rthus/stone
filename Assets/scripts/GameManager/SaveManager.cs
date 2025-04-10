using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public SaveData CurrentSaveData { get; private set; } = new();
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        CurrentSaveData = new SaveData
        {
            sceneStates = new List<SceneStateEntry>(),
            flags = new List<FlagData>(),
            returnButtonStates = ReturnButtonManager.Instance.returnButtonStates,
            currentScene = SceneStateManager.Instance.CurrentScene,
            previousScene = SceneStateManager.Instance.PreviousScene,
            currentDialogueFile = "",
            currentDialogueNode = ""
        };

        foreach (var entry in SceneStateManager.Instance.sceneStates)
        {
            CurrentSaveData.sceneStates.Add(new SceneStateEntry
            {
                sceneName = entry.Key,
                state = entry.Value
            });
        }

        foreach (var flag in FlagManager.Instance.Flags)
        {
            CurrentSaveData.flags.Add(new FlagData
            {
                flagName = flag.Key,
                state = flag.Value
            });
        }

        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null && dialogueManager.gameObject.activeInHierarchy)
        {
            CurrentSaveData.currentDialogueFile = dialogueManager.dialogueFileName;
            CurrentSaveData.currentDialogueNode = dialogueManager.CurrentNode?.id ?? "";
        }

        string jsonData = JsonUtility.ToJson(CurrentSaveData);
        File.WriteAllText(savePath, jsonData);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            CurrentSaveData = JsonUtility.FromJson<SaveData>(jsonData);

            FlagManager.Instance.ResetFlags();
            foreach (var flagData in CurrentSaveData.flags)
            {
                FlagManager.Instance.SetFlag(flagData.flagName, flagData.state);
            }

            ReturnButtonManager.Instance.returnButtonStates = CurrentSaveData.returnButtonStates;
            SceneStateManager.Instance.LoadGameState(CurrentSaveData);
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }
}