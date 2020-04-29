using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class Pilotable
{
    public enum PartEnabled { AllDisabled, AllEnabled, Mixed, NoParts }
    public enum PilotingType { None, Blob, Bot, Bug, Jet, Buzz, Rogue }
    [SerializeField] private PilotingType type;

    protected BodyPart[] pilotableParts;

    public Pilotable New(BodyPart[] parts)
    {
        switch(type)
        {
            case PilotingType.Blob: return new PilotableBlob(parts);
            case PilotingType.Bot: return new PilotableBot(parts);
            case PilotingType.Bug: return new PilotableBug(parts);
            case PilotingType.Jet: return new PilotableJet(parts);
            case PilotingType.Buzz: return new PilotableBuzz(parts);
            case PilotingType.Rogue: return new PilotableRogue(parts);
            default: return null;
        }
    }

    public PilotingType GetPilotingType()
    {
        return type;
    }

    protected void SetPilotingType(PilotingType pt)
    {
        type = pt;
    }

    public PartEnabled Enabled
    {
        get
        {
            if (pilotableParts == null || pilotableParts.Length == 0)
                return PartEnabled.NoParts;
            else
            {
                bool outEnabled = pilotableParts[0].enabled;
                foreach(BodyPart part in pilotableParts)
                {
                    if (part != null && part.enabled != outEnabled) return PartEnabled.Mixed;
                }
                return outEnabled ? PartEnabled.AllEnabled : PartEnabled.AllDisabled;
            }
        }

        set
        {
            bool enable;

            switch(value)
            {
                case PartEnabled.AllDisabled: enable = false; break;
                case PartEnabled.AllEnabled: enable = true; break;
                default: return;
            }

            if (pilotableParts != null)
                foreach (BodyPart part in pilotableParts)
                    if(part != null) part.enabled = enable;
        }
    }
}