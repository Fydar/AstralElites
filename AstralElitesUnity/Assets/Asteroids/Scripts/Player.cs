using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class Player : MonoBehaviour
{
	public Action<Collision2D> OnCollide;

	public SfxGroup HitSound;
	public SfxGroup DestroySound;
	public SfxGroup WarpSound;
	public SfxGroup GravelHitSound;

	[Space]
	public LoopGroup EngineSound;
	public LoopGroup AlarmSound;
	public LoopGroup ScrapingSound;

	[Header ("Health")]
	public IntEventField Health;
	public AnimationCurve DamageFromVelocity;

	public GlobalFloat DistanceTravelled;

	[Header ("Combat")]
	public float FireCooldown = 0.1f;
	public ProjectilePool WeaponProjectile;
	public SfxGroup ShootSound;

	[Space]
	public float RocketCooldown = 2.0f;
	public float RocketCooldownCurrent = 2.0f;
	public ProjectilePool RocketProjectile;
	public SfxGroup RocketSound;

	[Header ("Movement")]
	public float MovementSpeed = 10.0f;
	public float RotationSpeed = 10.0f;

	public float border = 0.1f;
	public TrailRenderer[] engineTrails;
	public ParticleSystem engineParticles;

	[Space]

	public float WarpForce = 10000.0f;

	[HideInInspector]
	public bool isAlive = true;

	private bool canFire = true;

	private Rigidbody2D rb;
	private Camera cam;
	private float lastDrag;

	private EffectFader EngineFade;
	private EffectFader AlarmFade;
	private EffectFader ScrapingFade;

	private List<Asteroid> Contacting = new List<Asteroid> ();

	private void Awake ()
	{
		WeaponProjectile.Initialise (null);

		rb = GetComponent<Rigidbody2D> ();
		cam = Camera.main;

		EngineFade = new EffectFader (new DampenInterpolator () { Speed = 5 });
		EngineFade.TargetValue = 0.0f;

		AlarmFade = new EffectFader (new DampenInterpolator () { Speed = 20 });
		AlarmFade.TargetValue = 0.0f;

		ScrapingFade = new EffectFader (new DampenInterpolator () { Speed = 20 });
		ScrapingFade.TargetValue = 0.0f;
	}

	private void Start ()
	{
		InvokeRepeating ("Fire", FireCooldown, FireCooldown);

		AudioManager.Play (EngineSound, EngineFade);
		AudioManager.Play (AlarmSound, AlarmFade);
		AudioManager.Play (ScrapingSound, ScrapingFade);

		lastDrag = rb.drag;

		Revive ();
	}

	private Vector3 lastPosition;

	private void Update ()
	{
		if (Health.Value < 45 && Health.Value > 0)
		{
			AlarmFade.TargetValue = 1.0f;
		}
		else
		{
			AlarmFade.TargetValue = 0.0f;
		}

		ScrapingFade.TargetValue = Contacting.Count == 0 ? 0.0f : 1.0f;

		if (!isAlive)
		{
			AlarmFade.TargetValue = 0.0f;
			ScrapingFade.TargetValue = 0.0f;
			EngineFade.TargetValue = 0.0f;
			return;
		}
		
		if (RocketProjectile.Template != null)
		{
			if (RocketCooldownCurrent >= 0.0f)
			{
				RocketCooldownCurrent -= Time.deltaTime;
			}
			if (RocketCooldownCurrent < 0.0f)
			{
				if (Input.GetMouseButton(1))
				{
					AudioManager.Play(RocketSound);

					var clone = RocketProjectile.Grab();
					clone.transform.SetPositionAndRotation(transform.position, transform.rotation);
					clone.LifetimeRemaining = clone.Lifetime;
					clone.Owner = gameObject;

					RocketCooldownCurrent = RocketCooldown;
				}
			}
		}

		var ray = cam.ScreenPointToRay (Input.mousePosition);

		var scenePoint = ray.origin + (ray.direction * 10);

		float AngleRad = Mathf.Atan2 (scenePoint.y - transform.position.y,
			scenePoint.x - transform.position.x);

		float AngleDeg = 180 / Mathf.PI * AngleRad;

		rb.rotation = Mathf.Lerp (rb.rotation, AngleDeg, Time.deltaTime * RotationSpeed);
		transform.rotation = Quaternion.Euler (0.0f, 0.0f, rb.rotation);

		if (Input.GetMouseButtonDown (0))
		{
			engineParticles.Play ();
			EngineFade.TargetValue = 1.0f;
		}
		if (Input.GetMouseButtonUp (0))
		{
			engineParticles.Stop ();
			EngineFade.TargetValue = 0.0f;
		}

		if (canFire && isAlive)
		{
			DistanceTravelled.Value += (lastPosition - transform.position).magnitude;
		}

		lastPosition = transform.position;
	}

	private void FixedUpdate ()
	{
		if (!isAlive)
		{
			return;
		}

		if (canFire)
		{
			ScreenManager.Clamp (transform, border);
		}

		if (Input.GetMouseButton (0))
		{
			rb.AddForce (transform.right * MovementSpeed * Time.deltaTime, ForceMode2D.Force);
		}
		else
		{
			var movementDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"), 0);
			rb.AddForce (movementDirection * MovementSpeed * Time.deltaTime, ForceMode2D.Force);
		}

		if (Contacting.Count != 0)
		{
			Health.Value -= 1;
		}
	}

	private void OnCollisionEnter2D (Collision2D collision)
	{
		Health.Value -= Mathf.RoundToInt (DamageFromVelocity.Evaluate (collision.relativeVelocity.magnitude));

		var collidingAsteroid = collision.gameObject.GetComponent<Asteroid> ();
		if (collidingAsteroid != null)
		{
			if (OnCollide != null)
			{
				OnCollide (collision);
			}

			if (Health.Value > 0)
			{
				AudioManager.Play(HitSound);
			}

			Contacting.Add (collidingAsteroid);

			collidingAsteroid.OnDestroy += () =>
			{
				Contacting.Remove (collidingAsteroid);
				collidingAsteroid.OnDestroy = null;
			};
		}
	}

	private void OnCollisionExit2D (Collision2D collision)
	{
		Contacting.Remove (collision.gameObject.GetComponent<Asteroid> ());
	}

	private void Fire ()
	{
		if (!isAlive)
		{
			return;
		}

		if (!canFire)
		{
			return;
		}

		AudioManager.Play (ShootSound);

		var clone = WeaponProjectile.Grab ();
		clone.transform.SetPositionAndRotation (transform.position, transform.rotation);
		clone.LifetimeRemaining = clone.Lifetime;

		clone.Owner = gameObject;
	}

	public void Kill ()
	{
		engineParticles.Stop ();
		isAlive = false;

		lastDrag = rb.drag;
		rb.drag = 0;
		Contacting.Clear ();
	}

	public void Revive ()
	{
		isAlive = true;
		Health.Value = 100;

		foreach (var trail in engineTrails)
		{
			trail.enabled = false;
		}

		rb.drag = lastDrag;
		rb.position = ScreenManager.RandomBorderPoint (-30);
		transform.position = rb.position;
		canFire = false;
		Invoke ("WarpIntoScene", 0.25f);
		Invoke ("StartShooting", 1.25f);
		Contacting.Clear ();

		foreach (var trail in engineTrails)
		{
			trail.enabled = true;
		}
	}

	private void WarpIntoScene ()
	{
		AudioManager.Play (WarpSound);

		Vector3 direction = -(rb.position);

		rb.AddForce (direction * WarpForce, ForceMode2D.Impulse);
	}

	private void StartShooting ()
	{
		canFire = true;
		lastPosition = transform.position;
	}
}
