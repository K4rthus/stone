using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneState
{
    public Vector3 playerPosition;
    public List<string> usedInteractables = new();
}