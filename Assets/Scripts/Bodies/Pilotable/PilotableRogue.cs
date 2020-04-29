using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotableRogue : Pilotable
{
    public Jumper jumper;
    public Walker walker;

    public PilotableRogue(BodyPart[] parts)
    {
        SetPilotingType(PilotingType.Rogue);

        if (parts != null)
        {
            foreach (BodyPart part in parts)
            {
                if (jumper == null && part is Jumper) jumper = part as Jumper;

                else if (walker == null && part is Walker) walker = part as Walker;
            }
        }

        pilotableParts = new BodyPart[] { jumper, walker };
    }    
}
