using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
	public event Action OnLivesEnded;

	[SerializeField]
	GameObject[] Lives;

	int livesCount;

	public int TakeHit()
	{
		livesCount--;

		Lives[livesCount].SetActive(false);

		return livesCount;
	}

	void Awake()
	{
		livesCount = Lives.Length;
	}
}
