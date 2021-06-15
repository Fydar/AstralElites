using UnityEngine;
using UnityEngine.UI;

public class DiscordLoginPopup : Popup
{
	[Header("Discord Avatar")]
	public Text Name;
	public RawImage Avatar;

	public void DisplayPopup(string name, Texture2D avatar)
	{
		Name.text = name;
		Avatar.texture = avatar;

		Avatar.gameObject.SetActive(avatar != null);

		StartCoroutine(PlayRoutine());
	}
}