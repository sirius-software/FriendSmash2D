using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePopUp : MonoBehaviour
{
	[SerializeField]
	SimpleButton closeButton;

	[SerializeField]
	List<SimpleButton> bombButtons;

	[SerializeField]
	List<SimpleButton> coinButtons;

	[SerializeField]
	int[] bombPrices;

	[SerializeField]
	int[] bombCount;

	public bool IsOpen
	{
		get
		{
			return gameObject.active;
		}
	}

	void OnEnable()
	{
		closeButton.OnClick += CloseButton_OnClick;

		foreach(SimpleButton button in bombButtons)
		{
			button.OnClick += bombButton_OnClick;
		}

		foreach(SimpleButton button in coinButtons)
		{
			button.OnClick += coinButton_OnClick;
		}
	}

	void OnDisable()
	{
		closeButton.OnClick -= CloseButton_OnClick;
	}

	void CloseButton_OnClick(SimpleButton obj)
	{
		gameObject.SetActive(false);
	}

	void bombButton_OnClick(SimpleButton button)
	{
		int index = bombButtons.IndexOf(button);

		if(GameSettings.CoinBalance >= bombPrices[index])
		{
			GameSettings.BombBalance += bombCount[index];
			GameSettings.CoinBalance -= bombPrices[index];
		}
	}

	void coinButton_OnClick(SimpleButton button)
	{
		int index = coinButtons.IndexOf(button);

		FBPayments.BuyCoins((FBPayments.CoinPackage)index);
	}
}
