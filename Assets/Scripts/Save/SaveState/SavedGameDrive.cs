public class SavedGameDrive : SaveSystem
{
    public SavedGame saveSlot;

    private void Start()
    {
        isSaving.AddValueListener<bool>(OnGameSave);
    }

    private void OnDestroy()
    {
        isSaving.RemoveValueListener<bool>(OnGameSave);
    }

    public void SaveToSlot(ComponentState[] saveStates)
    {
        if (saveSlot != null && saveStates != null)
            saveStates.CopyTo(saveSlot.savedStates, 0);
    }

    public void LoadFromSlot()
    {
        
    }

    private void OnGameSave(bool starting)
    {
        if (starting)
        {
            return;
        }
        else
        {
            SaveToSlot(saveStatesBuffer.ToArray());
        }
    }
}
