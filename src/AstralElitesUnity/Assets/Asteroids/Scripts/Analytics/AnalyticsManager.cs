﻿using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
	public static AnalyticsManager instance;

	public Player target;

	public AnalyticsHook<Collision2D> Collisions;

	public AnalyticsHook_Int healthHook;
	public AnalyticsHook_Int scoreHook;
	public AnalyticsHook_Int asteroidsDestroyedHook;
	public AnalyticsHook_Float distanceTravelledHook;

	private GameManager gameManager;

	private void Awake()
	{
		gameManager = GetComponent<GameManager>();

		instance = this;
	}

	private void Start()
	{
		healthHook = new AnalyticsHook_Int(gameManager.player.Health);
		scoreHook = new AnalyticsHook_Int(gameManager.Score);
		asteroidsDestroyedHook = new AnalyticsHook_Int(gameManager.AsteroidsDestroyed);
		distanceTravelledHook = new AnalyticsHook_Float(gameManager.DistanceTravelled);

		Collisions = new AnalyticsHook<Collision2D>();
		target.OnCollide += Collisions.Callback;
	}

	public void Clear()
	{
		scoreHook.Clear();
		healthHook.Clear();
		asteroidsDestroyedHook.Clear();
		distanceTravelledHook.Clear();
	}

	public void StartCapture()
	{
		InvokeRepeating("Capture", 0.0f, 2.00f);
	}

	public void EndCapture()
	{
		Recapture();
		CancelInvoke("Capture");
	}

	private void Capture()
	{
		scoreHook.Capture();
		healthHook.Capture();
		asteroidsDestroyedHook.Capture();
		distanceTravelledHook.Capture();
	}

	public void Recapture()
	{
		scoreHook.Recapture();
		healthHook.Recapture();
		asteroidsDestroyedHook.Recapture();
		distanceTravelledHook.Recapture();
	}
}
