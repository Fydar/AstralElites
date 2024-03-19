using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterPlayerController : MonoBehaviour
{
    private Character character;
    private Camera mainCamera;

    private void Awake()
    {
        character = GetComponent<Character>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!character.isAlive)
        {
            return;
        }

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

        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        var scenePoint = ray.origin + (ray.direction * 10);

        float angleRadians = Mathf.Atan2(scenePoint.y - transform.position.y,
            scenePoint.x - transform.position.x);

        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        character.inputRotation = angleDegrees;

        if (Input.GetMouseButton(0))
        {
            character.inputThrust = transform.right;
        }
        else
        {
            character.inputThrust = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        }
    }
}
