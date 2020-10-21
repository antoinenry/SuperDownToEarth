using System.Collections;
using UnityEngine;

public abstract class StepByStep : MonoBehaviour
{
    public enum CycleType { Single, Loop, PingPong }

    public CycleType cycleType;
}
