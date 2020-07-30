using UnityEngine;

[RequireComponent (typeof (ParticleSystem))]
public class DustParticles : MonoBehaviour, ISerializationCallbackReceiver
{
	private static DustParticles instance;

	private static ParticleSystem.Particle[] cache;

	public Vector3 Offset;


	[SerializeField]
	private int emissionCount;

	private ParticleSystem particles;

	public void OnAfterDeserialize ()
	{
		instance = this;
	}

	public void OnBeforeSerialize()
	{
	}

	public void FireOnTarget (MeshFilter target)
	{
		if (particles == null)
		{
			particles = GetComponent<ParticleSystem>();
			cache = new ParticleSystem.Particle[particles.main.maxParticles];
		}

		Vector3 Velocity = target.transform.parent.GetComponent<Rigidbody2D> ().velocity;
		int emitCount = Mathf.RoundToInt (emissionCount * target.mesh.bounds.extents.magnitude);

		var sh = particles.shape;
		sh.enabled = true;
		sh.shapeType = ParticleSystemShapeType.Mesh;
		sh.mesh = target.mesh;

		particles.transform.position = target.transform.position + Offset;
		particles.transform.rotation = target.transform.rotation;

		particles.Emit (emitCount);
		int countBefore = particles.GetParticles (cache);

		for (int i = 0; i < emitCount; i++)
		{
			int index = countBefore - i;

			cache[index].velocity += Velocity;
		}

		particles.SetParticles (cache, countBefore);

	}

	private void OnParticleCollision (GameObject other)
	{
		var player = other.GetComponent<Player> ();

		if (player != null)
		{
			AudioManager.Play (player.GravelHitSound);
		}
	}

	public static void Fire (MeshFilter target)
	{
		instance.FireOnTarget (target);
	}
}