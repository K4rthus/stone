using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance { get; private set; }
    public Dictionary<string, bool> Flags => flags;

    private Dictionary<string, bool> flags = new();

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

    public void SetFlag(string flagName, bool state) => flags[flagName] = state;
    public bool CheckFlag(string flagName) => flags.TryGetValue(flagName, out bool value) && value;
    public void ResetFlags() => flags.Clear();
}