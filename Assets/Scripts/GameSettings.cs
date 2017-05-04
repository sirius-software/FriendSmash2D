using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
	public static event Action OnRefresh;
	public static event Action<int> OnCoinBalanceChanged;
	public static event Action<int> OnBombBalanceChanged;
    public static readonly string ServerURL = "http://deploy.siriussoftware.bg/Unity2Canvas/FriendSmash2D/";

	static int coinBalance = 0;
	static int bombBalance = 0;

	public static string Username
	{
		get;
		set;
	}

	public static Texture2D UserTexture
	{
		get;
		set;
	}

	public static int Score
	{
		get;
		set;
	}

	public static int HighScore
	{
		get;
		set;
	}

	public static Texture2D FriendTexture
	{
		get;
		set;
	}

	public static Dictionary<string, Texture2D> FriendImages
	{
		get;
		set;
	}

	public static List<object> Scores
	{
		get;
		set;
	}

	public static List<object> Friends
	{
		get;
		set;
	}

	public static int CoinBalance
	{
		get
		{
			return coinBalance;
		}
		set
		{
			if(coinBalance == value)
			{
				return;
			}

			coinBalance = value;

			if(OnCoinBalanceChanged != null)
			{
				OnCoinBalanceChanged(coinBalance);
			}
		}
	}

	public static int BombBalance
	{
		get
		{
			return bombBalance;
		}
		set
		{
			if(bombBalance == value)
			{
				return;
			}

			bombBalance = value;

			if(OnBombBalanceChanged != null)
			{
				OnBombBalanceChanged(bombBalance);
			}
		}
	}

	public static string FriendName
	{
		get;
		set;
	}

	public static string FriendID
	{
		get;
		set;
	}

	public static bool UseCeleb
	{
		get;
		set;
	}

	static GameSettings()
	{
		FriendImages = new Dictionary<string, Texture2D>();
		Scores = new List<object>();
		Friends = new List<object>();
	}

	public static void RefreshSettings()
	{
		if(OnRefresh != null)
		{
			OnRefresh();
		}
	}
}
