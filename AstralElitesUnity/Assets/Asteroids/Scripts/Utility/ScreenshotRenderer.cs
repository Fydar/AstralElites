using UnityEngine;

public class ScreenshotRenderer : MonoBehaviour
{
	public KeyCode Capture;

	[Space]

	public int resWidth = 2048;
	public int resHeight = 2048;

	[Space]

	public Camera camera;

	private void Update ()
	{
		if (Input.GetKeyDown (Capture))
		{
			var rt = new RenderTexture (resWidth, resHeight, 24);

			rt.antiAliasing = 8;

			camera.targetTexture = rt;
			var screenShot = new Texture2D (resWidth, resHeight, TextureFormat.RGBA32, false);
			camera.Render ();
			RenderTexture.active = rt;
			screenShot.ReadPixels (new Rect (0, 0, resWidth, resHeight), 0, 0);
			camera.targetTexture = null;
			RenderTexture.active = null;
			Destroy (rt);

			byte[] bytes = screenShot.EncodeToPNG ();
			string filename = ScreenShotName (resWidth, resHeight);

			System.IO.File.WriteAllBytes (filename, bytes);

			Debug.Log (string.Format ("Took screenshot to: {0}", filename));
		}
	}

	public static string ScreenShotName (int width, int height)
	{
		return string.Format ("{0}/screenshots/screen_{1}x{2}_{3}.png",
							 Application.dataPath,
							 width, height,
							 System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"));
	}
}
