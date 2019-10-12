using System;
using UnityEngine;

public struct Polygon
{
	public Vector2[] Points;

	public Polygon (Vector2[] points)
	{
		Points = points;
	}

	public Polygon Inset (float amount)
	{
		var newPoints = new Vector2[Points.Length];

		var a = Points[newPoints.Length - 2];
		var b = Points[newPoints.Length - 1];

		for (int i = 0; i < Points.Length; i++)
		{
			var c = Points[i];

			var insetABDirection = Vector2.Perpendicular (b - a).normalized;
			var insetBCDirection = Vector2.Perpendicular (c - b).normalized;

			var insetA = a - insetABDirection * amount;
			var insetB = b - insetABDirection * amount;

			var insetC = b - insetBCDirection * amount;
			var insetD = c - insetBCDirection * amount;

			newPoints[i] = FindIntersection (insetA, insetB, insetC, insetD);

			a = b;
			b = c;
		}

		return new Polygon (newPoints);
	}

	public static Polygon Random (int segments, float variation, float scale)
	{
		float spacing = 360.0f / segments;

		var rotator = Quaternion.AngleAxis (spacing, Vector3.up);

		var points = new Vector2[segments];

		var current = Vector3.forward;
		for (int i = 0; i < segments; i++)
		{
			float distance = Mathf.Lerp (variation, 1.0f, UnityEngine.Random.value) * scale;
			var rot = current * distance;
			points[i] = new Vector2 (rot.x, rot.z);

			current = rotator * current;
		}

		return new Polygon (points);
	}

	static Vector2 FindIntersection (Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
	{
		float a1 = e1.y - s1.y;
		float b1 = s1.x - e1.x;
		float c1 = a1 * s1.x + b1 * s1.y;

		float a2 = e2.y - s2.y;
		float b2 = s2.x - e2.x;
		float c2 = a2 * s2.x + b2 * s2.y;

		float delta = a1 * b2 - a2 * b1;

		return Math.Abs (delta) < 0.001f
			? new Vector2 (float.NaN, float.NaN)
			: new Vector2 ((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
	}
}
