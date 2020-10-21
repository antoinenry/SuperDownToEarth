using UnityEngine;

public class ActionChain : MonoBehaviour
{
    public ActionChainLink[] links;
    public IntChangeEvent currentLinkIndex;

    public int Length => links != null ? links.Length : 0;

    private void Reset()
    {
        links = GetComponentsInChildren<ActionChainLink>(true);
    }

    private void OnEnable()
    {
        if (links != null)
            foreach (ActionChainLink link in links)
                link.currentState.AddValueListener<ActionChainLink.LinkState>(OnLinkStateChange);

        currentLinkIndex.Value = 0;
        currentLinkIndex.AddValueListener<int>(OnCurrentLinkIndexChange);
    }

    private void OnDisable()
    {
        if (links != null)
            foreach (ActionChainLink link in links)
                link.currentState.RemoveValueListener<ActionChainLink.LinkState>(OnLinkStateChange);

        currentLinkIndex.RemoveValueListener<int>(OnCurrentLinkIndexChange);
    }

    private void OnLinkStateChange(ActionChainLink.LinkState newState)
    {
        if (newState == ActionChainLink.LinkState.Passed)
            currentLinkIndex.Value = currentLinkIndex + 1;
        else if (newState == ActionChainLink.LinkState.ChainCompleted)
            currentLinkIndex.Value = Length;
        else if (newState == ActionChainLink.LinkState.ChainFailed)
            currentLinkIndex.Value = 0;
    }

    private void OnCurrentLinkIndexChange(int currentIndex)
    {
        if (currentLinkIndex >= 0 && currentLinkIndex < Length - 1)
        {
            links[currentLinkIndex].SetNextLink(links[currentLinkIndex + 1]);
        }
    }
}
