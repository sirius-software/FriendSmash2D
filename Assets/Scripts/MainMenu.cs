using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	SimpleButton playButton;

	[SerializeField]
	SimpleButton challengeButton;

	[SerializeField]
	SimpleButton bragButton;

	[SerializeField]
	SimpleButton storeButton;

	[SerializeField]
	GrowingButton loginButton;

	[SerializeField]
	StorePopUp storePopUp;

	[SerializeField]
	GameObject celebs;

	[SerializeField]
	TextMesh nameTextMesh;

	[SerializeField]
	GameObject notLoggedInInfo;

	[SerializeField]
	GameObject loggedInInfo;

	[SerializeField]
	TextMesh coinBalanceTextMesh;

	[SerializeField]
	TextMesh bombBalanceTextMesh;

	[SerializeField]
	SpriteRenderer avatarRenderer;

	[SerializeField]
	string[] defaultNames;

	List<Sprite> sprites = new List<Sprite>();
	List<string> names = new List<string>();

	bool isStarting = false;
	int loadedFriendCounter = 0;

	List<int> unusedFriends = new List<int>();

	void Awake()
	{
		if(!FB.IsInitialized)
		{
			FB.Init(InitCallback);
		}

		if(FB.IsLoggedIn)
		{
			GameSettings_OnRefresh();
		}
	}

	void OnEnable()
	{
		playButton.OnClick += PlayButton_OnClick;
		challengeButton.OnClick += ChallengeButton_OnClick;
		bragButton.OnClick += BragButton_OnClick;
		storeButton.OnClick += StoreButton_OnClick;
		loginButton.OnClick += LoginButton_OnClick;

		GameSettings.OnRefresh += GameSettings_OnRefresh;

		notLoggedInInfo.SetActive(!FB.IsLoggedIn);
		loggedInInfo.SetActive(FB.IsLoggedIn);

		GameSettings.OnCoinBalanceChanged += GameSettings_OnCoinBalanceChanged;
		GameSettings.OnBombBalanceChanged += GameSettings_OnBombBalanceChanged;
		
		coinBalanceTextMesh.text = GameSettings.CoinBalance.ToString();
		bombBalanceTextMesh.text = GameSettings.BombBalance.ToString();
	}

	void OnDisable()
	{
		playButton.OnClick -= PlayButton_OnClick;
		challengeButton.OnClick -= ChallengeButton_OnClick;
		bragButton.OnClick -= BragButton_OnClick;
		storeButton.OnClick -= StoreButton_OnClick;
		loginButton.OnClick -= LoginButton_OnClick;

		GameSettings.OnRefresh -= GameSettings_OnRefresh;
	}

	void GameSettings_OnRefresh()
	{
		if(GameSettings.Username != null)
		{
			nameTextMesh.text = GameSettings.Username;
		}

		if(GameSettings.UserTexture != null)
		{
			avatarRenderer.sprite = Sprite.Create(GameSettings.UserTexture, new Rect(0, 0, GameSettings.UserTexture.width, GameSettings.UserTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
		}
	}

	void FillWithCelebrities()
	{
		int counter = 0;

		while(counter < BoxSettings.TypesCount)
		{
			names.Add(defaultNames[counter]);
			counter++;
		}

		foreach(SpriteRenderer sr in celebs.transform.GetComponentsInChildren<SpriteRenderer>())
		{
			sprites.Add(sr.sprite);
		}
	}

	void GameStart()
	{
		if(sprites.Count < BoxSettings.TypesCount)
		{
			FillWithCelebrities();
		}

		BoxSettings.Sprites = sprites.ToArray();
		BoxSettings.Names = names.ToArray();

		SceneManager.LoadScene(1);
	}

	void LoadFriendsImages()
	{
		if(loadedFriendCounter >= BoxSettings.TypesCount || loadedFriendCounter >= GameSettings.Friends.Count)
		{
			GameStart();
			return;
		}

		int randomFriendIndex = unusedFriends[Random.Range(0, unusedFriends.Count)];

		unusedFriends.Remove(randomFriendIndex);

		Dictionary<string, object> friendData = GameSettings.Friends[randomFriendIndex] as Dictionary<string, object>;

		names.Add(friendData["first_name"] as string);

		string friendImageURL = GraphUtil.DeserializePictureURL(friendData);

		GraphUtil.LoadImgFromURL(friendImageURL, delegate (Texture pictureTexture)
		{
			if(pictureTexture != null)
			{
				//GameSettings.FriendImages.Add(GameSettings.FriendID, (Texture2D)pictureTexture);
				Sprite sprite = Sprite.Create((Texture2D)pictureTexture, new Rect(0, 0, pictureTexture.width, pictureTexture.height), new Vector2(0.5f, 0.5f));
				sprites.Add(sprite);
			}
			
			loadedFriendCounter++;
			LoadFriendsImages();
		});
	}

	void PlayButton_OnClick(SimpleButton obj)
	{
		if(storePopUp.IsOpen)
		{
			return;
		}

		if(isStarting)
		{
			return;
		}

		isStarting = true;

		Debug.Log("OnPlayClicked");

		if(GameSettings.Friends != null && GameSettings.Friends.Count > 0)
		{
			// Select a random friend and setup game state
			int randFriendNum = Random.Range(0, GameSettings.Friends.Count);
			var friend = GameSettings.Friends[randFriendNum] as Dictionary<string, object>;
			GameSettings.FriendName = friend["first_name"] as string;
			GameSettings.FriendID = friend["id"] as string;
			GameSettings.UseCeleb = false;

			// Set friend image
			if(GameSettings.FriendImages.ContainsKey(GameSettings.FriendID))
			{
				GameSettings.FriendTexture = GameSettings.FriendImages[GameSettings.FriendID];
			}
			else
			{
				// We don't have this players image yet, request it now
				string friendImgUrl = GraphUtil.DeserializePictureURL(friend);

				GraphUtil.LoadImgFromURL(friendImgUrl, delegate (Texture pictureTexture)
				{
					if(pictureTexture != null)
					{
						GameSettings.FriendImages.Add(GameSettings.FriendID, (Texture2D)pictureTexture);
						GameSettings.FriendTexture = (Texture2D)pictureTexture;
					}
				});
			}

			for(int i = 0; i < GameSettings.Friends.Count; i++)
			{
				unusedFriends.Add(i);
			}

			LoadFriendsImages();
		}
		else
		{
			//We can't access friends -- Use celebrity
			//GameStateManager.CelebFriend = UnityEngine.Random.Range(0, gResources.CelebTextures.Length - 1);
			//GameStateManager.FriendName = gResources.CelebNames[GameStateManager.CelebFriend];
			//GameStateManager.FriendTexture = gResources.CelebTextures[GameStateManager.CelebFriend];

			FillWithCelebrities();

			GameStart();
		}
	}

	void StoreButton_OnClick(SimpleButton obj)
	{
		if(storePopUp.IsOpen)
		{
			return;
		}

		storePopUp.gameObject.SetActive(true);
	}

	void BragButton_OnClick(SimpleButton obj)
	{
		if(storePopUp.IsOpen)
		{
			return;
		}

		FBShare.ShareBrag();
	}

	void ChallengeButton_OnClick(SimpleButton obj)
	{
		if(storePopUp.IsOpen)
		{
			return;
		}

		FBRequest.RequestChallenge();
	}

	void LoginButton_OnClick(GrowingButton obj)
	{
		Debug.Log("OnLoginClick");

		loginButton.IsActive = false;

		FBLogin.PromptForLogin(OnLoginComplete);
	}

	void InitCallback()
	{
		Debug.Log("InitCallback");

		// App Launch events should be logged on app launch & app resume
		// See more: https://developers.facebook.com/docs/app-events/unity#quickstart
		FBAppEvents.LaunchEvent();

		if(FB.IsLoggedIn)
		{
			Debug.Log("Already logged in");
			OnLoginComplete();
		}
	}

	void OnLoginComplete()
	{
		Debug.Log("OnLoginComplete");

		if(!FB.IsLoggedIn)
		{
			// Reenable the Login Button
			loginButton.IsActive = true;
			return;
		}

		notLoggedInInfo.SetActive(!FB.IsLoggedIn);
		loggedInInfo.SetActive(FB.IsLoggedIn);

		// Show loading animations
		//LoadingText.SetActive(true);

		// Begin querying the Graph API for Facebook data
		FBGraph.GetPlayerInfo();
		FBGraph.GetFriends();
		FBGraph.GetInvitableFriends();
		FBGraph.GetScores();
	}

	void GameSettings_OnBombBalanceChanged(int value)
	{
		bombBalanceTextMesh.text = value.ToString();
	}

	void GameSettings_OnCoinBalanceChanged(int value)
	{
		coinBalanceTextMesh.text = value.ToString();
	}
}
