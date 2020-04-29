using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotableBlob : Pilotable
{
    //public Feet feet;
    public Jumper jumper;
    public Spinner spinner;
    public Swimmer swimmer;

    public PilotableBlob(BodyPart[] parts)
    {
        SetPilotingType(PilotingType.Blob);

        LocalGravity gravity = null;

        if(parts != null)
        {
            foreach(BodyPart part in parts)
            {
                /*if (feet == null && part is Feet) feet = part as Feet;
                
                else*/ if(jumper == null && part is Jumper) jumper = part as Jumper;

                else if (spinner == null && part is Spinner) spinner = part as Spinner;
                
                else if (swimmer == null && part is Swimmer) swimmer = part as Swimmer;

                else if (gravity == null && part is LocalGravity) gravity = part as LocalGravity;
            }
        }

        pilotableParts = new BodyPart[] { jumper, spinner, swimmer, gravity };
    }
}
