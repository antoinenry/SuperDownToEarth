using UnityEngine;

public class RogueBrain : MonoBehaviour
{
    private Pilot pilot;
    private AIBehaviour currentAI;

    private void Awake()
    {
        pilot = GetComponent<Pilot>();
    }

    private void Start()
    {
        pilot.currentVehicle.AddValueListener<Component>(OnPilotingVehicle, true);
    }

    private void OnDestroy()
    {
        pilot.currentVehicle.RemoveValueListener<Component>(OnPilotingVehicle);
    }

    private void OnPilotingVehicle(Component piloted)
    {
        if (currentAI != null) currentAI.enabled = false;

        if (piloted != null)
            currentAI = piloted.GetComponentInChildren<AIBehaviour>(true);
        else
            currentAI = GetComponent<AIBehaviour>();

        if (currentAI != null) currentAI.enabled = true;
    }
}