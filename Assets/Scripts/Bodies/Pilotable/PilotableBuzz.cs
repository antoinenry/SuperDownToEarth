using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class PilotableBuzz : Pilotable
{
    public FlatGroundProbe groundProbe;
    public LocalGravity gravity;
    public Jumper jumper;
    public Spinner spinner;

    private float flyingAngle;
    private UnityAction<int> probeGround;

    public PilotableBuzz(BodyPart[] parts)
    {
        SetPilotingType(PilotingType.Buzz);
        
        if (parts != null)
        {
            foreach (BodyPart part in parts)
            {
                if (jumper == null && part is Jumper) jumper = part as Jumper;

                else if (spinner == null && part is Spinner) spinner = part as Spinner;

                else if (groundProbe == null && part is FlatGroundProbe) groundProbe = part as FlatGroundProbe;

                else if (gravity == null && part is LocalGravity) gravity = part as LocalGravity;
            }
        }

        pilotableParts = new BodyPart[] { jumper, spinner, gravity };

        flyingAngle = gravity.angleOffset;
        probeGround = new UnityAction<int>(flatness => OnProbeGround(flatness));
        AddPartListenners();
    }

    ~PilotableBuzz()
    {
        RemovePartListeners();
    }

    private void AddPartListenners()
    {
        if (groundProbe != null)
        {
            groundProbe.GroundFlatness.AddListener(probeGround);
            OnProbeGround(groundProbe.GroundFlatness.Value);
        }
    }

    private void RemovePartListeners()
    {
        if (groundProbe != null)
        {
            groundProbe.GroundFlatness.RemoveListener(probeGround);
        }
    }

    private void OnProbeGround(int groundFlatness)
    {
        if (groundFlatness == (int)FlatGroundProbe.Flatness.Flat)
            gravity.angleOffset = 0f;
        else
            gravity.angleOffset = flyingAngle;
    }
}
