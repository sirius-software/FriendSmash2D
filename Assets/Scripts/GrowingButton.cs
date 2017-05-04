using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingButton : MonoBehaviour
{
	public event Action<GrowingButton> OnClick;

	[SerializeField]
	bool isActive = true;

	[SerializeField]
	float modifier = 1;

	[SerializeField]
	Vector3 targetScale;

	Vector3 startingScale;

	Coroutine coroutine;
		
	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			if(isActive == value)
			{
				return;
			}

			isActive = value;

			coroutine = StartCoroutine(ChangeSize(transform.localScale, startingScale));
		}
	}

	void Awake()
	{
		startingScale = transform.localScale;
	}

	void OnMouseUpAsButton()
	{
		if(isActive && OnClick != null)
		{
			OnClick(this);
		}
	}

	void OnMouseEnter()
	{
		if(!isActive)
		{
			return;
		}

		if(coroutine != null)
		{
			StopCoroutine(coroutine);
		}

		coroutine = StartCoroutine(ChangeSize(transform.localScale, targetScale));
	}

	void OnMouseExit()
	{
		if(!isActive)
		{
			return;
		}


		if(coroutine != null)
		{
			StopCoroutine(coroutine);
		}

		coroutine = StartCoroutine(ChangeSize(transform.localScale, startingScale));
	}

	IEnumerator ChangeSize(Vector3 from, Vector3 to)
	{
		float t = 0.0f;

		while(t <= 1.0f)
		{
			t += Time.deltaTime;

			transform.localScale = Vector3.Lerp(from, to, t);

			yield return null;
		}

		coroutine = null;
	}
}
