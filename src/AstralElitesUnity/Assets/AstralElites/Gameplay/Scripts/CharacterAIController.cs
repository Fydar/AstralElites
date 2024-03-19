using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterAIController : MonoBehaviour
{
    private Character character;
    private Camera mainCamera;

    private Vector3 currentTarget;

    private void Awake()
    {
        character = GetComponent<Character>();
        mainCamera = Camera.main;

        currentTarget = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    private void Update()
    {
        if (!character.isAlive)
        {
            return;
        }

        var ray = mainCamera.ScreenPointToRay(currentTarget);

        var scenePoint = ray.origin + (ray.direction * 10);

        if (Vector3.Distance(transform.position, scenePoint) < 1.0f)
        {
            currentTarget = new Vector3(
                Random.Range(Screen.width * 0.25f, Screen.width * 0.75f),
                Random.Range(Screen.height * 0.25f, Screen.height * 0.75f),
                0.0f);
        }

        float angleRadians = Mathf.Atan2(scenePoint.y - transform.position.y,
            scenePoint.x - transform.position.x);

        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        character.inputRotation = angleDegrees + (Mathf.Sin(Time.time * 3) * 60);
        character.inputThrust = transform.right;

        if (character.RocketProjectile.Template != null)
        {
            if (character.RocketCooldownCurrent >= 0.0f)
            {
                character.RocketCooldownCurrent -= Time.deltaTime;
            }
            if (character.RocketCooldownCurrent < 0.0f)
            {
                if (Input.GetMouseButton(1))
                {
                    AudioManager.Play(character.RocketSound);

                    var clone = character.RocketProjectile.Grab();
                    clone.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    clone.LifetimeRemaining = clone.Lifetime;
                    clone.Owner = gameObject;

                    character.RocketCooldownCurrent = character.RocketCooldown;
                }
            }
        }
    }
}
