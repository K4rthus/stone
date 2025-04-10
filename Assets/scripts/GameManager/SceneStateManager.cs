using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    public Dictionary<string, SceneState> sceneStates = new();
    public string CurrentScene { get; private set; }
    public string PreviousScene { get; private set; }

    [SerializeField] public Vector3 initialPlayerPosition = new(-1.131f, -0.498f, 0f);
    [SerializeField] public string startSceneName = "Field";

    private bool isLoading = false;

    void Awake()
    {
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

    public void SaveCurrentState(Vector3 playerPos, string interactableId)
    {
        if (!sceneStates.ContainsKey(CurrentScene))
        {
            sceneStates[CurrentScene] = new SceneState();
        }

        sceneStates[CurrentScene].playerPosition = playerPos;
        if (!string.IsNullOrEmpty(interactableId))
        {
            sceneStates[CurrentScene].usedInteractables.Add(interactableId);
        }
    }

    public void LoadGameState(SaveData saveData)
    {
        sceneStates.Clear();
        foreach (var entry in saveData.sceneStates)
        {
            sceneStates[entry.sceneName] = entry.state;
        }

        PreviousScene = saveData.previousScene;
        CurrentScene = saveData.currentScene;
        isLoading = true;
        SceneManager.LoadScene(CurrentScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "00_ManagerScene") return;

        if (isLoading)
        {
            PreviousScene = SaveManager.Instance.CurrentSaveData.previousScene;
            CurrentScene = SaveManager.Instance.CurrentSaveData.currentScene;
            isLoading = false;
        }
        else
        {
            PreviousScene = CurrentScene;
            CurrentScene = scene.name;
        }

        if (sceneStates.ContainsKey(CurrentScene))
        {
            RestoreSceneState(CurrentScene);
        }
        else
        {
            sceneStates[CurrentScene] = new SceneState();
        }

        if (isLoading) LoadDialogueState();
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
                : initialPlayerPosition;
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

    private void LoadDialogueState()
    {
        var saveData = SaveManager.Instance.CurrentSaveData;
        if (!string.IsNullOrEmpty(saveData.currentDialogueFile))
        {
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            if (dialogueManager != null)
            {
                dialogueManager.LoadDialogue(saveData.currentDialogueFile);
                dialogueManager.StartDialogue(saveData.currentDialogueNode);
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

    public string GetPreviousScene()
    {
        return PreviousScene;
    }
}