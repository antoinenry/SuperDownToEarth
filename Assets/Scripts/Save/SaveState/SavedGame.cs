using UnityEngine;

[CreateAssetMenu(fileName = "Save", menuName = "ScriptableObjects/SavedGame", order = 1)]
public class SavedGame : ScriptableObject
{
    [SerializeField] public ComponentState[] savedStates;

    public int StateCount { get => savedStates == null ? 0 : savedStates.Length; }
}
