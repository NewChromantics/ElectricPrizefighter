using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
//using System.Reflection;
//using System.Text;

public class ButtonObj : MonoBehaviour 
{
	public enum ButtonType
	{
		Normal,				//Press/hold etc
		Toggle,				//Toggle on/off
		Radio,				//select one of group
		RadioToggleOff,		//select one of group or none
		Counter				//Activates after a set time of selection
	};

	public int				buttonID				= 0;
	public ButtonType		buttonType 				= ButtonType.Normal;
	public List<ButtonObj>	radioButtonList			= new List<ButtonObj>();

	//float 				switchTimer 			= 0.0f;
	public bool 			selected 				= false;
	ButtonSelectEffect		selectEffect 			= null;
	
	public Sprite			spriteNormal			=	null;
	public Sprite			spriteSelected			=	null;
	public SpriteRenderer	spriteRenderer			=	null;

	public List<TextMesh>	textMeshes				=	new List<TextMesh>();

	public ButtonObj		buttonMessageReceiver	= 	null;
	public ButtonObj		buttonMessageForwarded	=	null;

	public float			selectTimer				= 	0.0f;
	public float			selectTimerTarget		=	2.0f;



	//List of Button Action Indexes - these are assigned to the instances of the button prefabs and then decode to a function
	//To call via a dictionary
	public enum ActionIndex
	{
		//Main Menu Buttons

		Unset = -1,
		PlayGame,
		SelectCharacters,
		About,
		ReturnToMainMenu,

		//Prompt buttons
		Prompt_No,
		Prompt_Yes,

		//Player Select
		Player_Next,
		Player_Previous,


		PauseMainGame,
		ReturnToUserSelect,
	}
	
	private static Dictionary<ActionIndex, Func<ActionIndex,GameObject,ButtonObj,bool>> ButtonActions;
	private static Dictionary<ActionIndex, Func<ActionIndex,GameObject,ButtonObj,bool>> MouseOverActions;

	public ActionIndex			actionIndex				= ActionIndex.Unset;

	void Start() 
	{
		this.selectEffect = this.transform.parent.gameObject.GetComponent<ButtonSelectEffect>();

		this.spriteRenderer	= this.GetComponent<SpriteRenderer>();

		//Setup the button to function remap dictionary
		if (ButtonActions==null)
		{
			ButtonActions = new Dictionary<ActionIndex, Func<ActionIndex,GameObject,ButtonObj,bool>>
			{
				{ActionIndex.PlayGame,				MainMenu.BA_PlayLevel			},
				{ActionIndex.About,					MainMenu.BA_About				},
				{ActionIndex.SelectCharacters,		MainMenu.BA_SelectPlayers		},

			};
		}

		if (MouseOverActions==null)
		{
			MouseOverActions = new Dictionary<ActionIndex, Func<ActionIndex,GameObject,ButtonObj,bool>>
			{
				{ActionIndex.SelectCharacters,		MA_UpdateProgress		},
				{ActionIndex.PlayGame,				MA_UpdateProgress		},

			};
		}
	}


	public static bool MA_UpdateProgress(ButtonObj.ActionIndex actionIndex, GameObject ownerButton, ButtonObj buttonObj)
	{
		if (HUD_CircleProgress.Instance!=null)
		{
			HUD_CircleProgress.Instance.gameObject.SetActive(true);
			HUD_CircleProgress.Instance.NormalisedProgress = buttonObj.selectTimer/buttonObj.selectTimerTarget;
			HUD_CircleProgress.Instance.ownerObj = buttonObj;
		}


		return true;
	}



	// Update is called once per frame
	void Update () 
	{
		if (this.spriteRenderer!=null && this.spriteSelected!=null && this.spriteNormal!=null)
		{
			this.spriteRenderer.sprite = (this.selected ? this.spriteSelected : this.spriteNormal);
		}
	}
	
	public void SetSelected(bool sel)
	{
		
		
		if (this.buttonMessageReceiver)
		{
			this.selected = sel;
			buttonMessageReceiver.SetSelected(sel);
			return;
		}
		
		if (this.selected != sel)
		{
			this.selected = sel;
		
			if (this.selectEffect!=null)
				this.selectEffect.SetOn(sel);
		}	
	}

	public void ToggleRadioSelected(bool allowToggleToOff = false)
	{
		//Debug.Log("ToggleRadioSelected");
		if (allowToggleToOff)
		{
			if (this.selected==true)
			{
				foreach (ButtonObj bttn in this.radioButtonList)
				{
					//Debug.Log("ToggleRadioSelected turning all off " + bttn.name);
					bttn.SetSelected(false);
				}
				return;
			}
		}



		if (this.selected==false)
		{
			foreach (ButtonObj bttn in this.radioButtonList)
			{
				bttn.SetSelected(bttn==this);
			}
		}

	}


	
	public void HandleMousePress()
	{
		switch (this.buttonType)
		{
			case ButtonType.Normal:
			case ButtonType.Counter:
				SetSelected(true);	
				break;

			case ButtonType.Toggle:
				SetSelected(!this.selected);	
				break;

			case ButtonType.Radio:
				ToggleRadioSelected();
				DoAction();
				break;

			case ButtonType.RadioToggleOff:
				ToggleRadioSelected(true);
				DoAction();
				break;
		};



		//Debug.Log(this.name + " HandleMousePress");
	}
	
	
	public void HandleMouseOver()
	{
		switch (this.buttonType)
		{
			case ButtonType.Normal:
				SetSelected(true);	
				break;

			case ButtonType.Counter:
				SetSelected(true);	
				
				this.selectTimer+=Time.deltaTime;
				if (this.selectTimer>=this.selectTimerTarget)
				{
					HUD_CircleProgress.Instance.NormalisedProgress = 0.0f;
					DoAction();
				}

				DoMouseOverAction();
				
				break;

		};



		//Debug.Log(this.name + " HandleMouseOver");

	}
	
	public void HandleMouseUp()
	{
		//Debug.Log(this.name + " HandleMouseUp");

	

		switch (this.buttonType)
		{
			case ButtonType.Normal:
				if (this.selected)
				{
					//Debug.Log(">>>>>>>>>>>>PRESSED " + this.name + "!!!!!!!!!!!!!!");
					//Application.LoadLevel("SumGameScene");
					DoAction();
				}
				SetSelected(false);	
				break;
		};

	
	}
	
	public void OnMouseDownInvalidate()
	{
		//Debug.Log(this.name + " OnMouseDownInvalidate");

		//mouse up event received for previously selected item
		switch (this.buttonType)
		{
			case ButtonType.Normal:
				SetSelected(false);	
				this.selectTimer = 0.0f;
				HUD_CircleProgress.Instance.NormalisedProgress = 0.0f;
				break;
			case ButtonType.Counter:
				SetSelected(false);	
				this.selectTimer = 0.0f;
				HUD_CircleProgress.Instance.NormalisedProgress = 0.0f;

				if (HUD_CircleProgress.Instance.ownerObj==this)
				{
					HUD_CircleProgress.Instance.Reset();
				}
				break;
		};



	}
	
	private void DoAction()
	{
		//if (DebugMenu.DisplayMenu)
		//	return;

		//Debug.Log("Button Object DoAction() " + this.actionIndex.ToString());
		//MainMenu.returnTargetCameraIndex();


		if (ButtonActions.ContainsKey(this.actionIndex))
		{
			ButtonActions[this.actionIndex].Invoke(this.actionIndex, this.gameObject, this); 

			MasterAudio.PlaySound("Sfx_Select");
		}
	}

	private void DoMouseOverAction()
	{
		if (MouseOverActions.ContainsKey(this.actionIndex))
		{
			MouseOverActions[this.actionIndex].Invoke(this.actionIndex, this.gameObject, this); 
		}
	}


	
	
}


