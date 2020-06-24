using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class Propeller : BodyPart
{
    public float populsionForce;
    public float propulsionAngle;
    public bool constantWorldDirection;

    public BoolChangeEvent turnedOn;
    public BoolChangeEvent turnedOff;

    private Coroutine propulsionCoroutine;

    private void Awake()
    {
        AttachedBody = GetComponent<Body>();
    }

    private void OnEnable()
    {
        if (turnedOn == null) turnedOn = new BoolChangeEvent();
        if (turnedOff == null) turnedOff = new BoolChangeEvent();

        turnedOn.AddValueListener<bool>(OnTurnedOn);
        turnedOff.AddValueListener<bool>(OnTurnedOff);
    }

    private void OnDisable()
    {
        turnedOn.RemoveValueListener<bool>(OnTurnedOn);
        turnedOff.RemoveValueListener<bool>(OnTurnedOff);
    }

    private void OnTurnedOn(bool on)
    {
        turnedOff.SetValueWithoutTriggeringEvent(!on);
        if (Application.isPlaying == true)
        {
            if (on)
            {
                if (propulsionCoroutine == null)
                    propulsionCoroutine = StartCoroutine(PropulsionCoroutine());
            }
            else
            {
                if (propulsionCoroutine != null)
                {
                    StopCoroutine(propulsionCoroutine);
                    propulsionCoroutine = null;
                }
            }
        }
    }

    private void OnTurnedOff(bool off)
    {
        turnedOn.Value = !off;
    }

    private IEnumerator PropulsionCoroutine()
    {
        while (turnedOn)
        {
            Vector2 propulsionDirection = constantWorldDirection ?
            Quaternion.Euler(0f, 0f, propulsionAngle) * Vector2.right :
            Quaternion.Euler(0f, 0f, propulsionAngle) * transform.rotation * Vector2.right;

            AttachedRigidbody.AddForce(propulsionDirection * populsionForce);
            yield return new WaitForFixedUpdate();
        }
    }
}
