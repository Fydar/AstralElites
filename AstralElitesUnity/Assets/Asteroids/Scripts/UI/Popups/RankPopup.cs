using UnityEngine;
using UnityEngine.UI;

public class RankPopup : Popup
{
	[Header ("Rank")]
	public Text Name;
	public Image RankIcon;
	public RankRender RankDisplay;

	public void DisplayPopup (Rank rank)
	{
		Name.text = rank.DisplayName;
		RankIcon.sprite = rank.Icon;
		RankDisplay.RenderRank (rank);

		StartCoroutine (PlayRoutine ());
	}
}