using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonSelectEffect : MonoBehaviour 
{
	private float	scaleTimer 			= 	0;
	
	//Scale
	public float	scaleRate		=	5.0f;
	
	public float	scale1			=	1.0f;
	public float	scale2			=	1.1f;
	public bool		forceScale		=	false;
	private float	currentScale	= 	1.0f;
	
	//Rock Rotater
	public float	rotateMax		=	5.0f;
	private float	rotateTimer		= 	0;
	public float	rotateRate		=	2.5f;
	public float	rotateOffset	=	0.0f;
	public bool		rotateRocker	=	true;
	
	private float	onOffValue		=	1.0f;
	private bool	on				= 	false;
	public bool		alwaysOn		=	false;
	
	public string	buttonText		= 	"Button Text";
	
	public float	appearDelay		=	1.0f;
	private Animation	appearAnim	=	null;
	private bool		appearAnimPlaying = false;
	private bool		playedAppearSound = false;
	public float		sfxAppearPlayTime = 0.9f;
	//public AudioSource	sfxAppear = null;
	public string		sfxAppearString = "";
	
	public Rigidbody	rBody = null;

	public MainMenu.MenuIndex	menuIndex = MainMenu.MenuIndex.Unset;

	private List<TextMesh>	textMeshes = new List<TextMesh>();
	
	void Start()
	{
		this.scaleTimer = 0;
		this.currentScale = this.scale1;
		
		this.rotateTimer = 0.0f;
		
		
		
		this.on = this.alwaysOn;
		
		this.onOffValue = (this.on) ? 1.0f : 0.0f;
		
		foreach (TextMesh tm in GetComponentsInChildren<TextMesh>())
		{
			if (!tm.name.Contains("FIXED"))
				this.textMeshes.Add(tm);
		}
		
		if (this.buttonText!="")
			SetText(buttonText);
		
		this.appearAnim = this.GetComponentInChildren<Animation>();

		this.appearAnimPlaying = false;
		this.playedAppearSound = false;

		//this.rBody = this.gameObject.AddComponent<Rigidbody>();

		if (this.rBody==null)
		{
			this.rBody = GetComponentInChildren<Rigidbody>();
		}



		if (this.rBody)
		{	
			this.rBody.useGravity = false;
			this.rBody.isKinematic = true;
		}

		
	}

	void SetText(string textString)
	{
		foreach (TextMesh tm in this.textMeshes)
		{
			tm.text = textString;
		}
	}
	
	
	void Update()
	{
		/*
		if (this.name=="RotateAndScalerPracticeInfo")
		{
			//Debug.Log("RotateAndScalerPracticeInfo");
		}
		*/

		if (this.appearDelay>0)
		{
			this.appearDelay-=Time.deltaTime;
			
			if (this.appearDelay<=0.0f)
			{
				if (this.appearAnim)
				{
					this.appearAnim.Play("anm_ButtonAppear");
					this.appearAnimPlaying = true;
				}
			}
		}
		else
		{
			if (this.appearAnimPlaying)
			{
				float appearedAnimProgress = this.appearAnim["anm_ButtonAppear"].normalizedTime;
				if ((appearedAnimProgress>this.sfxAppearPlayTime)&&
				    (this.playedAppearSound==false))
				{
					this.playedAppearSound = true;

					if (this.sfxAppearString!="")
						MasterAudio.PlaySound(this.sfxAppearString);
				}

				if (this.playedAppearSound == true)
				{
					if ((appearedAnimProgress>0.99f)||
						(appearedAnimProgress==0))
					{
						this.appearAnimPlaying = false;
					}
				}
			}
		}
		
		if ((MainMenu.getTargetCameraIndex()==(int)this.menuIndex)||
		    (this.menuIndex==MainMenu.MenuIndex.Unset))
		{
			UpdateScale();
			UpdateRotate();
		}
	}

	private void UpdateScale()
	{
		//Update on/off value
		bool isOn = this.on|this.forceScale;

		float fTargetValue = (isOn) ? 1.0f : 0.0f;
		this.onOffValue = Mathf.Lerp(this.onOffValue,fTargetValue,0.1f);
		
		this.scaleTimer+=(Time.deltaTime*this.scaleRate);
		
		float scaleTimerCos = (Mathf.Cos(this.scaleTimer)+1.0f)*0.5f;
		
		float fTargetScale = Mathf.Lerp(this.scale1,scale2,onOffValue);
		
		currentScale = Mathf.Lerp(scale1,fTargetScale,scaleTimerCos);

		//Performance - only update the scale if required
		if (this.rBody)
		{
			if ((Mathf.Abs(currentScale-this.rBody.transform.localScale.x))>0.005f)
				this.rBody.transform.localScale = new Vector3(currentScale,currentScale,currentScale);

		}
		else
		{
			if ((Mathf.Abs(currentScale-this.transform.localScale.x))>0.005f)
				this.transform.localScale = new Vector3(currentScale,currentScale,currentScale);
		}

	}

	private void UpdateRotate()
	{


		this.rotateTimer+=(Time.deltaTime*this.rotateRate);
		
		float rotateCos = (this.rotateTimer+this.rotateOffset);
		
		if (this.rotateRocker)
		{
			rotateCos = (Mathf.Cos(rotateCos));
		}
		
		
		Quaternion qRot = Quaternion.Euler(0,0,rotateCos*this.rotateMax);

		if (this.rBody && (this.appearAnimPlaying==false))
			this.rBody.rotation = qRot;
		else
			transform.localRotation = qRot;
	}
	
	
	public void SetOn(bool bTurnOn)
	{
		if (this.alwaysOn)
		{
			this.on = this.alwaysOn;
			return;
		}
		
		
		if (this.on != bTurnOn)
		{
			if (bTurnOn)
				this.scaleTimer = 0.0f;
			
			this.on = bTurnOn;
		}
	}
	
}
