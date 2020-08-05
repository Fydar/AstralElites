using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
	public Action OnDestroy;

	[Header("Setup")]
	public MeshFilter body;
	public MeshFilter outline;
	public PolygonCollider2D collider;

	[Header("Style")]
	public float Width;

	private AsteroidTemplate template;
	private int health;
	private float scale;
	private int segments;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		InvokeRepeating("CleanupCheck", 3.0f, 1.0f);
	}

	public void Generate(AsteroidTemplate template)
	{
		this.template = template;

		health = this.template.Health;
		scale = UnityEngine.Random.Range(this.template.MinScale, this.template.MaxScale);
		segments = UnityEngine.Random.Range(this.template.MinSegments, this.template.MaxSegments - 1);

		var outsidePolygon = Polygon.Random(segments, this.template.Variation, scale);
		var insidePolygon = outsidePolygon.Inset(Width);

		collider.points = insidePolygon.Points;
		body.mesh = collider.CreateMesh(false, false);

		collider.points = outsidePolygon.Points;
		outline.mesh = collider.CreateMesh(false, false);

		transform.position = ScreenManager.RandomBorderPoint(-outline.mesh.bounds.size);
		Fling(UnityEngine.Random.Range(this.template.MinSpeed, this.template.MaxSpeed));
	}

	public void Hit(int damage)
	{
		health -= damage;

		if (health <= 0)
		{
			Kill();
			AudioManager.Play(template.DestroySound);
		}
		else
		{
			AudioManager.Play(template.ImpactSound);
		}
	}

	public void Kill()
	{
		DustParticles.Fire(body);
		if (template.Spawn != null)
		{
			template.Spawn.Scatter(transform.position, scale, rb.velocity * 0.5f, UnityEngine.Random.Range(template.MinSpawn, template.MaxSpawn + 1));
		}

		GameManager.ScorePoints(template.Reward);
		AsteroidGenerator.instance.AsteroidPool.Return(this);

		if (OnDestroy != null)
		{
			OnDestroy();
		}
	}

	private void CleanupCheck()
	{
		if (!gameObject.activeInHierarchy)
		{
			return;
		}

		if (ScreenManager.IsOutside(transform.position, (-scale) - 0.5f))
		{
			AsteroidGenerator.instance.AsteroidPool.Return(this);
		}
	}

	private void Fling(float velocity)
	{
		var direction = -(transform.position).normalized;

		var rotator = Quaternion.AngleAxis(UnityEngine.Random.Range(-45.0f, 45.0f), Vector3.forward);

		direction = rotator * direction;

		rb.AddForce(direction * velocity, ForceMode2D.Impulse);
	}

	public static Mesh Trianglulate(Vector2[] points)
	{
		var verts = new Vector3[points.Length + 1];
		var normals = new Vector3[verts.Length];
		verts[0] = Vector3.zero;
		normals[0] = Vector3.up;

		for (int i = 0; i < points.Length; i++)
		{
			verts[i + 1] = points[i];
			normals[i + 1] = Vector3.up;
		}

		var mesh = new Mesh();
		int[] tris = new int[verts.Length * 3];

		int currentIndex = 0;
		for (int i = 1; i < verts.Length; i++)
		{
			tris[currentIndex] = 0;
			tris[currentIndex + 1] = i - 1;
			tris[currentIndex + 2] = i;

			currentIndex += 3;
		}

		tris[currentIndex] = 0;
		tris[currentIndex + 1] = verts.Length - 1;
		tris[currentIndex + 2] = 1;

		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.normals = normals;

		return mesh;
	}
}
