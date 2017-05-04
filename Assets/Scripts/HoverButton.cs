using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButton : MonoBehaviour
{
	[SerializeField]
	Sprite spriteNormal;

	[SerializeField]
	Sprite spriteHover;

	[SerializeField]
	Sprite spriteClicked;

	SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer.sprite = spriteNormal;
	}

	void OnMouseEnter()
	{
		if(spriteHover != null)
		{
			spriteRenderer.sprite = spriteHover;
		}
	}

	void OnMouseExit()
	{
		if(spriteHover != null)
		{
			spriteRenderer.sprite = spriteNormal;
		}
	}
}
