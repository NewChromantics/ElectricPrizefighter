using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Handles the flow into the main game
//See also level loader - which loads the extra levels in the background

public class BootSequence : MonoBehaviour 
{
	
	public string 		levelName 			= "";
	public bool			loadOnKeyPress 		= true;
	public float 		timeout				= 5.0f;
	
	//private float 		timeoutCounter 		= 0;
	//private bool		actioned 			= false;

	public static bool	extraScenesLoaded	= false;


	public string		levelLoadSound 		= "";
	public string		startSound 			= "";
	public bool			introSoundComplete 	= false;

	public TextMesh		initialisingText	= null;


	//private float		socialMediaDelay 	= 2.0f;
	
	private string		sequenceDescription = "...";

	void Update()
	{
		if (initialisingText!=null)
		{
			initialisingText.text = "Initialising" + sequenceDescription;
		}
	}


	void Start () 
	{
		//clear all the world high scores from player prefs
		Application.targetFrameRate = 60;
		StartCoroutine(BootSequenceUpdate());
	}

	void OnLevelWasLoaded(int level)
	{
	}


	IEnumerator PlayStartSound()
	{
		this.introSoundComplete = true;

		var result = MasterAudio.PlaySound(startSound);
		if (result != null && result.SoundPlayed) 
		{ 
			//Debug.Log("Play start Sound " + startSound + "Added LoadLevel.IntroSoundComplete" );
			result.ActingVariation.SoundFinished += IntroSoundComplete;
			this.introSoundComplete = false;
		}

		while(this.introSoundComplete==false)
			yield return null;

	}

	void IntroSoundComplete()
	{
		this.introSoundComplete = true;
	}
	
	
	IEnumerator BootSequenceUpdate()
	{
		sequenceDescription = "";

		while (extraScenesLoaded == false )
			yield return null;

		//yield return StartCoroutine(PlayStartSound());

		sequenceDescription = "";

		LoadTheLevel();
		
	}

	//#if UNITY_EDITOR
	void OnGUI()
	{
		GUI.Label(new Rect(20,20,100,50), sequenceDescription);
	}
	//#endif

	private void LoadTheLevel()
	{
		if (this.levelLoadSound!="")
		{
			MasterAudio.PlaySound(this.levelLoadSound);
		}
		
		Application.LoadLevel(levelName);
	}
}
