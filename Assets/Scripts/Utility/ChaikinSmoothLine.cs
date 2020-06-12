using System.Collections.Generic;
using UnityEngine;

public class ChaikinSmoothLine
{
    static public Vector2[] Smoothen(Vector2[] positions, bool loop)
    {
        if (positions == null) return new Vector2[0];
        if (positions.Length < 3)
        {
            Vector2[] newPositions = new Vector2[positions.Length];
            positions.CopyTo(newPositions, 0);
            return newPositions;
        }
        else
        {
            List<Vector2> newPositions = new List<Vector2>();
            newPositions.Add(positions[0]);

            for (int i = 0, length = positions.Length, imax = loop ? length : length - 2; i < imax; i++)
            {
                Vector2[] sub = DivideAngle(positions[i], positions[(i + 1) % length], positions[(i + 2) % length]);
                newPositions.AddRange(sub);
            }

            if (loop == false)
            {
                newPositions.Add(positions[positions.Length - 1]);
            }
            else
            {
                int lastIndex = newPositions.Count - 1;
                newPositions[0] = newPositions[lastIndex];
                newPositions.RemoveAt(lastIndex);
            }

            return newPositions.ToArray();
        }
    }

    static public Vector2[] Smoothen(Vector2[] positions, bool loop, float cornerRadius, int numIterations)
    {
        if (numIterations == 0 || cornerRadius <= 0f)
        {
            Vector2[] newPositions = new Vector2[positions.Length];
            positions.CopyTo(newPositions, 0);
            return newPositions;
        }
        else
        {
            List<Vector2> addPositions = new List<Vector2>();
            for (int i = 0, imax = positions.Length; i < imax; i++)
            {
                Vector2 current = positions[i];
                Vector2 next = positions[(i + 1) % imax];
                float segmentLength = Vector2.Distance(current, next);

                addPositions.Add(current);
                if (i == imax - 1 && loop == false) break;

                if (segmentLength > 2f * cornerRadius)
                {
                    Vector2 segmentDirection = (next - current).normalized;
                    addPositions.Add(current + cornerRadius * segmentDirection);
                    addPositions.Add(next - cornerRadius * segmentDirection);
                }
                else if (segmentLength > cornerRadius)
                {
                    addPositions.Add((current + next) / 2f);
                }
            }

            Vector2[] newPositions = new Vector2[addPositions.Count];
            addPositions.CopyTo(newPositions, 0);

            
            for (int i = 0; i < numIterations; i++)
                newPositions = Smoothen(newPositions, loop);                

            return newPositions;
        }
    }

    static public Vector2[] DivideAngle(Vector2 A, Vector2 B, Vector2 C)
    {
        if (A == B || B == C || C == A) return new Vector2[] { B };


        if (Mathf.Pow(Vector2.Dot(A-B, A-C), 2f) == Vector2.SqrMagnitude(A-B) * Vector2.SqrMagnitude(A-C))
            return new Vector2[] { B };

        Vector2[] output = new Vector2[2];
        output[0] = A / 4f + 3f * B / 4f;
        output[1] = 3f * B / 4f + C / 4f;

        return output;
    }
}