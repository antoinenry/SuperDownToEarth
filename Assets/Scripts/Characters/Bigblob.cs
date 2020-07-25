using System.Collections;
using UnityEngine;

public class Bigblob : MonoBehaviour
{
    public enum BigblobState { Asleep, Wriggle, Awake, Dance }
    
    public EnumChangeEvent currentState = new EnumChangeEvent(typeof(BigblobState));
    public float wriggleTime = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ChangeStateCoroutine(BigblobState.Awake));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine(ChangeStateCoroutine(BigblobState.Dance));
    }

    private IEnumerator ChangeStateCoroutine(BigblobState newState)
    {
        if (currentState == (int)BigblobState.Asleep || currentState == (int)BigblobState.Dance)
        {
            currentState.Value = (int)BigblobState.Wriggle;
            yield return new WaitForSeconds(wriggleTime);
        }

        currentState.Value = newState;
    }
}
