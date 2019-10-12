#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DiscordController : MonoBehaviour
{
	private const string GetProfileAddress = "https://cdn.discordapp.com/avatars/{0}/{1}.jpg";

	public static DiscordController Instance;

	public InspectorLog Log;

	[Header ("Game")]
	public GlobalInt Highscore;

	[Header ("Services")]
	public string applicationId;
	public string optionalSteamId;

	public DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence ();
	public event Action<DiscordRpc.DiscordUser> OnConnect;
	public event Action OnDisconnect;
	private DiscordRpc.EventHandlers handlers;

	public void SetHighscore (int highscore)
	{
		var rank = Rank.GetRank (highscore);

		presence.details = string.Format ("Highscore: {0}", highscore.ToString ("###,###"));
		presence.largeImageKey = rank.DiscordAsset;
		presence.largeImageText = "Rank: " + rank.DisplayName;

		Log.Log ("Setting Highscore: " + highscore.ToString ("###,###") + ", rank of " + rank.DisplayName);

		DiscordRpc.UpdatePresence (presence);
	}

	public void SetState (string state)
	{
		presence.state = state;
	}

	public void EndGame (int score)
	{
		if (score > Highscore.Value)
		{
			SetHighscore (score);
		}
	}

	public void StartNewGame ()
	{
		Log.Log ("Starting New Game");

		presence.startTimestamp = (int)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;

		DiscordRpc.UpdatePresence (presence);
	}

	private IEnumerator DownloadAvatar (DiscordRpc.DiscordUser connectedUser)
	{
		var www = UnityWebRequestTexture.GetTexture (string.Format (GetProfileAddress, connectedUser.userId, connectedUser.avatar));
		yield return www.SendWebRequest ();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log (www.error);

			PopupManager.instance.GetPopup<DiscordLoginPopup> ().DisplayPopup (
				string.Format ("{0} <color=#666>#{1}</color>", connectedUser.username, connectedUser.discriminator), null);
		}
		else
		{
			var avatarIcon = ((DownloadHandlerTexture)www.downloadHandler).texture;

			PopupManager.instance.GetPopup<DiscordLoginPopup> ().DisplayPopup (
				string.Format ("{0} <color=#666>#{1}</color>", connectedUser.username, connectedUser.discriminator), avatarIcon);
		}
	}

	public void ReadyCallback (ref DiscordRpc.DiscordUser connectedUser)
	{
		Log.Log (string.Format ("Connected to {0}", connectedUser.username));

		SetHighscore (Highscore.Value);

		StartCoroutine (DownloadAvatar (connectedUser));

		if (OnConnect != null)
		{
			OnConnect (connectedUser);
		}
	}

	public void DisconnectedCallback (int errorCode, string message)
	{
		Log.Log (string.Format ("Disconnected {0}: {1}", errorCode, message));

		if (OnDisconnect != null)
		{
			OnDisconnect ();
		}
	}

	public void ErrorCallback (int errorCode, string message)
	{
		Log.Log (string.Format ("Discord: error {0}: {1}", errorCode, message));
	}

	public void JoinCallback (string secret)
	{
		Log.Log (string.Format ("Discord: join ({0})", secret));
	}

	public void SpectateCallback (string secret)
	{
		Log.Log (string.Format ("Discord: spectate ({0})", secret));
	}

	public void RequestCallback (ref DiscordRpc.DiscordUser request)
	{
		Log.Log (string.Format ("Discord: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId));
		//joinRequest = request;
	}

	public void RequestRespondYes ()
	{
		Log.Log ("Discord: responding yes to Ask to Join request");
		//DiscordRpc.Respond (joinRequest.userId, DiscordRpc.Reply.Yes);
	}

	public void RequestRespondNo ()
	{
		Log.Log ("Discord: responding no to Ask to Join request");
		//DiscordRpc.Respond (joinRequest.userId, DiscordRpc.Reply.No);
	}

	private void Awake ()
	{
		Instance = this;
	}

	private void Update ()
	{
		DiscordRpc.RunCallbacks ();
	}

	private void OnEnable ()
	{
		Log.Log ("Initializing Discord");

		DiscordRpc.Events.readyCallback = ReadyCallback;
		DiscordRpc.Events.disconnectedCallback += DisconnectedCallback;
		DiscordRpc.Events.errorCallback += ErrorCallback;
		DiscordRpc.Events.joinCallback += JoinCallback;
		DiscordRpc.Events.spectateCallback += SpectateCallback;
		DiscordRpc.Events.requestCallback += RequestCallback;

		DiscordRpc.Initialize (applicationId, true, optionalSteamId);
	}

	private void OnDisable ()
	{
		Log.Log ("Shutdown");
		DiscordRpc.Shutdown ();
	}

	private void OnDestroy ()
	{

	}
}
#endif