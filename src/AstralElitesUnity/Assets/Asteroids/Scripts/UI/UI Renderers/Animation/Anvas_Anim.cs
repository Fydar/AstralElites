using System;
using System.Collections.Generic;

[Serializable]
public struct Anvas_Anim
{
	public float Duration;
	public float CurrentTime;

	public List<Anvas_Target> Clips;
}
