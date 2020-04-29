using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotableBot : Pilotable
{
    //public Feet feet;
    public Jumper jumper;
    public Spinner spinner;
    public Walker walker;

    public PilotableBot(BodyPart[] parts)
    {
        SetPilotingType(PilotingType.Bot);

        //LocalGravity gravity = null;

        if (parts != null)
        {
            foreach (BodyPart part in parts)
            {
                /*if (feet == null && part is Feet) feet = part as Feet;

                else*/ if (jumper == null && part is Jumper) jumper = part as Jumper;

                else if (spinner == null && part is Spinner) spinner = part as Spinner;

                else if (walker == null && part is Walker) walker = part as Walker;

                /*else if (gravity == null && part is LocalGravity) gravity = part as LocalGravity;*/
            }
        }

        pilotableParts = new BodyPart[] { jumper, spinner, walker/*, gravity*/ };
    }
}
