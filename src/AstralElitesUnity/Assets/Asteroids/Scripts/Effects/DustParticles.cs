using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DustParticles : MonoBehaviour, ISerializationCallbackReceiver
{
    private static DustParticles instance;

    private static ParticleSystem.Particle[] cache;

    public Vector3 Offset;


    [SerializeField]
    private int emissionCount;

    private ParticleSystem particles;

    public void OnAfterDeserialize()
    {
        instance = this;
    }

    public void OnBeforeSerialize()
    {
    }

    public void FireOnTarget(MeshFilter target)
    {
        if (particles == null)
        {
            particles = GetComponent<ParticleSystem>();
            cache = new ParticleSystem.Particle[particles.main.maxParticles];
        }

        Vector3 Velocity = target.transform.parent.GetComponent<Rigidbody2D>().velocity;
        int emitCount = Mathf.RoundToInt(emissionCount * target.mesh.bounds.extents.magnitude);

        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Mesh;
        shape.mesh = target.mesh;

        particles.transform.SetPositionAndRotation(target.transform.position + Offset, target.transform.rotation);

        particles.Emit(emitCount);
        int countBefore = particles.GetParticles(cache);

        for (int i = 0; i < emitCount; i++)
        {
            int index = countBefore - i;

            cache[index].velocity += Velocity;
        }

        particles.SetParticles(cache, countBefore);

    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            AudioManager.Play(player.GravelHitSound);
        }
    }

    public static void Fire(MeshFilter target)
    {
        instance.FireOnTarget(target);
    }
}
