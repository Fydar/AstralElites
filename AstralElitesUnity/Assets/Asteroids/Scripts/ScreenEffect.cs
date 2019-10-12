using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScreenEffect : MonoBehaviour
{
	public static ScreenEffect instance;

	public Spring Spring;
	public float PulseVelocity = 10;

	private PostProcessVolume volume;

	private void Awake ()
	{
		volume = GetComponent<PostProcessVolume> ();
		instance = this;
	}

	private void Update ()
	{
		Spring.Update (Time.deltaTime);

		volume.weight = Spring.Value;
	}

	public void Pulse (float strength)
	{
		Spring.Velocity += strength * PulseVelocity;
	}
}