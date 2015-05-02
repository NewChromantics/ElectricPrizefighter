using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour 
{
	public enum TimerType
	{
		ScoreTimer,
		RoundTimer_Seconds,
		RoundTimer_Tenths,
	};


	public float			startingTime 			= 60.0f;
	public float			currentTime 			= 60.0f;
	private float 			timeChunked				= 60.0f;
	public float			descreteChunk			= 0.5f;		//Update the timer ever X seconds 1.0f 0.1f etc
	public int				decimalPlaces			= 1;
	public Material 		timerMaterial			= null;
	private float			pauseTimerDelay			= 0.0f;

	private float			minMaterialVal			= 0.0039525691699605f; //1/253
	private float			maxMaterialVal			= 0.9960474308300395f; //1.0f-(1/253)


	//float debugTimerTICK						= 0.0f;
	//float debugTimerTOCK						= 0.0f;

	public bool			textAsMinutesAndSeconds = true;

	//private bool		pausedGame				= false;
	
	private List<TextMesh>	hudTextMesh 		= new List<TextMesh>();

	
	public List<Color>	colors					= new List<Color>();
	
	public bool			timerActive 			= false;
	public bool			updateTimeText			= false;
	public MainGame		mainGame 				= null;
	
	public bool			countDescrete 			= false;
	public bool			messageMainGameOnTimeUp = false;

	public delegate void DescreteChunkChange();
	private DescreteChunkChange	onDescreteChunkChange	= null;

	public void SetOnDescreteChunkChange(DescreteChunkChange _dcc)
	{
		this.onDescreteChunkChange = _dcc;
	}


	private bool		audioTickTock 			= true;

	public GameObject			pauseTimeEffect			= null;
	public Animation 			pauseTimeEffectAnim		= null;
	public SpriteSheetHelper	spriteSheetHelper		= null;
	public GameObject			splatParticlePrefab		= null;
	//public static PoolingSystem<PoolObject> 	splatEffectPool 		= null;

	public bool testActivate = false;

	private bool timerPausedUntilPress = false;

	// Use this for initialization
	void Start () 
	{
		foreach (TextMesh tm in GetComponentsInChildren<TextMesh>())
		{
			this.hudTextMesh.Add(tm);
		}

		if (this.pauseTimeEffect)
		{
			this.pauseTimeEffectAnim 	= this.pauseTimeEffect.GetComponentInChildren<Animation>();
			this.spriteSheetHelper 		= this.pauseTimeEffect.GetComponentInChildren<SpriteSheetHelper>();
		}

		Reset();

		SetupAnimationSpeeds();

		UpdateTimerMaterial(1.0f);
	}

	
	public void SetTimerPauseDuration(float duration)
	{
		this.pauseTimerDelay = duration;

		if (this.pauseTimeEffectAnim)
		{
			pauseTimeEffectAnim.Play("anm_TimerSplatAppear", PlayMode.StopAll);
			this.spriteSheetHelper.SetRandomSprite();
			splatParticlePrefab.GetComponent<ParticleSystem>().Play();
		}
	}

	public void IncrementTimer(float timerIncrement)
	{
		this.currentTime+=timerIncrement;
		//this.timeChunked+=timerIncrement;
	}
	
	void SetupAnimationSpeeds()
	{
		if (GetComponent<Animation>()!=null)
		{
			foreach (AnimationState state in GetComponent<Animation>()) 
			{
	            state.speed = 1.5f;
			}
		}
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		/*
		if (testActivate)
		{
			SetTimerPauseDuration(3.0f);
			testActivate = false;
		}
		*/

		if (this.timerPausedUntilPress)
		{
			if (Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1))
				this.timerPausedUntilPress = false;

			return;
		}

		/*
		if ((this.pauseTimeEffect!=null)&&false)
		{
			this.pauseTimeEffect.SetActive(this.pauseTimerDelay>0);
		}
		*/

		if (GameSettings.Instance.mainGamePaused)
			return;

		if (!this.timerActive)
			return;
	
		if (this.pauseTimerDelay>0)
		{
			this.pauseTimerDelay-=Time.deltaTime;

			if (this.pauseTimerDelay<=0)
			{
				MasterAudio.PlaySound("Sfx_FreezeTimeClockStartsAgain");
				if (this.pauseTimeEffectAnim)
				{

					pauseTimeEffectAnim.Play("anm_TimerSplatDisappear", PlayMode.StopAll);
					splatParticlePrefab.GetComponent<ParticleSystem>().Stop();
				}
			}

			return;
		}

		if (this.startingTime<=0.0f)
			this.currentTime+=Time.deltaTime;
		else
			this.currentTime-=Time.deltaTime;
		
		float fNormalisedTime = GetNormalisedTime();
		
		if (this.countDescrete && this.startingTime>0.0f)
		{
			if ((this.currentTime<=this.timeChunked))//&&(this.currentTime<10.0f))
			{
				if (this.onDescreteChunkChange!=null)
					this.onDescreteChunkChange();

				if (this.currentTime<10.5f)
				{	

					if (this.audioTickTock)
					{
						MasterAudio.PlaySound("Sfx_Tick");
						//this.debugTimerTICK = 0.25f;
					}					
					else
					{
						if ((this.currentTime<=(5.0f+Time.deltaTime)))
						{
							MasterAudio.PlaySound("Sfx_Tock");
							//this.debugTimerTOCK = 0.25f;
						}

					}					

					this.audioTickTock = !this.audioTickTock;

				}
					
				
				UpdateTimerMaterial(fNormalisedTime);
				this.timeChunked-=this.descreteChunk;
				
				if (this.timeChunked<=0)
				{
					HandleTimeUp();
				}
			}		
		}
		else if(this.startingTime>0.0f)
		{
			UpdateTimerMaterial(fNormalisedTime);
			
			if (this.currentTime<=0.0f)
			{
				HandleTimeUp();
			}
		}
		
		if (this.updateTimeText)
		{
			UpdateTimeText();
		}
		
	}
	#if UNITY_EDITOR2
	void OnGUI()
	{
		GUI.contentColor = Color.red;

		string ticktock = "";

		if (this.debugTimerTICK>0)
		{
			this.debugTimerTICK-=Time.deltaTime;
			ticktock = ticktock + "TICK";
		}

		if (this.debugTimerTOCK>0)
		{
			this.debugTimerTOCK-=Time.deltaTime;
			ticktock = ticktock + "..TOCK";
		}

		GUI.Label(new Rect(0,0,Screen.width,Screen.height), ticktock);

	}
	#endif


	private void UpdateTimeText()
	{
		float timeToDisplay = this.currentTime;



		if (this.countDescrete)
		{
			timeToDisplay = this.timeChunked;
		}
		
		timeToDisplay = Mathf.Max(timeToDisplay,0);
		
		
		string timeFormat = string.Format("f{0}", this.decimalPlaces);
		string timeString = timeToDisplay.ToString(timeFormat);
		
		if (this.textAsMinutesAndSeconds)
		{
			int minutes = (int)(timeToDisplay/60.0f);
			int seconds = (int)(timeToDisplay - ((float)minutes*60.0f));
			float tenths = (int)(((timeToDisplay - ((float)minutes*60.0f) - (float)seconds))*10.0f);
			
			if (this.decimalPlaces==0)	
				timeString = string.Format("{0}:{1}",minutes.ToString(),((int)seconds).ToString("D2"));
			else
				timeString = string.Format("{0}:{1}.{2}",minutes.ToString(),((int)seconds).ToString("D2"),tenths);
		}
		else
		{
			//just seconds then!!!

			if (this.decimalPlaces==0)	
				timeString = string.Format("{0}",timeToDisplay);
			else
				timeString = string.Format("{0:0.0}",timeToDisplay);
		}
		

		//if (decimalPlaces==0)
		//	timeString = (((int)(timeToDisplay))).ToString();
		
		
		UpdateText(timeString);
	}
	
	private void UpdateText(string newText)
	{
		foreach (TextMesh tm in this.hudTextMesh)
		{
			tm.text = newText;
		}
	}	

	public void Reset(float roundTime, TimerType timerType)
	{

		switch (timerType)
		{
			case TimerType.ScoreTimer:
				this.decimalPlaces = 0;
				this.countDescrete = false;
				this.textAsMinutesAndSeconds = false;
				break;
			case TimerType.RoundTimer_Seconds:
				this.decimalPlaces = 0;
				this.countDescrete = true;
				this.textAsMinutesAndSeconds = true;
				break;
			case TimerType.RoundTimer_Tenths:
				this.decimalPlaces = 1;
				this.countDescrete = false;
				this.textAsMinutesAndSeconds = false;
			break;
		};

		this.startingTime = roundTime;

		if (this.startingTime<=0.0f)
		{
			this.countDescrete = false;
		}


		Reset();

		UpdateTimeText();
	}
	
	public void SetTime(float time)
	{
		this.currentTime = time;
		this.timeChunked = time;
		this.timerActive = true;
		
		
		UpdateTimerMaterial(1.0f);
		UpdateTimeText();	
	}
	
	
	public void Reset()
	{
		this.currentTime = this.startingTime;
		this.timeChunked = this.currentTime;
		//this.timerActive = false;
		
		
		UpdateTimerMaterial(1.0f);
		UpdateTimeText();
	}
	
	private void UpdateTimerMaterial(float normalisedTime)
	{


		if (this.timerMaterial)
		{
			this.timerMaterial.SetFloat("_Cutoff", Mathf.Clamp(1.0f-normalisedTime,minMaterialVal*2.0f,maxMaterialVal));
		}

	}
	
	void HandleTimeUp()
	{

		if (mainGame && this.messageMainGameOnTimeUp)
			mainGame.HandleTimeUp();
		
		StopTimer();
		

	}
	
	public void ShowTimer(bool bShow = true)
	{
		if (this.GetComponent<Animation>())
		{
			if (bShow)
			{
				this.GetComponent<Animation>().Play("anm_TimerShow",PlayMode.StopAll);
			}
			else
			{
				this.GetComponent<Animation>().Play("anm_TimerHide",PlayMode.StopAll);
			}
		}
	}
	
	
	
	public void StartTimer(float roundDuration = -1, bool waitForPress = false)
	{
		if (roundDuration>0)
		{
			this.startingTime = roundDuration;
			Reset();
		}

		this.timerPausedUntilPress = waitForPress;

		
		this.timerActive = true;
	}
	
	public void StopTimer()
	{
		this.timerActive = false;
	}
	
	public float GetNormalisedTime()
	{
		float normTime = ((this.currentTime/this.startingTime));	 
		if (this.countDescrete)
			normTime = ((this.timeChunked/this.startingTime));

		
		return Mathf.Clamp(normTime,0.00f,1.0f);
		
		
		
		
	}
	
	
}
