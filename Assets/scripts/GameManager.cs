using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Настройки старта")]
    [SerializeField] private Vector3 initialPlayerPosition = new(-1.131f, -0.498f, 0f);
    [SerializeField] private string startSceneName = "Field";

    public Dictionary<string, SceneState> sceneStates = new();

    [HideInInspector]
    public List<ReturnButtonState> returnButtonStates = new();

    private string previousScene;
    private string currentScene; 
  
    public SaveData CurrentSaveData => currentSaveData;
    private SaveData currentSaveData = new();
    private bool isLoading = false;
    private string savePath;


    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        currentSaveData = new SaveData
        {
            sceneStates = new List<SceneStateEntry>(),
            flags = new List<FlagData>(),
            returnButtonStates = returnButtonStates,
            currentScene = currentScene,
            previousScene = previousScene,
            currentDialogueFile = "",
            currentDialogueNode = ""
        };

        foreach (var entry in sceneStates)
        {
            currentSaveData.sceneStates.Add(new SceneStateEntry
            {
                sceneName = entry.Key,
                state = entry.Value
            });
        }

        foreach (var flag in FlagManager.Instance.Flags)
        {
            currentSaveData.flags.Add(new FlagData
            {
                flagName = flag.Key,
                state = flag.Value
            });
        }

        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null && dialogueManager.gameObject.activeInHierarchy)
        {
            currentSaveData.currentDialogueFile = dialogueManager.dialogueFileName;
            currentSaveData.currentDialogueNode = dialogueManager.CurrentNode?.id ?? "";
            Debug.Log($"Сохранение диалога: файл={currentSaveData.currentDialogueFile}, узел={currentSaveData.currentDialogueNode}");
        }

        string jsonData = JsonUtility.ToJson(currentSaveData);
        File.WriteAllText(savePath, jsonData);
    }

    public void SaveDialogueData(string dialogueFile, string startNode)
    {
        PlayerPrefs.SetString("DialogueFile", dialogueFile);
        PlayerPrefs.SetString("TargetNode", startNode);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveData>(jsonData);

            sceneStates.Clear();
            foreach (var entry in currentSaveData.sceneStates)
            {
                sceneStates[entry.sceneName] = entry.state;
            }

            FlagManager.Instance.ResetFlags();
            foreach (var flagData in currentSaveData.flags)
            {
                FlagManager.Instance.SetFlag(flagData.flagName, flagData.state);
            }

            returnButtonStates = currentSaveData.returnButtonStates;
            isLoading = true;

            SceneManager.LoadScene(currentSaveData.currentScene);
        }
    }

    public void ResetGame()
    {
        sceneStates.Clear();
        returnButtonStates.Clear();
        currentSaveData = new SaveData();
        FlagManager.Instance.ResetFlags();
        DeleteSave();

        sceneStates[startSceneName] = new SceneState
        {
            playerPosition = initialPlayerPosition,
            usedInteractables = new List<string>()
        };

        currentSaveData.currentDialogueFile = "";
        currentSaveData.currentDialogueNode = "";
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
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
        var state = returnButtonStates.Find(s => s.dialogueFile == dialogueFile && s.nodeId == nodeId);
        return state != null && state.isActive;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "00_ManagerScene")
        {
            return;
        }

        if (isLoading)
        {
            previousScene = currentSaveData.previousScene;
            currentScene = currentSaveData.currentScene;
            isLoading = false;
        }
        else
        {
            previousScene = currentScene;
            currentScene = scene.name;
        }

        if (currentScene == "MainMenu")
        {
            return;
        }

        if (sceneStates.ContainsKey(currentScene))
        {
            RestoreSceneState(currentScene);
        }
        else
        {
            sceneStates[currentScene] = new SceneState();
        }

        if (isLoading)
        {
            LoadDialogueState();
        }
    }

    void LoadDialogueState()
    {
        if (!string.IsNullOrEmpty(currentSaveData.currentDialogueFile))
        {
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.LoadDialogue(currentSaveData.currentDialogueFile);
                dialogueManager.StartDialogue(currentSaveData.currentDialogueNode);
            }
        }
    }

    public void SaveCurrentState(Vector3 playerPos, string interactableId)
    {
        if (!sceneStates.ContainsKey(currentScene))
        {
            sceneStates[currentScene] = new SceneState();
        }

        sceneStates[currentScene].playerPosition = playerPos;

        if (!string.IsNullOrEmpty(interactableId))
        {
            sceneStates[currentScene].usedInteractables.Add(interactableId);
        }
    }

    public string GetPreviousScene()
    {
        return previousScene;
    }

    private void RestoreSceneState(string sceneName)
    {
        if (!sceneStates.ContainsKey(sceneName)) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 targetPos = sceneStates[sceneName].playerPosition;
            player.transform.position = targetPos != Vector3.zero
                ? targetPos
                : Instance.initialPlayerPosition;
        }

        foreach (string id in sceneStates[sceneName].usedInteractables)
        {
            GameObject obj = GameObject.Find(id);
            if (obj != null)
            {
                var interactable = obj.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.DisableInteraction();
                }
            }
        }
    }

    public bool IsInteractableUsed(string sceneName, string id)
    {
        return sceneStates.ContainsKey(sceneName) &&
               sceneStates[sceneName].usedInteractables.Contains(id);
    }

    public void MarkInteractableUsed(string sceneName, string id)
    {
        if (!sceneStates.ContainsKey(sceneName))
        {
            sceneStates[sceneName] = new SceneState();
        }

        if (!sceneStates[sceneName].usedInteractables.Contains(id))
        {
            sceneStates[sceneName].usedInteractables.Add(id);
        }
    }
}