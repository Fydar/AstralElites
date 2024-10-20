﻿using System.Collections;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("Scene")]
    public GameObjectPool<Character> playerPool;

    private void OnEnable()
    {
        AsteroidGenerator.instance.AsteroidPool.Flush();

        CreateCharacter();
    }

    private void CreateCharacter()
    {
        AsteroidGenerator.instance.Enable();
        var player = playerPool.Grab();

        player.Revive();
        int lastValue = player.Health.Value;
        player.Health.OnAfterChanged += () =>
        {
            if (player.Health.Value <= 0.0f)
            {
                if (player.isAlive)
                {
                    ScreenEffect.instance.Pulse(1.0f);
                    Camera.main.GetComponent<PerlinShake>().PlayShake(1.0f);

                    AudioManager.Play(player.HitSound);
                    AudioManager.Play(player.DestroySound);

                    player.Kill();

                    AsteroidGenerator.instance.Disable();

                    StartCoroutine(GameOverRoutine());
                }
            }
            else
            {
                float delta = lastValue - player.Health.Value;
                lastValue = player.Health.Value;
                if (delta > 7)
                {
                    Camera.main.GetComponent<PerlinShake>().PlayShake(Mathf.InverseLerp(-30, 50, delta));
                }

                if (delta > 0)
                {
                    ScreenEffect.instance.Pulse(delta / 60.0f);
                }
            }
        };

        player.gameObject.AddComponent<CharacterAIController>();
    }

    private IEnumerator GameOverRoutine()
    {
        var asteroidsToKill = AsteroidGenerator.instance.AsteroidPool.pool;
        for (int i = asteroidsToKill.Count - 1; i >= 0; i--)
        {
            var asteroid = asteroidsToKill[i];
            if (asteroid.isActiveAndEnabled)
            {
                asteroid.Kill(false);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        yield return new WaitForSecondsRealtime(0.1f);

        playerPool.Flush();

        yield return new WaitForSecondsRealtime(0.1f);

        CreateCharacter();
    }

    private void OnDisable()
    {
        AsteroidGenerator.instance.Disable();
    }
}
