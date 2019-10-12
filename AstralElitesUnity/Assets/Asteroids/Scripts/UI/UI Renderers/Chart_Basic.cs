using UnityEngine;
using UnityEngine.UI;

public abstract class Chart_Basic : Graphic
{
	public float MinYMax = 1.0f;
	public Chart_Point[] Points;

	public void SetData (Chart_Point[] points)
	{
		Points = points;
		this.SetVerticesDirty ();
	}

	public void SetData (float[] values)
	{
		float maxValue = MinYMax;

		Points = new Chart_Point[values.Length];
		for (int i = 0; i < Points.Length; i++)
		{
			float value = values[i];
			maxValue = Mathf.Max (value, maxValue);
		}

		if (Points.Length == 0)
		{
			Points[0] = new Chart_Point (0.0f, Points[0].Value / maxValue);
		}
		else
		{
			for (int i = 0; i < Points.Length; i++)
			{
				float value = values[i];
				Points[i] = new Chart_Point ((float)i / (Points.Length - 1), value / maxValue);
			}
		}
		this.SetVerticesDirty ();
	}

	public void SetNormalizedData (float[] values)
	{
		Points = new Chart_Point[values.Length];
		for (int i = 0; i < Points.Length; i++)
		{
			Points[i] = new Chart_Point ((float)i / (Points.Length - 1), values[i]);
		}
		this.SetVerticesDirty ();
	}

	protected override void OnPopulateMesh (VertexHelper vh)
	{
		vh.Clear ();

		Rect rect = rectTransform.rect;

		int vertIndex = 0;

		for (int i = 0; i < Points.Length; i++)
		{
			float time = Points[i].Time;
			float height = Points[i].Value;

			float yPos = Mathf.Lerp (rect.yMin, rect.yMax, height);
			float xPos = Mathf.Lerp (rect.xMin, rect.xMax, time);

			vh.AddVert (new Vector3 (xPos, yPos, 0), color, Vector2.zero);
			vh.AddVert (new Vector3 (xPos, rect.yMin, 0), color, Vector2.zero);

			if (vertIndex > 1)
			{
				vh.AddTriangle (vertIndex - 2, vertIndex, vertIndex + 1);
				vh.AddTriangle (vertIndex - 2, vertIndex + 1, vertIndex - 1);
			}

			vertIndex += 2;
		}
		if (Points.Length == 1)
		{
			float time = 1.0f;
			float height = Points[0].Value;

			float yPos = Mathf.Lerp (rect.yMin, rect.yMax, height);
			float xPos = Mathf.Lerp (rect.xMin, rect.xMax, time);

			vh.AddTriangle (vertIndex - 2, vertIndex, vertIndex + 1);
			vh.AddTriangle (vertIndex - 2, vertIndex + 1, vertIndex - 1);
		}
	}
}