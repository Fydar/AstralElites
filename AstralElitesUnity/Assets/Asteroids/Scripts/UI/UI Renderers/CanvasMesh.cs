using UnityEngine;
using UnityEngine.UI;

public class CanvasMesh : Graphic
{
	public Mesh mesh;

	protected override void OnPopulateMesh (VertexHelper vh)
	{
		vh.Clear ();

		if (mesh == null)
			return;

		Vector3 boundsSize = mesh.bounds.extents;

		Vector3 multiplier = new Vector3 ((Mathf.Abs (rectTransform.rect.width) / boundsSize.x) * -0.5f,
			(Mathf.Abs (rectTransform.rect.height) / boundsSize.y) * 0.5f, 0);

		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			Vector3 vertPos = mesh.vertices[i];
			vertPos.Scale (multiplier);
			vh.AddVert (vertPos, color, Vector2.zero);
		}
		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			vh.AddTriangle (mesh.triangles[i], mesh.triangles[i + 1], mesh.triangles[i + 2]);
		}
	}
}