using UnityEngine;

public static class Bezier
{
    public static Vector3 GetPointQuadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t) 
    {
        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    public static Vector3 GetPointCubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 A = Vector3.Lerp(p0, p1, t);
        Vector3 B = Vector3.Lerp(p1, p2, t);
        Vector3 C = Vector3.Lerp(p2, p3, t);

        Vector3 D = Vector3.Lerp(A, B, t);
        Vector3 E = Vector3.Lerp(B, C, t);

        Vector3 F = Vector3.Lerp(D, E, t);

        return F;
    }

    public static Vector3 GetPointCubicPolynomial(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        return 
        (1f-t) * (1f-t) * (1f-t) * p0 +
        3f * (1f-t) * (1f-t) * t * p1 +
        3f * (1f-t) * t * t * p2 +
        t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
		return
			3f * (1f-t) * (1f-t) * (p1 - p0) +
			6f * (1f-t) * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}
}
