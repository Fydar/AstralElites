using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
	public static AsteroidGenerator instance;

	[Header ("Gameplay")]
	public float SpawnCooldown = 0.25f;

	[Header ("Asteroids")]
	public AsteroidTemplate[] AsteroidTypes;
	public AsteroidsPool AsteroidPool;

	private void Awake ()
	{
		instance = this;
	}

	private void Regenerate ()
	{
		if (!isActiveAndEnabled)
		{
			return;
		}

		var template = AsteroidTypes[UnityEngine.Random.Range (0, AsteroidTypes.Length)];

		template.Generate (AsteroidPool.Grab (null));
	}

	public void Disable ()
	{
		CancelInvoke ();
	}

	public void Enable ()
	{
		InvokeRepeating ("Regenerate", 1.75f, SpawnCooldown);
	}
}