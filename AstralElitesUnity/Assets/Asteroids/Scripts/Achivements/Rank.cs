using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Rank")]
public class Rank : ScriptableObject
{
	public string DisplayName = "Asset";
	public string DiscordAsset = "Asset";
	public int RequiredScore = 1000;

	public RankRenderInformation Render;

	public Sprite Icon;

	private static Rank[] Ranks;

	public static Rank GetRank (int score)
	{	
		if (Ranks == null)
		{
			var ranks = new List<Rank> (Resources.LoadAll<Rank> ("Achievements"));
			ranks.Sort (SortRanks);

			Ranks = ranks.ToArray ();
		}

		var lastRank = Ranks[0];

		for (int i = 1; i < Ranks.Length; i++)
		{
			var currentRank = Ranks[i];

			if (currentRank.RequiredScore > score)
			{
				return lastRank;
			}
			lastRank = currentRank;
		}
		return lastRank;
	}

	private static int SortRanks (Rank a, Rank b)
	{
		return a.RequiredScore.CompareTo (b.RequiredScore);
	}
}
