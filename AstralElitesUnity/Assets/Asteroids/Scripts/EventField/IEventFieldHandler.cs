using System;

public interface IEventFieldHandler : IDisposable
{
	void OnBeforeChanged();
	void OnAfterChanged();
}
