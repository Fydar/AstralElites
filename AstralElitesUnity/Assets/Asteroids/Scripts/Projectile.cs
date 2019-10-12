using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject Owner;
	public float Speed = 1.0f;
	public float Lifetime = 1.0f;

	public float LifetimeRemaining;

	private Transform thisTransform;

	private void Awake ()
	{
		thisTransform = transform;
	}

	private void Update ()
	{
		if (ScreenManager.IsOutside (thisTransform.position))
		{
			DestroyProjectile ();
			return;
		}

		LifetimeRemaining -= Time.deltaTime;
		if (LifetimeRemaining <= 0.0f)
		{
			DestroyProjectile ();
			return;
		}

		float moveDistance = Speed * Time.deltaTime;

		RaycastHit2D hit;
		hit = Physics2D.Raycast (new Vector2 (thisTransform.position.x, thisTransform.position.y),
			new Vector2 (thisTransform.right.x, thisTransform.right.y));

		if (hit.transform != null && hit.distance < moveDistance && hit.transform.gameObject != Owner)
		{
			var asteroid = hit.transform.gameObject.GetComponent<Asteroid> ();

			if (asteroid != null)
			{
				LazerParticles.Fire (hit.point, thisTransform.rotation);
				asteroid.Hit (1);

				DestroyProjectile ();
				return;
			}
		}

		thisTransform.position += thisTransform.right * moveDistance;
	}

	public void DestroyProjectile ()
	{
		Owner.GetComponent<Player> ().WeaponProjectile.Return (this);
	}
}
