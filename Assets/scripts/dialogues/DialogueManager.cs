using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject optionButtonPrefab;

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public Transform optionsPanel;

    [Header("Dialogue File")]
    public string dialogueFileName;

    private DialogueNode _currentNode;
    private DialogueData currentDialogue;

    [Header("Return Button")]
    public Button returnButton;


    private PauseMenuManager pauseMenu;
    public DialogueNode CurrentNode => _currentNode;

    private void Start()
    {
        pauseMenu = FindFirstObjectByType<PauseMenuManager>();
        if (PlayerPrefs.HasKey("DialogueFile"))
        {
            string file = PlayerPrefs.GetString("DialogueFile");
            string node = PlayerPrefs.GetString("TargetNode");
            PlayerPrefs.DeleteKey("DialogueFile");
            PlayerPrefs.DeleteKey("TargetNode");

            LoadDialogue(file);
            StartDialogue(node);
            SaveManager.Instance.SaveGame();
        }
        else if (SaveManager.Instance.CurrentSaveData.currentDialogueFile == dialogueFileName
                 && !string.IsNullOrEmpty(SaveManager.Instance.CurrentSaveData.currentDialogueNode))
        {
            LoadDialogue(dialogueFileName);
            StartDialogue(SaveManager.Instance.CurrentSaveData.currentDialogueNode);
        }
        else
        {
            LoadDialogue(dialogueFileName);
            StartDialogue(currentDialogue.startNode);
        }
    }

    public void LoadDialogue(string fileName)
    {
        TextAsset jsonData = Resources.Load<TextAsset>(fileName);
        currentDialogue = JsonUtility.FromJson<DialogueData>(jsonData.text);
    }

    public void StartDialogue(string nodeId)
    {
        Debug.Log($"Trying to start a node: {nodeId}");

        if (currentDialogue == null)
        {
            Debug.LogError("The dialog is not loaded!");
            return;
        }

        _currentNode = currentDialogue.nodes.Find(n => n.id == nodeId);

        if (_currentNode == null)
        {
            Debug.LogError($"Node {nodeId} not found!");
            return;
        }

        Debug.Log($"Node started successfully: {_currentNode.id}");
        UpdateDialogueUI();
    }

    public void UpdateDialogueUI()
    {
        bool shouldBlock = pauseMenu != null && pauseMenu.IsPaused;

        dialogueText.text = _currentNode.text;

        foreach (Transform child in optionsPanel)
            Destroy(child.gameObject);

        foreach (var option in _currentNode.options)
        {
            if (CheckConditions(option.conditions))
            {
                GameObject button = Instantiate(optionButtonPrefab, optionsPanel);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                Button buttonComponent = button.GetComponent<Button>();

                buttonText.text = option.text;

                buttonComponent.interactable = !shouldBlock;

                buttonComponent.onClick.AddListener(() =>
                {
                    if (!shouldBlock) SelectOption(option);
                });
            }
        }

        if (returnButton != null)
        {
            bool isActive = ReturnButtonManager.Instance.IsReturnButtonActive(dialogueFileName, _currentNode.id);
            CanvasGroup cg = returnButton.GetComponent<CanvasGroup>();

            cg.alpha = (isActive && !shouldBlock) ? 1f : 0.5f;
            cg.interactable = isActive && !shouldBlock;
            cg.blocksRaycasts = isActive && !shouldBlock;
        }
    }

    bool CheckConditions(List<FlagCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            bool requiredState = condition.requiredState;
            bool actualState = FlagManager.Instance.CheckFlag(condition.flagName);

            if (actualState != requiredState)
            {
                return false;
            }
        }
        return true;
    }

    void ApplyOperations(List<FlagOperation> operations)
    {
        foreach (var op in operations)
        {
            FlagManager.Instance.SetFlag(op.flagName, op.newState);
        }
    }

    public void SelectOption(DialogueOption option)
    {
        if (pauseMenu != null && pauseMenu.IsPaused)
        {
            return;
        }

        if (!string.IsNullOrEmpty(option.sound))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/Dialogue/{option.sound}");
            if (clip != null)
            {
                SoundManager.Instance.PlaySFX(clip);
            }
        }

        ApplyOperations(option.flagOperations);

        if (!string.IsNullOrEmpty(option.targetNode))
        {
            var targetNode = currentDialogue.nodes.Find(n => n.id == option.targetNode);
            if (option.activateReturnButton && targetNode != null)
            {
                ReturnButtonManager.Instance.SetReturnButtonState(dialogueFileName, targetNode.id, true);
            }

            StartDialogue(option.targetNode);

            SaveManager.Instance.SaveGame();
        }
        else
        {
            CloseDialogue();
        }
    }

    private void HandlePauseState(bool isPaused)
    {
        if (gameObject.activeInHierarchy) 
        { 
            UpdateDialogueUI();
        }
    }

    private void OnEnable()
    {
        PauseMenuManager.OnPauseStateChanged += HandlePauseState;
    }

    private void OnDisable()
    {
        PauseMenuManager.OnPauseStateChanged -= HandlePauseState;
    }

    private void CloseDialogue()
    {
        gameObject.SetActive(false);
    }
}