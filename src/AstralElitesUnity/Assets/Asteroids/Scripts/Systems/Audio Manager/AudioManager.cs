using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private static AudioManager instance;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnRuntimeMethodLoad()
	{
		var audioManager = new GameObject("Audio Manager");
		DontDestroyOnLoad(audioManager);

		instance = audioManager.AddComponent<AudioManager>();
	}

	public MusicGroup Music;

	public VolumeControl TabFade = new VolumeControl(1.0f);
	public VolumeControl MasterVolume = new VolumeControl(1.0f);
	public VolumeControl SfxVolume = new VolumeControl(1.0f);
	public VolumeControl MusicVolume = new VolumeControl(1.0f);

	public AudioSourcePool Pool = new AudioSourcePool(25);
	public List<AudioSourceAnimator> Animators = new List<AudioSourceAnimator>();

	private IInterpolator interpolator;

	private void Awake()
	{
		Pool.Initialise(gameObject);

		interpolator = new LinearInterpolator(8.0f) { Value = 1.0f };
	}

	private void Start()
	{
		interpolator.TargetValue = 1.0f;
		interpolator.Value = 1.0f;
	}

	private void Update()
	{
		SfxVolume.Volume = SfxVolume.Volume;
		MusicVolume.Volume = MusicVolume.Volume;
		interpolator.Update(Time.unscaledDeltaTime);
		TabFade.Volume = interpolator.Value;

		for (int i = Animators.Count - 1; i >= 0; i--)
		{
			var animator = Animators[i];

			animator.Update(Time.deltaTime);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		interpolator.TargetValue = hasFocus ? 1.0f : 0.0f;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
	}

	public void PlayClip(SfxGroup group)
	{
		var source = Pool.Grab();

		source.clip = group.GetClip();
		source.volume = UnityEngine.Random.Range(group.VolumeRange.x, group.VolumeRange.y);
		source.pitch = UnityEngine.Random.Range(group.PitchRange.x, group.PitchRange.y);
		source.loop = false;

		var animator = new AudioSourceAnimator(source, TabFade, MasterVolume, SfxVolume);
		Animators.Add(animator);

		source.Play();
		StartCoroutine(ReturnToPool(animator));
	}

	public void PlayClip(LoopGroup group, EffectFader fader)
	{
		var source = Pool.Grab();

		source.clip = group.LoopedAudio;
		source.pitch = group.PitchRange.x;
		source.volume = group.VolumeRange.x;
		source.loop = true;

		var animator = new AudioSourceAnimator(source, TabFade, MasterVolume, SfxVolume);
		Animators.Add(animator);

		source.Play();
		StartCoroutine(ManageLoop(animator, group, fader));
	}

	public static void PlayMusic(string name)
	{
		instance.StartCoroutine(instance.LoadAndPlayMusic(name));
	}

	public void PlayMusic(MusicGroup group)
	{
		var source = Pool.Grab();

		source.clip = group.Music[0];
		source.volume = group.Volume;
		source.priority = 1024;
		source.loop = true;

		var animator = new AudioSourceAnimator(source, TabFade, MasterVolume, MusicVolume);
		Animators.Add(animator);

		source.Play();
	}

	private IEnumerator LoadAndPlayMusic(string resourcePath)
	{
		var request = Resources.LoadAsync<MusicGroup>(resourcePath);

		while (!request.isDone)
		{
			yield return null;
		}

		if (request.asset != null)
		{
			PlayMusic((MusicGroup)request.asset);
		}
	}

	private IEnumerator ReturnToPool(AudioSourceAnimator animator)
	{
		yield return new WaitForSeconds(animator.Source.clip.length / animator.Source.pitch);
		animator.Source.Stop();
		Pool.Return(animator.Source);
		Animators.Remove(animator);
	}

	private IEnumerator ManageLoop(AudioSourceAnimator animator, LoopGroup group, EffectFader fader)
	{
		var FadeControl = new VolumeControl(0.0f);
		animator.AddControl(FadeControl);
		while (true)
		{
			fader.Update(Time.deltaTime);
			FadeControl.Volume = fader.Value;
			yield return null;
		}
	}

	public static void Play(SfxGroup group)
	{
		instance.PlayClip(group);
	}

	public static void Play(LoopGroup group, EffectFader fader)
	{
		instance.PlayClip(group, fader);
	}
}