using UnityEngine;
using System.Collections;

public class HUDPositioner : MonoBehaviour 
{
	public enum LCR
	{
		Left,
		Centre,
		Right
	};
	
	public enum TMB
	{
		Top,
		Middle,
		Bottom
	};
	
	public float 	normalisedX 		= 0;
	public float 	normalisedY 		= 0;
	public float 	normalisedScaleX 	= 1.0f;
	public float 	normalisedScaleY 	= -1.0f;

	public bool 	initialUpdateOnly 	= true;
	private bool	doneInitialUpdate 	= false;
	public bool 	smallestScaleXY		= false;
	public bool 	forceUpdate			= false;
	
	private float 	previousNormalisedX 		= 0;
	private float 	previousNormalisedY 		= 0;
	private float 	previousNormalisedScaleX	= 1.0f;
	private float 	previousNormalisedScaleY	= 1.0f;
	
	public LCR positioningLCR = LCR.Centre;
	public TMB positioningTMB = TMB.Middle;
	
	private LCR PREVIOUSpositioningLCR = LCR.Left;
	private TMB PREVIOUSpositioningTMB = TMB.Top;
	
	private int previousScreenWidth;
	private int previousScreenHeight;
	
	public Camera ownerCamera;
	
	private Vector3 screenTopLeft;
	private Vector3 screenBottomRight;
	
	private Transform anchorTL;
	private Transform anchorBR;
	private Transform anchor;
	
	private Vector3 anchorPosition;
	
	public Transform 	boundsObject = null;
	public bool			combineAllBoundsObjects = false;
	Bounds combinedBounds = new Bounds();
	
	
	// Use this for previousization
	void Start () 
	{
		previousScreenHeight = -1;
		previousScreenWidth = -1;

		SetupCamera();

		
		anchor 		= transform.FindChild("3DHUD_AnchorPoint");
		anchorBR 	= transform.FindChild("3DHUD_AnchorBottomRight");
		anchorTL 	= transform.FindChild("3DHUD_AnchorTopLeft");
		
		if (this.anchor)
			anchorPosition = anchor.position;

		this.forceUpdate = false;

	}
	
	private void SetupCamera()
	{
		if (!ownerCamera)
		{
			ownerCamera = transform.GetComponent<Camera>();
		
			if (!ownerCamera)
			{
				ownerCamera = GameObject.Find("3DHUDCamera").GetComponent<Camera>();
			}
		}
	}
	
	
	
	public void UpdateHUD(bool bForceUpdate = false)
	{
		if (bForceUpdate)
		{
			this.ownerCamera=null;
		}


		//this.initialUpdateOnly 	= true;
		//this.forceUpdate 		= false;



		if ((this.doneInitialUpdate==true) && (this.initialUpdateOnly==true) &&
		    (bForceUpdate==false)
		    )
			return;
		
		if (	
			((this.previousScreenHeight 	!= Screen.height)||
			 	(this.previousScreenWidth 		!= Screen.width)||
			 	(this.previousNormalisedX 		!= this.normalisedX) ||
			 	(this.previousNormalisedY 		!= this.normalisedY) ||
			 	(this.previousNormalisedScaleX	!= this.normalisedScaleX) ||
			 	(this.previousNormalisedScaleY	!= this.normalisedScaleY) ||
			 	(this.PREVIOUSpositioningLCR 	!= this.positioningLCR)||
			 	(this.PREVIOUSpositioningTMB 	!= this.positioningTMB)||
			 	(this.forceUpdate) || /*|| true*/
		 		(this.ownerCamera==null)
			)
		   )
		{
			if (this.ownerCamera==null)
				SetupCamera();
			
			UpdateAnchors();
			DoReposition();
			UpdateAnchors();
			DoReposition();
			forceUpdate = false;
			this.doneInitialUpdate = true;
		}	

	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateHUD();
	}
	
	void CalculateBounds()
	{
		if (boundsObject!=null)
		{
			this.combinedBounds = boundsObject.GetComponent<Renderer>().bounds;
			return;
		}
		else
		{
			Renderer[] renderers;
			renderers = GetComponentsInChildren<Renderer>();
	    	bool bFirst = true;
				
			foreach (Renderer render in renderers) 
			{
	    		//if (render != renderer) 
				{
					if (!render.name.Contains("Anchor"))
					{
						if (bFirst)
						{
							this.combinedBounds = render.bounds;
							bFirst = false;
							
							if (!this.combineAllBoundsObjects)
								return;
						}
						else
						{
							this.combinedBounds.Encapsulate(render.bounds);
						}
					}					
				}
	    	}
		}
	}
	
	
	
	void UpdateAnchors()
	{
		float AnchorNormX = 0.5f;
		float AnchorNormY = 0.5f;
		
		if (positioningLCR==LCR.Left)
			AnchorNormX = 0.0f;
		else if (positioningLCR==LCR.Right)
			AnchorNormX = 1.0f;
		
		if (positioningTMB==TMB.Top)
			AnchorNormY = 0.0f;
		else if (positioningTMB==TMB.Bottom)
			AnchorNormY = 1.0f;
		
		CalculateBounds();
		anchorPosition.x = Mathf.Lerp(this.combinedBounds.min.x,this.combinedBounds.max.x,AnchorNormX);
		anchorPosition.y = Mathf.Lerp(this.combinedBounds.max.y,this.combinedBounds.min.y,AnchorNormY);

		PREVIOUSpositioningLCR = positioningLCR;
		PREVIOUSpositioningTMB = positioningTMB;
		
		if (anchorBR)
		{
			Vector3 vAnchorBR = new Vector3(this.combinedBounds.max.x,this.combinedBounds.min.y,anchorBR.position.z);
			anchorBR.position = vAnchorBR;
		}
		
		if (anchorTL)
		{
			Vector3 vAnchorTL = new Vector3(this.combinedBounds.min.x,this.combinedBounds.max.y,anchorTL.position.z);
			anchorTL.position = vAnchorTL;
		}		
		
	}
	
	
	void DoReposition()
	{
		if (ownerCamera)
		{
			previousScreenHeight = Screen.height;
			previousScreenWidth = Screen.width;
			
			previousNormalisedX = normalisedX;
			previousNormalisedY = normalisedY;		
			
			previousNormalisedScaleX = normalisedScaleX;
			previousNormalisedScaleY = normalisedScaleY;
			
			float farClip = this.ownerCamera.farClipPlane;
			
			screenTopLeft = ownerCamera.ScreenToWorldPoint(new Vector3(0,previousScreenHeight,farClip));
			screenBottomRight = ownerCamera.ScreenToWorldPoint(new Vector3(previousScreenWidth,0,farClip));
			
			Vector3 vNewPosition = new Vector3
				(
					//Mathf.Lerp(screenTopLeft.x,screenBottomRight.x,normalisedX),
					//Mathf.Lerp(screenTopLeft.y,screenBottomRight.y,normalisedY),
					
					//Unclamped version of the above
					(this.screenTopLeft.x + ((this.screenBottomRight.x-this.screenTopLeft.x)*normalisedX)),
					(this.screenTopLeft.y + ((this.screenBottomRight.y-this.screenTopLeft.y)*normalisedY)),
					ownerCamera.farClipPlane
				);
			
			//Vector3 vMoveAmount = vNewPosition-anchor.position;
			Vector3 vMoveAmount = vNewPosition-anchorPosition;
			vMoveAmount.z = 0;
			
			this.transform.position+=vMoveAmount;
			
			
			float TargetWidth = 	(screenBottomRight.x - screenTopLeft.x)*normalisedScaleX;
			float BaseHUDWidth = 	(this.combinedBounds.max.x - this.combinedBounds.min.x)/transform.localScale.x;
			float NewScaleX = 		0.0f;
			
			if (BaseHUDWidth>0)
				NewScaleX = (TargetWidth/BaseHUDWidth);

			float TargetHeight =	(screenTopLeft.y - screenBottomRight.y)*normalisedScaleY;
			float BaseHUDHeight = 	(this.combinedBounds.max.y - this.combinedBounds.min.y)/transform.localScale.y;
			float NewScaleY = 		0;
			
			if (BaseHUDHeight>0)
				NewScaleY = (TargetHeight/BaseHUDHeight);
			
			if (NewScaleX>0 && NewScaleY>0)
			{
				if (this.smallestScaleXY)
				{
					//keep it square but use the smallest scale
					float smallestScale = Mathf.Min(NewScaleX,NewScaleY);
					this.transform.localScale = new Vector3(smallestScale,smallestScale,1.0f);
				}
				else
				{
					this.transform.localScale = new Vector3(NewScaleX,NewScaleY,1.0f);
				}
			}
			else if (NewScaleX>0)
			{
				this.transform.localScale = new Vector3(NewScaleX,NewScaleX,1.0f);
			}
			else if (NewScaleY>0)
			{
				this.transform.localScale = new Vector3(NewScaleY,NewScaleY,1.0f);
			}
			else
			{
				this.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
			}

			

			
			
			
			
		}
	}

}
