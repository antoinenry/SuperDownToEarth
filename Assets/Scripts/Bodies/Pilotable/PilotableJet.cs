using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotableJet : Pilotable
{
    public Jumper jumper;
    public Spinner spinner;

    public PilotableJet(BodyPart[] parts)
    {
        SetPilotingType(PilotingType.Jet);

        if (parts != null)
        {
            foreach (BodyPart part in parts)
            {
                if (jumper == null && part is Jumper) jumper = part as Jumper;

                else if (spinner == null && part is Spinner) spinner = part as Spinner;
            }
        }

        pilotableParts = new BodyPart[] { jumper, spinner };
    }
}
