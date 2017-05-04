using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
	[SerializeField]
	float minTravelDistance;

	[SerializeField]
	float maxTravelDistance;

	[SerializeField]
	float minHeight;

	[SerializeField]
	float maxHeight;

	[SerializeField]
	float minDelay;

	[SerializeField]
	float maxDelay;

	[SerializeField]
	float minLife;

	[SerializeField]
	float maxLife;

	[SerializeField]
	float minRotation;

	[SerializeField]
	float maxRotation;

	[SerializeField]
	Vector3 spawnStart;

	[SerializeField]
	Vector3 spawnEnd;

	[SerializeField]
	GameObject boxPrefab;

	[SerializeField]
	HealthBar healthBar;

	[SerializeField]
	TextMesh textMeshScore;

	[SerializeField]
	SpriteRenderer targetSprite;

	[SerializeField]
	TextMesh targetTextMesh;

	int targetType;

	int score;
	bool isActive = true;

	float timer = 0;

	public float MinTravelDistance
	{
		get
		{
			return minTravelDistance;
		}
	}

	public float MaxTravelDistance
	{
		get
		{
			return maxTravelDistance;
		}
	}

	public float MinHeight
	{
		get
		{
			return minHeight;
		}
	}

	public float MaxHeight
	{
		get
		{
			return maxHeight;
		}
	}

	public float MinLife
	{
		get
		{
			return minLife;
		}
	}

	public float MaxLife
	{
		get
		{
			return maxLife;
		}
	}

	public float MinRotation
	{
		get
		{
			return minRotation;
		}
	}

	public float MaxRotation
	{
		get
		{
			return maxRotation;
		}
	}

	public bool GameActive
	{
		get
		{
			return isActive;
		}
	}

	void Awake()
	{
		targetType = Random.Range(0, BoxSettings.TypesCount);

		Sprite sprite = BoxSettings.Sprites[targetType];

		targetSprite.sprite = sprite;
		targetTextMesh.text = "Smash " + BoxSettings.Names[targetType];

		GameSettings.Score = 0;
	}

	void Start ()
	{
		timer = 1.0f;
	}
	
	void Update ()
	{
		if(isActive)
		{
			timer -= Time.deltaTime;

			if(timer <= 0)
			{
				SpawnNewBox();
				timer = Random.Range(minDelay, maxDelay);
			}
		}
	}

	void SpawnNewBox()
	{
		GameObject go = (GameObject)Instantiate(boxPrefab);
		Box box = go.GetComponent<Box>();
		box.Game = this;
		box.transform.localPosition = Vector3.Lerp(spawnStart, spawnEnd, Random.Range(0.0f, 1.0f));
		//int type = Random.Range(0, BoxSettings.Textures.Length);
		int type = Random.Range(0, 3) == 0 ? targetType : Random.Range(0, BoxSettings.Sprites.Length);
		box.Type = type;
		box.Sprite = BoxSettings.Sprites[type];
		box.Init();
		box.Launch();
		box.OnDeath += Box_OnDeath;
	}

	void Box_OnDeath(Box box, bool isForcefull)
	{
		if(!isActive)
		{
			return;
		}

		if(box.Type == targetType)
		{
			if(isForcefull)
			{
				GameSettings.Score += 10;
				textMeshScore.text = "Score: " + GameSettings.Score;

				if(GameSettings.Score > GameSettings.HighScore)
				{
					GameSettings.HighScore = GameSettings.Score;
				}
			}
			else
			{
				int livesLeft = healthBar.TakeHit();

				if(livesLeft == 0)
				{
					SceneManager.LoadScene(0);
				}
			}
		}
		else
		{
			if(isForcefull)
			{
				isActive = false;
				StartCoroutine(GrowToCenter(box));
				box.OnDeath -= Box_OnDeath;
				return;
			}
		}

		Destroy(box.gameObject);
		box.OnDeath -= Box_OnDeath;
	}

	IEnumerator GrowToCenter(Box box)
	{
		float t = 0.0f;

		Vector3 startingPosition = box.transform.localPosition;
		Vector3 endPosition = new Vector3();

		Vector3 startingSize = box.transform.localScale;
		Vector3 endSize = new Vector3(4, 4, 4);


		while(t <= 1.0f)
		{
			t += Time.deltaTime;
			box.transform.localPosition = Vector3.Lerp(startingPosition, endPosition, t);
			box.transform.localScale = Vector3.Lerp(startingSize, endSize, t);
			box.Rotate();
			yield return null;
		}

		SceneManager.LoadScene(0);
	}
}
