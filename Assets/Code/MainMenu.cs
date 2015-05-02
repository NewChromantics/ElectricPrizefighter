using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainMenu : StateMachine
{

	public enum MenuIndex
	{
		Unset	=	-1,
		MainMenu = 0,
		SelectPlayers,
		About,
	}


	public List<GameObject>			cameraPosObjects = new List<GameObject>();
	private static int 				targetCameraIndex	= (int)MenuIndex.MainMenu;
	private static int 				returnCameraIndex	=  (int)MenuIndex.MainMenu;

	private static Dictionary<int, Func<ButtonObj.ActionIndex,GameObject,bool>> BackButtonActions;





	public static int getTargetCameraIndex()
	{
		return targetCameraIndex;
	}
	
	public static void setTargetCameraIndex(int newIndex)
	{
		returnCameraIndex = targetCameraIndex;
		targetCameraIndex = newIndex;
	}

	public static void returnTargetCameraIndex()
	{
		targetCameraIndex = returnCameraIndex;
		returnCameraIndex = (int)MenuIndex.MainMenu;
	}
	

	public List<GameObject> 		buttonList = new List<GameObject>();

	private Vector3					vMouseDownPosition;
	private Vector3					vMousePosition;
	private float					vMouseDownTime;

	public TextMesh					txtHighScoreModeTitle 	= null;
	public TextMesh					txtHighScoreNames 		= null;
	public TextMesh					txtHighScoreScores 		= null;
	public GameObject				txtDifficulty 			= null;
	public TextMesh					txtLocalGlobal			= null;

	public List<TextMesh>			txtVersion				= new List<TextMesh>();

	[System.NonSerialized]
	public static bool				highScoreTableDirty = true;
	public static int				highScoreTableMode = 0;

	public GameObject				leaderboardButton;
	public GameObject				avatarIconObj = null;

	public GameObject				editUserObj = null;

	private bool					mainMenuIntroSoundComplete = false;


	public List<GameObject>			highScoreTitles = new List<GameObject>();
	public List<TextMesh>			highScoreScores = new List<TextMesh>();

	protected override void Start()
	{
		if (GameSettings.gameSettingsFirstPass==true)
		{
			StartCoroutine(MainMenuPlayStartSound());		

			HUD_CircleProgress.Instance.normalisedProgress = 0.0f;

		}
		else
		{
		}

		HUDManager.Instance.ShowHUD("HUD_ProgressCircle");

	}

	void MainMenuIntroSoundComplete()
	{
		//Do some stuff when the intro sound stops
	}
	
	
	IEnumerator MainMenuPlayStartSound()
	{
		this.mainMenuIntroSoundComplete = true;
		GameSettings.gameSettingsFirstPass = false;

		var result = MasterAudio.PlaySound("SumGame_MusicBoot");
		if (result != null && result.SoundPlayed) 
		{ 
			result.ActingVariation.SoundFinished += MainMenuIntroSoundComplete;
			this.mainMenuIntroSoundComplete = false;
		}
		
		while(this.mainMenuIntroSoundComplete==false)
			yield return null;
		
	}

	
	private void CheckForSwipe()
	{
		if (Input.GetMouseButtonDown(0))
		{
			this.vMouseDownPosition	= this.vMousePosition = Input.mousePosition;
			this.vMouseDownTime = 0.0f;
		}
		else if (Input.GetMouseButton(0))
		{
			this.vMouseDownTime+=Time.deltaTime;
			this.vMousePosition = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			//if (this.vMouseDownTime<0.75f)
			{
				Vector3 vDelta = this.vMousePosition - this.vMouseDownPosition;
	
				vDelta.x/=Screen.width;
				vDelta.y/=Screen.height;
				vDelta.z = 0.0f;

				if (Mathf.Abs(vDelta.x)>0.15f)
				{
					HandleSwipeLeftRight(vDelta.x);			
				}
			}
		}
	}

	void HandleSwipeLeftRight(float swipeDist)
	{
		MasterAudio.PlaySound("Sfx_Select");

		//TO DO - handle swiping left/right on menus
	
	}

	protected override void Update()
	{
		//Update the logic controller
		base.Update();

		if(Input.GetKeyDown(KeyCode.Space))
		{
			Application.LoadLevel(GameSettings.sceneName_Game);
		}

		return;

		bool cameraMoving = MoveCamera();

		if (cameraMoving==false)
		{
			if (Input.GetKeyDown(KeyCode.Escape)) 
			{ 
				MasterAudio.PlaySound("Sfx_Select");

				int menuIndex = getTargetCameraIndex();

				if (BackButtonActions!=null && BackButtonActions.ContainsKey(menuIndex))
				{
					BackButtonActions[menuIndex].Invoke(ButtonObj.ActionIndex.Unset, null); 
				}
				else
				{
					returnTargetCameraIndex();
				}

			}
		}

		CheckForSwipe();



	}

	//returns 'is moving'
	bool MoveCamera()
	{
		if (getTargetCameraIndex()<0)
			setTargetCameraIndex((getTargetCameraIndex() + this.cameraPosObjects.Count)%(this.cameraPosObjects.Count));

		Vector3 vCurrentPos = Camera.main.transform.position;
		Vector3 vTargetPos = this.cameraPosObjects[getTargetCameraIndex()].transform.position;
		
		Vector3 vCamPos = Vector3.Lerp(vCurrentPos,vTargetPos,0.15f);
		
		vCamPos.z = 0.0f;
		
		Camera.main.transform.position = vCamPos;

		Vector3 vCamOff = (vCurrentPos-vTargetPos);
		vCamOff.z = 0.0f;

		return (vCamOff.sqrMagnitude>2.0f);

	}

	public static bool StartGameMode(ButtonObj.ActionIndex actionIndex)
	{
		Application.LoadLevel(GameSettings.sceneName_Game);
		return true;
	}

	public static bool BA_PlayLevel(ButtonObj.ActionIndex actionIndex, GameObject ownerButton, ButtonObj buttonObj)
	{
		StartGameMode(actionIndex);
		return true;
	}
	
	public static bool BA_About(ButtonObj.ActionIndex actionIndex, GameObject ownerButton, ButtonObj buttonObj)
	{
		//Debug.Log("BA_About " + actionIndex.ToString());
		setTargetCameraIndex((int)MenuIndex.About);
		return true;
		
	}

	public static bool BA_SelectPlayers(ButtonObj.ActionIndex actionIndex, GameObject ownerButton, ButtonObj buttonObj)
	{
		Debug.Log("BA_SelectPlayers " + actionIndex.ToString());
		StartGameMode(actionIndex);
		//setTargetCameraIndex((int)MenuIndex.SelectPlayers);
		return true;
	}


	public static bool BA_AvatarNext(ButtonObj.ActionIndex actionIndex, GameObject ownerButton)
	{
		return true;
	}
	
	public static bool BA_AvatarPrevious(ButtonObj.ActionIndex actionIndex, GameObject ownerButton)
	{
		return true;
	}

	public static bool BA_AvatarSet(ButtonObj.ActionIndex actionIndex, GameObject ownerButton)
	{
		return true;
	}


	public static bool BA_ReturnToMainMenu(ButtonObj.ActionIndex actionIndex, GameObject ownerButton)
	{
		//Debug.Log("BA_ReturnToMainMenu " + actionIndex.ToString());

		setTargetCameraIndex((int)MenuIndex.MainMenu);

		return true;
	}

	public static bool BA_QuitGameCheck(ButtonObj.ActionIndex actionIndex, GameObject ownerButton)
	{
		PromptBox.Instance.ShowPromptBox(true, PromptBox.promptType.QUIT);

		return true;
	}

	void OnGUI2()
	{
		GUI.color = Color.yellow;
		//GUI.Label(new Rect(Screen.width/2,20,100,50), string.Format("ABTest_A{0}",GameSettings.Instance.ABTest_A.ToString()));
		char finalChar = SystemInfo.deviceUniqueIdentifier[(SystemInfo.deviceUniqueIdentifier.Length-1)];
		int iFinalChar = (int)finalChar;
		GUI.Label(new Rect(Screen.width/2,40,100,50), string.Format("{0} {1} {2}",finalChar.ToString(),iFinalChar,(iFinalChar%2)));
		GUI.Label(new Rect(Screen.width/2,60,100,50), string.Format("{0}",SystemInfo.deviceUniqueIdentifier));
	}
}