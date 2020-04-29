using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotableBug : Pilotable
{
    public Walker walker;
    public GearBox gearBox;
    public FlatGroundProbe groundProbe;

    public PilotableBug(BodyPart[] parts)
    {
        SetPilotingType(PilotingType.Bug);

        LocalGravity gravity = null;

        if (parts != null)
        {
            foreach (BodyPart part in parts)
            {
                if (walker == null && part is Walker) walker = part as Walker;

                else if (gearBox == null && part is GearBox) gearBox = part as GearBox;

                else if (groundProbe == null && part is FlatGroundProbe) groundProbe = part as FlatGroundProbe;

                else if (gravity == null && part is LocalGravity) gravity = part as LocalGravity;
            }
        }

        pilotableParts = new BodyPart[] { walker, gearBox, gravity };
    }
}
