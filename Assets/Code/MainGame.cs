using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

public class MainGame : StateMachine
{
	//Debug Schizz
	private bool							debugFinalSeconds	= false;

	public PoolingSystem<PoolObject> 	hudAwardsPool 		= null;
	public Transform					hudAwardContainer 	= null;

	public GameObject					hudPauseButton		= null;

	public Timer						scoreTimer 			= null;
	public Timer						roundTimer 			= null;

	//KNOCKOUT!!!!
	public HUD_AnimatedText				knockoutCounterText	= null;
	public Timer						knockoutTimer		= null;

	public float						roundTime			= 10.0f;
	public Timer.TimerType				timerType			= Timer.TimerType.RoundTimer_Seconds;

	public bool							playAgain			= false;

	public int							roundCount			= 0;
	public int							victimID			= -1;

	public string						stateInfoString		= "";

	public enum mainGameStates
	{
		LevelIntro,		
	    Ready,				
	    Fight,				
	    TimeOut,			
		Knockdown,
		Knockout,
		Recover,
		Victory,
		Defeat,
		Replay,
		ResetRound,
		PointsDecision,
		ExitGame			
	};

	private void RegisterStates()
	{
		//this.stateMachine.Name = "MainGameMachine";
		this.stateMachineName =  "MainGameMachine";
		AddState((int)mainGameStates.LevelIntro,		LevelIntro_Begin,		LevelIntro_Update,		LevelIntro_End, "LevelIntro"	);			
		AddState((int)mainGameStates.Ready,				Ready_Begin,			Ready_Update,			Ready_End,		"Ready"			);			
		AddState((int)mainGameStates.Fight,				Fight_Begin,			Fight_Update,			Fight_End,		"Fight"			);			
		AddState((int)mainGameStates.TimeOut,			TimeOut_Begin,			TimeOut_Update,			TimeOut_End,	"TimeOut"		);			
		AddState((int)mainGameStates.Knockdown,			Knockdown_Begin,		Knockdown_Update,		Knockdown_End,	"KnockDown"		);			
		AddState((int)mainGameStates.Knockout,			Knockout_Begin,			Knockout_Update,		Knockout_End,	"KnockOut"		);			
		AddState((int)mainGameStates.Recover,			Recover_Begin,			Recover_Update,			Recover_End,	"Recover"		);			
		AddState((int)mainGameStates.Victory,			Victory_Begin,			Victory_Update,			Victory_End,	"Victory"		);			
		AddState((int)mainGameStates.Defeat,			Defeat_Begin,			Defeat_Update,			Defeat_End,		"Defeat"		);			
		AddState((int)mainGameStates.Replay,			Replay_Begin,			Replay_Update,			Replay_End,		"Replay"		);			
		AddState((int)mainGameStates.ResetRound,		ResetRound_Begin,		ResetRound_Update,		ResetRound_End,	"ResetRound"	);			
		AddState((int)mainGameStates.PointsDecision,	PointsDecision_Begin,	PointsDecision_Update,	PointsDecision_End, "PointsDecision"	);			
		AddState((int)mainGameStates.ExitGame,			ExitGame_Begin,			ExitGame_Update,		ExitGame_End,	"ExitGame"	);			
	
	}





	// Use this for initialization
	protected override void Start() 
	{
		//check to see if the hud scene is loaded
		//Allows you to play the levels standalone
		GameObject hudScene = GameObject.Find ("__Scene_HUDs");

		if (hudScene==null)
			Application.LoadLevelAdditive("HUDs");


		//MasterAudio.FadeOutAllOfSound("Sfx_FrontEnd_KidsAmb",1.0f);
		//MasterAudio.StopAllOfSound(GameOptions.Instance.settings.ambientsound);
		//MasterAudio.PlaySound(GameOptions.Instance.settings.ambientsound);
		//MasterAudio.FadeSoundGroupToVolume(GameOptions.Instance.settings.ambientsound,1.0f,1.0f);
		//Setup the main scene - number nodes etc.
		SetupScene();

		//State mode stuff - intros, main game, results etc.
		RegisterStates();
		SetState((int)mainGameStates.LevelIntro);

		HUDManager.Instance.HideHUD("HUD_ProgressCircle");


		if (this.roundTimer)
		{
			this.roundTimer.Reset(10, Timer.TimerType.RoundTimer_Seconds);

		}

		if (this.knockoutTimer)
		{
			this.knockoutTimer.SetOnDescreteChunkChange(this.KnockOutTimerCountsDown);
		}


		/*
		if (this.hudPauseButton)
		{
			hudPauseButton.SetActive(false);
		}
		*/
		//PromptBox.Instance.SetMainGame(this);

	}

	void OnDestroy()
	{
		//MasterAudio.FadeOutAllOfSound(GameOptions.Instance.settings.ambientsound,1.0f);
	}

	private void DisplayText(string text)
	{
		GameObject hudTextAppear = HUDManager.Instance.GetHUDObject("HUD_TextAppear");

		HUDManager.Instance.ShowHUD("HUD_TextAppear");

		if (hudTextAppear)
		{
			HUD_AnimatedText animatedText = hudTextAppear.GetComponent<HUD_AnimatedText>();

			if (animatedText)
			{
				animatedText.PlayText(text);
			}
		}
	}



	void OnGUI()
	{
		Rect vPrintRect = new Rect(10,10,200,200);

		GUI.color = Color.red;
		GUI.Label(vPrintRect, string.Format("Round {0}", this.roundCount));
		vPrintRect.y+=40;
		GUI.Label(vPrintRect, string.Format("{0}", GetActiveStateInfo()));
		vPrintRect.y+=40;
		GUI.Label(vPrintRect, this.stateInfoString);
		vPrintRect.y+=40;
	}

	virtual public void SetupScene()
	{
	}

	// Update is called once per frame
	protected override void Update() 
	{
		//if (Input.GetKeyDown(KeyCode.F))
		//	DEBUG_FinalSeconds();
		/*
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Break();
		}
		*/

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			SetState((int)mainGameStates.ExitGame);
		}


		//Update state machine
		base.Update();
		/*
		if (Input.GetKeyDown(KeyCode.Escape)) 
		{ 
			MasterAudio.StopAllOfSound("Sfx_CountupPoints");
			MasterAudio.PlaySound("Sfx_Select");
			PromptBox.Instance.ShowPromptBox(true, PromptBox.promptType.PAUSEQUIT);
			return;
		}

		if (debugFinalSeconds)
		{
			debugFinalSeconds = false;
			roundTimer.SetTime(11.0f);
		}
		*/
	}

	public void HandleTimeUp()
	{
		Debug.Log("MainGame HandleTimeUp");
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// STATE MODES
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	
	private bool useRoundTimer()
	{
		return true;
	}

	void ShowTimers(bool bShow = true)
	{		
		//show the timer
		if (useRoundTimer())
		{
			Timer timer = this.scoreTimer.GetComponent<Timer>();
			
			if (timer)
			{
				timer.ShowTimer(bShow);
			}
		}
		
		if (this.roundTimer)
		{
			this.roundTimer.gameObject.SetActive(bShow);
		}			
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// LEVEL INTRO
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void LevelIntro_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue";

		DisplayText("Intro!");
	}
	
	public void LevelIntro_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			SetState((int)mainGameStates.Ready);
		}
	}
	
	public void LevelIntro_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// READY
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Ready_Begin(int PrevMode)
	{
		this.roundCount++;
		this.stateInfoString = "";
		DisplayText("Round " + this.roundCount);
	}
	
	public void Ready_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			SetState((int)mainGameStates.Fight);
		}
	}
	
	public void Ready_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// FIGHT
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Fight_Begin(int PrevMode)
	{
		this.stateInfoString = "d:KnockDown k:Knockout SHFT:Opponent";
		DisplayText("Fight!");

		this.roundTimer.gameObject.SetActive(true);
		this.roundTimer.StartTimer(10);
	}
	
	public void Fight_Update()
	{
		if (this.roundTimer.currentTime<=0)
		{
			SetState((int)mainGameStates.TimeOut);
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				this.victimID = 1;
			else
				this.victimID = 0;

			SetState((int)mainGameStates.Knockdown);
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				this.victimID = 1;
			else
				this.victimID = 0;

			SetState((int)mainGameStates.Knockout);
		}

	}
	
	public void Fight_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// TIMEOUT
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void TimeOut_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue Round:" + this.roundCount;
		DisplayText("Timeout!");
	}
	
	public void TimeOut_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			if (this.roundCount>2)
			{
				SetState((int)mainGameStates.PointsDecision);
			}
			else
			{
				SetState((int)mainGameStates.ResetRound);
			}
		}
	}
	
	public void TimeOut_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// KNOCKDOWN
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Knockdown_Begin(int PrevMode)
	{
		this.stateInfoString = "";
		DisplayText("KnockDown!");
	}
	
	public void Knockdown_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			//Someone is in recovery.....
			SetState((int)mainGameStates.Recover);
		}
	}
	
	public void Knockdown_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// KNOCKOUT
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void	KnockOutTimerCountsDown()
	{
		if (this.knockoutCounterText)
		{
			this.knockoutCounterText.PlayText("10");
		}

	}



	public void Knockout_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue KnockOut Victim:" + this.victimID;
		DisplayText("KnockOut!");

		knockoutTimer.StartTimer(10.0f);

	}
	
	public void Knockout_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			//Someone is in recovery.....
			if (this.victimID==0)
				SetState((int)mainGameStates.Defeat);
			else if (this.victimID==1)
				SetState((int)mainGameStates.Victory);
		}
	}
	
	public void Knockout_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// RECOVER
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Recover_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue";
		DisplayText("Recover?");
	}
	
	public void Recover_Update()
	{
		bool bRecovered = UnityEngine.Random.Range(0,100)<50;	//50% chance of successful recovery
		this.stateInfoString = "Recover?:" + bRecovered.ToString();

		if (ActiveStateTime()>2.0f)
		{
			//Someone is in recovery.....
			if (bRecovered==false)
			{
				//Someone has lost!
				if (this.victimID==0)
					SetState((int)mainGameStates.Defeat);
				else if (this.victimID==1)
					SetState((int)mainGameStates.Victory);
			}
			else
			{
				SetState((int)mainGameStates.Fight);
			}
		}
	}
	
	public void Recover_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// VICTORY
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Victory_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue";
		DisplayText("You Win!");
	}
	
	public void Victory_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			SetState((int)mainGameStates.Replay);
		}

	}
	
	public void Victory_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// DEFEAT
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Defeat_Begin(int PrevMode)
	{
		this.stateInfoString = "";
		DisplayText("You Lose!");
	}
	
	public void Defeat_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			SetState((int)mainGameStates.Replay);
		}
	}
	
	public void Defeat_End(int NextMode)
	{
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REPLAY
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void Replay_Begin(int PrevMode)
	{
		this.stateInfoString = "Replay?: Y:Yes N:No";
		DisplayText("Replay Y/N!");
	}
	
	public void Replay_Update()
	{
		if (Input.GetKeyDown(KeyCode.Y))
		{
			SetState((int)mainGameStates.LevelIntro);
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			SetState((int)mainGameStates.ExitGame);
		}
	}
	
	public void Replay_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// RESET
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void ResetRound_Begin(int PrevMode)
	{
		this.stateInfoString = "";
		DisplayText("Reset Round!");
	}
	
	public void ResetRound_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			SetState((int)mainGameStates.Ready);
		}
	}
	
	public void ResetRound_End(int NextMode)
	{
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// POINTS
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void PointsDecision_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue";
		DisplayText("Points!");
	}
	
	public void PointsDecision_Update()
	{
		bool bPlayerWins = UnityEngine.Random.Range(0,100)<50;	//50% chance of successful recovery
		this.stateInfoString = "Points Decision Player Wins?:" + bPlayerWins.ToString();

		if (ActiveStateTime()>2.0f)
		{
			//Someone is in recovery.....
			if (bPlayerWins)
			{
				SetState((int)mainGameStates.Victory);
			}
			else
			{
				SetState((int)mainGameStates.Defeat);
			}
		}
	}
	
	public void PointsDecision_End(int NextMode)
	{
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// EXIT
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void ExitGame_Begin(int PrevMode)
	{
		this.stateInfoString = "SPACE:Continue";
		DisplayText("Exiting!");
	}
	
	public void ExitGame_Update()
	{
		if (ActiveStateTime()>2.0f)
		{
			Application.LoadLevel(GameSettings.sceneName_Menu);
		}
	}
	
	public void ExitGame_End(int NextMode)
	{
	}




}
