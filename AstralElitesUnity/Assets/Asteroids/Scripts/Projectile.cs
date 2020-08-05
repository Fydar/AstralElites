using UnityEngine;

public class Projectile : MonoBehaviour
{
	public string EffectId;
	public float Speed = 1.0f;
	public int Damage = 1;
	public float Lifetime = 1.0f;
	public bool ExplodeOnDeath;

	[Header("Area of Effect")]
	public bool HasAreaOfEffect;
	public float AreaOfEffect;

	[Header("Realtime")]
	public GameObject Owner;
	public float LifetimeRemaining;

	private Transform thisTransform;

	private void Awake()
	{
		thisTransform = transform;
	}

	private void Update()
	{
		if (ScreenManager.IsOutside(thisTransform.position))
		{
			DestroyProjectile();
			return;
		}

		LifetimeRemaining -= Time.deltaTime;
		if (LifetimeRemaining <= 0.0f)
		{
			DestroyProjectile();
			return;
		}

		float moveDistance = Speed * Time.deltaTime;

		RaycastHit2D hit;
		hit = Physics2D.Raycast(new Vector2(thisTransform.position.x, thisTransform.position.y),
			new Vector2(thisTransform.right.x, thisTransform.right.y));

		if (hit.transform != null && hit.distance < moveDistance && hit.transform.gameObject != Owner)
		{
			var asteroid = hit.transform.gameObject.GetComponent<Asteroid>();

			if (asteroid != null)
			{
				PulseEffect.Instances[EffectId].PlayAt(hit.point, thisTransform.rotation);
				asteroid.Hit(Damage);

				DestroyProjectile();
			}

			if (HasAreaOfEffect)
			{
				var colliders = Physics2D.OverlapCircleAll(hit.point, AreaOfEffect);

				foreach (var collider in colliders)
				{
					if (collider.gameObject != asteroid.gameObject)
					{
						var otherAsteroid = collider.gameObject.GetComponent<Asteroid>();

						if (otherAsteroid != null)
						{
							otherAsteroid.Hit(Damage);
						}
					}
				}
			}
		}

		thisTransform.position += thisTransform.right * moveDistance;
	}

	public void DestroyProjectile()
	{
		if (ExplodeOnDeath)
		{
			PulseEffect.Instances[EffectId].PlayAt(thisTransform.position, thisTransform.rotation);
		}
		if (EffectId == "Explosion2")
		{
			Owner.GetComponent<Player>().RocketProjectile.Return(this);
		}
		else
		{
			Owner.GetComponent<Player>().WeaponProjectile.Return(this);
		}

		if (HasAreaOfEffect)
		{
			var colliders = Physics2D.OverlapCircleAll(transform.position, AreaOfEffect);

			foreach (var collider in colliders)
			{
				var otherAsteroid = collider.gameObject.GetComponent<Asteroid>();

				if (otherAsteroid != null)
				{
					otherAsteroid.Hit(Damage);
				}
			}
		}
	}
}
