using UnityEngine;

[CreateAssetMenu]
public class AsteroidTemplate : ScriptableObject
{
	public int Health = 1;

	[Space]
	public float MinScale = 0.5f;
	public float MaxScale = 1.0f;

	[Space]

	public float MinSpeed = 5.0f;
	public float MaxSpeed = 10.0f;

	[Space]

	public int MinSegments = 5;
	public int MaxSegments = 9;

	[Space]

	public float Variation = 0.2f;

	[Header ("Death")]
	public int Reward = 50;
	[Space]
	public AsteroidTemplate Spawn;
	public SfxGroup DestroySound;
	public SfxGroup ImpactSound;
	public int MinSpawn;
	public int MaxSpawn;

	public void Generate (Asteroid clone)
	{
		clone.Generate (this);
	}

	public void Scatter (Vector3 point, float range, Vector3 velocity, int count = 1)
	{
		for (int i = count - 1; i >= 0; --i)
		{
			var clone = AsteroidGenerator.instance.AsteroidPool.Grab (null);

			clone.Generate (this);

			var rand = Random.insideUnitCircle;

			clone.transform.position = point + (new Vector3 (rand.x, rand.y, 0) * range * 0.5f);

			clone.GetComponent<Rigidbody2D> ().velocity = velocity;
		}
	}
}