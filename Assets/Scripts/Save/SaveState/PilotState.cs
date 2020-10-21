using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotState : ComponentState
{
    public override Component Component => throw new System.NotImplementedException();
    public override Type Type => typeof(Pilot);
}
