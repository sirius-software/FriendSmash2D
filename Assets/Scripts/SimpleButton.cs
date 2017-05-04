using System;
using UnityEngine;

public class SimpleButton : MonoBehaviour
{
	public event Action<SimpleButton> OnClick;

	void OnMouseUpAsButton()
	{
		if(OnClick != null)
		{
			OnClick(this);
		}
	}
}
