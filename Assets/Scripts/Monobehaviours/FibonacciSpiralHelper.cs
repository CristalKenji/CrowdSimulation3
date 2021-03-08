using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FibonacciSpiralHelper
{

    private static float goldenRatio = (Mathf.Sqrt(5.0f) + 1.0f) / 2.0f;
    private static float goldenAngle = (2.0f - goldenRatio) * (2 * Mathf.PI);
    public static Vector3[] GenerateUnitSphere(int numberOfPoints)
    {
        Vector3[] result = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float elevation = Mathf.Asin(-1.0f + 2.0f * i / (numberOfPoints + 1));
            float azimuth = goldenAngle * i;
            result[i] = new Vector3(Mathf.Cos(azimuth) * Mathf.Cos(elevation), Mathf.Sin(azimuth) * Mathf.Cos(elevation), -Mathf.Sin(elevation));
        }
        return result;
    }
}
