using UnityEngine;
using System.Collections;

public class HUD_CircleProgress :  Singleton<HUD_CircleProgress> 
{
	public Material	circleMaterial				= null;
	public ButtonObj ownerObj					= null;
	public float normalisedProgress 			= 0.0f;
	private float previousNormalisedProgress	= 0.0f;

	public float NormalisedProgress 
	{ 
		get 
		{
			return normalisedProgress;
		}

		set 
		{
			normalisedProgress = value;
			Refresh();
		}
	}

	void Start()
	{

	}

	void Update()
	{
		//Refresh();
	}

	public void Reset()
	{
		this.ownerObj = null;
		NormalisedProgress = 0.0f;

	}

	private void Refresh()
	{
		//this.normalisedProgress = Mathf.Clamp(this.normalisedProgress,0.001f,1.0f);

		if (this.previousNormalisedProgress!=this.normalisedProgress)
		{
			this.previousNormalisedProgress=this.normalisedProgress;

			if (this.circleMaterial!=null)
			{
				this.circleMaterial.SetFloat("_Cutoff",Mathf.Clamp(1.0f-this.normalisedProgress,0.001f,1.0f));
			}

		}
	}

}
