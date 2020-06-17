using UnityEngine;

public class Circuit : MonoBehaviour
{
    public enum CycleType { Single, Loop, PingPong }
        
    [Range(0, 5)] public int numCornerVertices;
    [Min(0f)] public float cornerRadius;
    public CycleType cycleType;

    [SerializeField]
    private Vector2[] controlPoints = new Vector2[0];
    [HideInInspector] [SerializeField]
    private Vector2[] trajectory = new Vector2[0];

    private void Awake()
    {
        UpdateTrajectory();
    }
        
    public void OnDrawGizmosSelected()
    {
        Vector2 circuitPosition = transform.position;

        int length = controlPoints.Length;
        if (length < 2) return;
        Gizmos.color = Color.gray;
        for (int i = 0; i < length; i++)
            Gizmos.DrawLine(circuitPosition, circuitPosition + controlPoints[i]);

        length = trajectory.Length;
        if (length < 2) return;
        Gizmos.color = Color.magenta;
        for (int i = 0; i < length - 1; i++)
            Gizmos.DrawLine(circuitPosition + trajectory[i], circuitPosition + trajectory[i+1]);
        if (cycleType == CycleType.Loop) Gizmos.DrawLine(circuitPosition + trajectory[length - 1], circuitPosition + trajectory[0]);
    }

    public Vector2 GetPosition(int index)
    {
        if (trajectory == null || index < 0 || index >= trajectory.Length) return transform.position;
        else return (Vector2)transform.position + trajectory[index];
    }

    public int GetCorrectPositionIndex(int index)
    {
        int correctIndex = -1;
        if (trajectory != null && trajectory.Length != 0)
        {
            if (trajectory.Length == 1)
            {
                correctIndex = 0;
            }
            else
            {
                switch (cycleType)
                {
                    case CycleType.Single:
                        correctIndex = Mathf.Clamp(index, 0, trajectory.Length - 1);
                        break;
                    case CycleType.Loop:
                        correctIndex = (int)Mathf.Repeat(index, trajectory.Length);
                        break;
                    case CycleType.PingPong:
                        correctIndex = (int)Mathf.PingPong(index, trajectory.Length - 1);
                        break;
                }
            }
        }

        return correctIndex;
    }

    public void UpdateTrajectory()
    {
        trajectory = ChaikinSmoothLine.Smoothen(controlPoints, cycleType == CycleType.Loop, cornerRadius, numCornerVertices);
    }
}