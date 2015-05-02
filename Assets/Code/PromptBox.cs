using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PromptBox : MonoBehaviour 
{
	static PromptBox 	instance;
		
	public TextMesh		promptTextHigh 	= null;
	public TextMesh		promptTextLow 	= null;
	public TextMesh		promptFooter 	= null;
	public TextMesh		titleText 		= null;
	public TextMesh		scoreText 		= null;
	public TextMesh		yesText			= null;
	public TextMesh		noText			= null;

	public GameObject	bttnYes			= null;
	public GameObject	bttnNo			= null;


	public GameObject	fadeBackgroundObj 		= null;
	public GameObject	highScoreParticleObj 	= null;
	public GameObject	fbPostButton			= null;	
	
	ParticleSystem particlesystem = null;
    
	private float		ignoreButtonDelay = 0.5f;
	
	public bool			showFadeBackground = true;
	
	private MainGame	mainGame = null;
	
	public enum promptType
	{
		INACTIVE,
		QUIT,
		PLAYAGAIN,
		PAUSEQUIT,
	}
	
	public promptType	type = promptType.QUIT;
	public promptType	activePrompt = promptType.INACTIVE;


	public ButtonObj.ActionIndex 	gameModeSelected = ButtonObj.ActionIndex.Unset;

	public void SetMainGame(MainGame mGame)
	{
		mainGame = mGame;
	}
	/*
    public static PromptBox Instance
    {
        get
        {
           	if (instance==null)
			{
				instance = (new GameObject("PromptBoxContainer" + Application.loadedLevelName)).AddComponent<PromptBox>();
				//instance = FindSceneObjectsOfType<PromptBox>();
			}
			return instance;
        
		
		}
    }
    */
	
	public static PromptBox Instance
	{
		get
		{
			if (instance == null)
			{
				var objArray = Resources.FindObjectsOfTypeAll (typeof(PromptBox)) as PromptBox[];
				
				foreach (var obj in objArray)
				{
					if (obj.transform.parent != null)
					{
						if (instance!=null)
						{
							//Debug.LogError("Singleton " + objArray.GetType().Name + " has " + objArray.Length + " items. more than 1 in scene!!!");
						}

						instance = obj;

					}
				}
				
				
			}
 
			return instance;
		}
	}
	
	
	
	PromptBox()
	{
		instance = this;
		instance.activePrompt = promptType.INACTIVE;
		instance.type = promptType.INACTIVE;

	}
	
	public void Initialise()
	{
	//	instance = this;
	}
	
	
	public void Quit(bool bQuit)
	{
		if (bQuit)
		{
			Application.Quit();
		}
		else
		{
			Instance.ShowPromptBox(false, promptType.QUIT);
		}
		
	}
	
	//Button Action handling
	public static bool BA_YesNo(ButtonObj.ActionIndex actionIndex, GameObject ownerButton)
	{
		if (Instance.ignoreButtonDelay>0)
			return false;
		
		switch (Instance.type)
		{
			case promptType.QUIT:
				Instance.Quit(actionIndex == ButtonObj.ActionIndex.Prompt_Yes);
				//Debug.Log("BA_Quit " + actionIndex.ToString());
				break;

			case promptType.PAUSEQUIT:
				if (actionIndex == ButtonObj.ActionIndex.Prompt_No)
				{
					Instance.ShowPromptBox(false, promptType.PAUSEQUIT);
					GameSettings.Instance.mainGamePaused = false;
				}
				else
				{
					Instance.ShowPromptBox(false,Instance.type);
					Application.LoadLevel(GameSettings.sceneName_Menu);	
				}
				break;

			case promptType.PLAYAGAIN:
				if (actionIndex == ButtonObj.ActionIndex.Prompt_No)
				{
					Instance.ShowPromptBox(false, Instance.type);
					Application.LoadLevel(GameSettings.sceneName_Menu);
				}
				else
				{
					Instance.ShowPromptBox(false, Instance.type);
					if (Instance.mainGame)
					{
						Instance.mainGame.playAgain = true;
					}
				}
			
				//Debug.Log("BA_PlayAgain " + actionIndex.ToString());
				break;

		}
		
		

		return true;
	}
	
	void Awake()
	{
		ActivateHighScoreParticleSystem(false);
		DontDestroyOnLoad(gameObject);
	}
	
	
	void Start()
	{
	}
	
	void ActivateHighScoreParticleSystem(bool bActivate)
	{
		if (highScoreParticleObj)
		{
			if (this.particlesystem == null)
			{
				ParticleSystem[] ps = highScoreParticleObj.GetComponentsInChildren<ParticleSystem>(true);
				
				if (ps!=null)
				{
					this.particlesystem = ps[0];
				}
			}
			
			if (this.particlesystem)
			{
				particlesystem.Clear();
				particlesystem.enableEmission = bActivate;
				particlesystem.playOnAwake = bActivate;
				
				if (bActivate)
					particlesystem.Play();
				else
					particlesystem.Stop();
			}

			//highScoreParticleObj.SetActiveRecursively(bActivate);
			
			
		}
	}
	
	void Update()
	{
		this.ignoreButtonDelay -=Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			MasterAudio.PlaySound("Sfx_Select");
			switch (instance.type)
			{
				case promptType.PAUSEQUIT:
				case promptType.QUIT:
					BA_YesNo(ButtonObj.ActionIndex.Prompt_No, null);
					break;

				default:
					BA_YesNo(ButtonObj.ActionIndex.Prompt_No, null);
					break;
			};


		}


	}

	
	
	public void SetPromptTextHigh(string promptString)
	{
		if (this.promptTextHigh!=null)
			this.promptTextHigh.text = promptString;
	
	}	
	
	public void SetPromptTextLow(string promptString)
	{
		if (this.promptTextLow!=null)
			this.promptTextLow.text = promptString;
		
	}

	public void SetPromptTextFooter(string promptString)
	{
		if (this.promptFooter!=null)
			this.promptFooter.text = promptString;
		
	}




	public void SetTitleText(string titleString)
	{
		if (this.titleText!=null)
			this.titleText.text = titleString;

		
	}
	
	public void SetScoreText(string scoreString)
	{
		if (this.scoreText!=null)
			this.scoreText.text = scoreString;
	}

	public void SetYesNoText(string yesString, string noString)
	{
		if (this.yesText!=null)
		{
			this.yesText.text = yesString;

			if (this.bttnYes!= null)
			{
				this.bttnYes.SetActive(yesString!="");
			}

		}
		
		if (this.noText!=null)
		{
			this.noText.text = noString;

			if (this.bttnNo!= null)
			{
				this.bttnNo.SetActive(noString!="");
			}
		}
		


	}

	public void ShowPromptBox(bool bShow, promptType type = promptType.QUIT, string scoreString = "" )
	{
		//Debug.Log(string.Format("ShowPromptBox {0} {1}", bShow.ToString(), type.ToString()));
		if (bShow)
		{
			//Debug.Log("Active Prompt = " + activePrompt.ToString());
			if (activePrompt != promptType.INACTIVE)
				return;


			foreach (HUDPositioner hudPos in gameObject.GetComponentsInChildren<HUDPositioner>(true))
			{
				hudPos.ownerCamera = null;
				hudPos.forceUpdate = true;
				hudPos.initialUpdateOnly = false;
				//hudPos.UpdateHUD(true);
			}
		}

		this.type = type;

		bool fbButtonVisibility = false;

		if (bShow)
		{
			switch(type)
			{
				case promptType.QUIT:	//Quit from main menu
					SetPromptTextHigh("Are you sure\nyou want to quit?");
					SetPromptTextLow("");
					SetPromptTextFooter("");
					SetTitleText("Quit?");
					SetScoreText("");

					SetYesNoText("Quit","Resume");

				break;

				case promptType.PAUSEQUIT:
					SetPromptTextHigh("Game Paused");
					SetPromptTextLow("");
					SetPromptTextFooter("");
					SetTitleText("Pause");
					SetScoreText("");
					SetYesNoText("Quit","Continue");
					GameSettings.Instance.mainGamePaused = true;	
				break;


				case promptType.PLAYAGAIN:
					//SetPromptTextHigh(GameSettings.Instance.DifficultyString() + " Mode");
					SetPromptTextLow("Play Again?");
					SetPromptTextFooter("");
					//SetTitleText(GameOptions.Instance.settings.highScoreTitle);
					SetScoreText(scoreString);
					fbButtonVisibility = true;
					ActivateHighScoreParticleSystem(false);
					SetYesNoText("Yes","Main Menu");
					break;
				
			}
		}		
		else
		{
			ActivateHighScoreParticleSystem(false);

			switch(type)
			{
				case promptType.QUIT:	//Quit from main menu
					break;
			};

		}
		//turn on/off
		//this.gameObject.SetActiveRecursively(bShow);
		Instance.gameObject.SetActive(bShow);

		if (bShow)
			activePrompt = type;
		else
		{
			activePrompt = promptType.INACTIVE;
		}

		//the background fader may be on or off when the rest of the prompt box is on
		if (this.fadeBackgroundObj && (bShow==true))
		{
			this.fadeBackgroundObj.SetActive(this.showFadeBackground);
		}
		ignoreButtonDelay = 0.5f;
		
		if (bShow==false)
		{
			GameSettings.Instance.mainGamePaused = false;	
		}

	}
	
}


