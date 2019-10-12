using UnityEngine;

public class ScreenManager : MonoBehaviour
{
	public static Vector3 MinPosition;
	public static Vector3 MaxPosition;
	public static Vector2 Scale;

	[SerializeField]
	private Camera cam;

	private Vector2 lastResolution;

	public Transform[] ScaleToScene;

	private void Awake ()
	{
		if (cam == null)
		{
			cam = Camera.main;
		}

		Recalculate ();
	}

	private void Update ()
	{
		var resolution = new Vector2 (Screen.height, Screen.width);

		if (lastResolution != resolution)
		{
			Recalculate ();

			lastResolution = resolution;
		}
	}

	private void Recalculate ()
	{
		var minRay = cam.ViewportPointToRay (Vector3.zero);
		var maxRay = cam.ViewportPointToRay (Vector3.one);

		MinPosition = new Vector3 (minRay.origin.x, minRay.origin.y);
		MaxPosition = new Vector3 (maxRay.origin.x, maxRay.origin.y);

		Scale = new Vector2 (MaxPosition.x - MinPosition.x,
			MaxPosition.y - MinPosition.y);

		foreach (var scaleObject in ScaleToScene)
		{
			scaleObject.localScale = new Vector3 (Scale.x, Scale.y, scaleObject.localScale.z);
		}
	}

	public static void Clamp (Transform target, float border = 0.0f)
	{
		var minPosition = new Vector3 (MinPosition.x + border, MinPosition.y + border);
		var maxPosition = new Vector3 (MaxPosition.x - border, MaxPosition.y - border);

		if (target.position.x < minPosition.x)
		{
			target.position = new Vector3 (minPosition.x, target.position.y, 0);
		}
		else if (target.position.x > maxPosition.x)
		{
			target.position = new Vector3 (maxPosition.x, target.position.y, 0);
		}

		if (target.position.y < minPosition.y)
		{
			target.position = new Vector3 (target.position.x, minPosition.y, 0);
		}
		else if (target.position.y > maxPosition.y)
		{
			target.position = new Vector3 (target.position.x, maxPosition.y, 0);
		}
	}

	public static bool IsOutside (Vector3 position, float border = 0.0f)
	{
		var minPosition = new Vector3 (MinPosition.x + border, MinPosition.y + border);
		var maxPosition = new Vector3 (MaxPosition.x - border, MaxPosition.y - border);

		if (position.x < minPosition.x)
		{
			return true;
		}
		else if (position.x > maxPosition.x)
		{
			return true;
		}

		if (position.y < minPosition.y)
		{
			return true;
		}
		else if (position.y > maxPosition.y)
		{
			return true;
		}

		return false;
	}

	public static Vector3 RandomBorderPoint (float border = 0.0f)
	{
		var minPosition = new Vector3 (MinPosition.x + border, MinPosition.y + border);
		var maxPosition = new Vector3 (MaxPosition.x - border, MaxPosition.y - border);

		float rand = Random.value;
		switch (Random.Range (0, 4))
		{
			case 0:
				return new Vector3 (Mathf.Lerp (minPosition.x, maxPosition.x, rand), maxPosition.y, 0);
			case 1:
				return new Vector3 (Mathf.Lerp (minPosition.x, maxPosition.x, rand), minPosition.y, 0);
			case 2:
				return new Vector3 (maxPosition.x, Mathf.Lerp (minPosition.y, maxPosition.y, rand), 0);
			case 3:
				return new Vector3 (minPosition.x, Mathf.Lerp (minPosition.y, maxPosition.y, rand), 0);
		}

		return Vector3.zero;
	}

	public static Vector3 ScreenToWorld (Vector2 position)
	{
		return new Vector3 (
			Mathf.Lerp (MinPosition.x, MaxPosition.x, position.x),
			Mathf.Lerp (MinPosition.y, MaxPosition.y, position.y), 0);
	}
}
