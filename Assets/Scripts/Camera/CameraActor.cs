using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class CameraActor : MonoBehaviour
{
    public class CameraEvent : UnityEvent<CameraSettings> { }
    public class InsideOutsideEvent : UnityEvent<bool> { }

    public string entranceTag = "Entrance";
    public string exitTag = "Exit";

    public CameraEvent OnCameraChange;
    public InsideOutsideEvent OnGetInOut;

    private List<CameraRoom> enteredRooms;

    private void Awake()
    {
        OnCameraChange = new CameraEvent();
        OnGetInOut = new InsideOutsideEvent();

        enteredRooms = new List<CameraRoom>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CameraRoom room = collision.GetComponent<CameraRoom>();
        if (room != null && enteredRooms.Contains(room) == false)
        {
            if (enteredRooms.Count == 0)
            {
                OnCameraChange.Invoke(room.enterSettings);
                enteredRooms.Add(room);
            }
            else if (enteredRooms.Contains(room) == false)
                enteredRooms.Add(room);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enteredRooms.Count > 0)
        {
            CameraRoom room = collision.GetComponent<CameraRoom>();
            if (room != null)
            {
                if (room == enteredRooms[0])
                {
                    if (enteredRooms.Count > 1) OnCameraChange.Invoke(enteredRooms[1].enterSettings);
                }

                enteredRooms.Remove(room);
            }
        }        
    }
}
