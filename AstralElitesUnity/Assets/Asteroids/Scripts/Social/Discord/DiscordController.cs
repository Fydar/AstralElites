using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DiscordController : MonoBehaviour
{
	public static DiscordController Instance;

#if UNITY_STANDALONE || UNITY_EDITOR
	private const string GetProfileAddress = "https://cdn.discordapp.com/avatars/{0}/{1}.jpg";


	public InspectorLog Log;

	[Header("Game")]
	public GlobalInt Highscore;
	public int LastHighscore;

	[Header("Services")]
	public string applicationId;
	public string optionalSteamId;

	public DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
	public event Action<DiscordRpc.DiscordUser> OnConnect;
	public event Action OnDisconnect;
	private DiscordRpc.EventHandlers handlers;
#endif

	public void SetHighscore(int highscore)
	{
#if UNITY_STANDALONE || UNITY_EDITOR
		var rank = Rank.GetRank(highscore);
		LastHighscore = highscore;

		presence.details = string.Format("Highscore: {0}", highscore.ToString("###,###"));
		presence.largeImageKey = rank.DiscordAsset;
		presence.largeImageText = "Rank: " + rank.DisplayName;

		Log.Log("Setting Highscore: " + highscore.ToString("###,###") + ", rank of " + rank.DisplayName);

		DiscordRpc.UpdatePresence(presence);
#endif
	}

	public void EndGame(int score)
	{
#if UNITY_STANDALONE || UNITY_EDITOR
		if (score > LastHighscore)
		{
			SetHighscore(score);
		}
#endif
	}

	public void StartNewGame()
	{
#if UNITY_STANDALONE || UNITY_EDITOR
		Log.Log("Starting New Game");

		presence.startTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

		DiscordRpc.UpdatePresence(presence);
#endif
	}

	private void Awake()
	{
		Instance = this;
	}

#if UNITY_STANDALONE || UNITY_EDITOR
	private IEnumerator DownloadAvatar(DiscordRpc.DiscordUser connectedUser)
	{
		var www = UnityWebRequestTexture.GetTexture(string.Format(GetProfileAddress, connectedUser.userId, connectedUser.avatar));
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);

			PopupManager.instance.GetPopup<DiscordLoginPopup>().DisplayPopup(
				string.Format("{0} <color=#666>#{1}</color>", connectedUser.username, connectedUser.discriminator), null);
		}
		else
		{
			var avatarIcon = ((DownloadHandlerTexture)www.downloadHandler).texture;

			PopupManager.instance.GetPopup<DiscordLoginPopup>().DisplayPopup(
				string.Format("{0} <color=#666>#{1}</color>", connectedUser.username, connectedUser.discriminator), avatarIcon);
		}
	}

	private void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
	{
		Log.Log(string.Format("Connected to {0}", connectedUser.username));

		SetHighscore(Highscore.Value);

		StartCoroutine(DownloadAvatar(connectedUser));

		if (OnConnect != null)
		{
			OnConnect(connectedUser);
		}
	}

	private void DisconnectedCallback(int errorCode, string message)
	{
		Log.Log(string.Format("Disconnected {0}: {1}", errorCode, message));

		if (OnDisconnect != null)
		{
			OnDisconnect();
		}
	}

	private void ErrorCallback(int errorCode, string message)
	{
		Log.Log(string.Format("Discord: error {0}: {1}", errorCode, message));
	}

	private void JoinCallback(string secret)
	{
		Log.Log(string.Format("Discord: join ({0})", secret));
	}

	private void SpectateCallback(string secret)
	{
		Log.Log(string.Format("Discord: spectate ({0})", secret));
	}

	private void RequestCallback(ref DiscordRpc.DiscordUser request)
	{
		Log.Log(string.Format("Discord: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId));
		//joinRequest = request;
	}

	private void RequestRespondYes()
	{
		Log.Log("Discord: responding yes to Ask to Join request");
		//DiscordRpc.Respond (joinRequest.userId, DiscordRpc.Reply.Yes);
	}

	private void RequestRespondNo()
	{
		Log.Log("Discord: responding no to Ask to Join request");
		//DiscordRpc.Respond (joinRequest.userId, DiscordRpc.Reply.No);
	}

	private void Update()
	{
		DiscordRpc.RunCallbacks();
	}

	private void OnEnable()
	{
		Log.Log("Initializing Discord");

		DiscordRpc.Events.readyCallback = ReadyCallback;
		DiscordRpc.Events.disconnectedCallback += DisconnectedCallback;
		DiscordRpc.Events.errorCallback += ErrorCallback;
		DiscordRpc.Events.joinCallback += JoinCallback;
		DiscordRpc.Events.spectateCallback += SpectateCallback;
		DiscordRpc.Events.requestCallback += RequestCallback;

		DiscordRpc.Initialize(applicationId, true, optionalSteamId);
	}

	private void OnDisable()
	{
		Log.Log("Shutdown");
		DiscordRpc.Shutdown();
	}

	private void OnDestroy()
	{

	}
#endif
}