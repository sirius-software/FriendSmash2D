using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Box : MonoBehaviour
{
	public event Action<Box, bool> OnDeath;

	float life = 0;
	float fullLife = 5;
	float heightModifier = 5;
	float travelDistance = 5;
	float direction = 1;
	float rotation;
	bool isAlive;

	Vector3 startingPosition;
	SpriteRenderer spriteRenderer;

	public Sprite Sprite
	{
		get;
		set;
	}

	public int Type
	{
		get;
		set;
	}

	public Game Game
	{
		get;
		set;
	}

	public void Init()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Sprite;

		float heightModifier = 100f / Sprite.rect.height;
		float widthModifier = 100f / Sprite.rect.width;
		transform.localScale = new Vector3(heightModifier, widthModifier, 1.0f);

		BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
		collider.size = new Vector3(1f, 1f, 0.2f);
	}

	public void Launch()
	{
		fullLife = Random.Range(Game.MinLife, Game.MaxLife);
		heightModifier = Random.Range(Game.MinHeight, Game.MaxHeight);
		direction = transform.localPosition.x >= 0 ? -1 : 1;
		travelDistance = Random.Range(Game.MinTravelDistance, Game.MaxTravelDistance);
		rotation = Random.Range(Game.MinRotation, Game.MaxRotation);

		startingPosition = transform.localPosition;
		isAlive = true;
	}

	public void Rotate()
	{
		transform.eulerAngles += new Vector3(0, 0, rotation * Time.deltaTime * direction);
	}

	void OnMouseDown()
	{
		Die(true);
	}

	void Update()
	{
		if(isAlive)
		{
			Move();
			Rotate();
		}

		if(life > fullLife)
		{
			Die(false);
		}
	}

	void Die(bool forcefully)
	{
		isAlive = false;

		if(OnDeath != null)
		{
			OnDeath(this, forcefully);
		}
	}

	void Move()
	{
		life += Time.deltaTime;

		float t = Mathf.Clamp01(life / fullLife);

		transform.localPosition = startingPosition + new Vector3(direction, 0, 0) * travelDistance * t + new Vector3(0, 1, 0) * Mathf.Sin(t * Mathf.PI) * heightModifier;
	}
}
