using UnityEngine;

public class ActionChainLink : MonoBehaviour
{
    public enum LinkState { FarInLine, NextInLine, Passed, ChainCompleted, ChainFailed }
    public bool isFirstLink;
    public float resetChainDelay = -1f;
    public ActionChainLink nextLink;

    public EnumChangeEvent currentState = new EnumChangeEvent(typeof(LinkState));

    protected virtual void OnEnable()
    {
        currentState.Value = isFirstLink ? LinkState.NextInLine : LinkState.FarInLine;
        currentState.AddValueListener<LinkState>(OnStateChange);
        if (nextLink != null)
            nextLink.currentState.AddValueListener<LinkState>(OnNextLinkStateChange);
    }

    protected virtual void OnDisable()
    {
        currentState.RemoveValueListener<LinkState>(OnStateChange);
        if (nextLink != null)
            nextLink.currentState.RemoveValueListener<LinkState>(OnNextLinkStateChange);
    }

    public void SetNextLink(ActionChainLink newNextLink)
    {
        if (nextLink != newNextLink)
        {
            if (nextLink != null) nextLink.currentState.RemoveValueListener<LinkState>(OnNextLinkStateChange);
            nextLink = newNextLink;
            if (nextLink != null) nextLink.currentState.AddValueListener<LinkState>(OnNextLinkStateChange);
        }
    }

    private void OnStateChange(LinkState newState)
    {
        switch (newState)
        {
            case LinkState.FarInLine: OnFarInLine(); break;
            case LinkState.NextInLine: OnNextInLine(); break;
            case LinkState.Passed: OnPassed(); break;
            case LinkState.ChainCompleted: OnChainCompleted(); break;
            case LinkState.ChainFailed: OnChainFailed(); break;
        }
    }

    private void OnNextLinkStateChange(LinkState newState)
    {
        switch (newState)
        {
            case LinkState.FarInLine: OnNextLinkFarInLine(); break;
            case LinkState.NextInLine: OnNextLinkNextInLine(); break;
            case LinkState.Passed: OnNextLinkPassed(); break;
            case LinkState.ChainCompleted: OnNextLinkChainCompleted(); break;
            case LinkState.ChainFailed: OnNextLinkChainFailed(); break;
        }
    }

    protected virtual void OnFarInLine()
    { }

    protected virtual void OnNextInLine()
    { }

    protected virtual void OnPassed()
    {
        if (nextLink != null) nextLink.currentState.Value = LinkState.NextInLine;
        else currentState.Value = LinkState.ChainCompleted;
    }

    protected virtual void OnChainCompleted()
    { }

    protected virtual void OnChainFailed()
    {
        if (resetChainDelay >= 0f)
            Invoke("ResetChainLink", resetChainDelay);
    }    

    protected virtual void OnNextLinkFarInLine()
    { }

    protected virtual void OnNextLinkNextInLine()
    { }

    protected virtual void OnNextLinkPassed()
    { }

    protected virtual void OnNextLinkChainCompleted()
    {
        currentState.Value = LinkState.ChainCompleted;
    }

    protected virtual void OnNextLinkChainFailed()
    {
        currentState.Value = LinkState.ChainFailed;
    }

    private void ResetChainLink()
    {
        if (isFirstLink)
            currentState.Value = LinkState.NextInLine;
        else
            currentState.Value = LinkState.FarInLine;
    }
}
