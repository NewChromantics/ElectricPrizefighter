using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour 
{
	
	//Rotater
	public float m_fRotateRate 	= 0.5f;
	private float m_fRotation	 = 0.0f;

	//Scaler
	public float m_fScaleMin	= 1.0f;
	public float m_fScaleMax	= 5.0f;
	public float m_fScaleRate	= 1.0f;
	private float m_fScaler		= 0.0f;
	
	//Turn Off
	private bool m_bTurnOff			= false;
	private float m_fTurnOffScaler	= 1.0f;
	
	// Use this for initialization
	public virtual void Start () 
	{
		UpdateScale();
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	public virtual void Update () 
	{
		//transform.Rotate();// - new Vector3(0.0f,0.0f,));
		m_fRotation+=m_fRotateRate*Time.deltaTime*30.0f;
		Quaternion qRot = Quaternion.Euler(0,0,m_fRotation);
		
		Quaternion qLocal = Quaternion.identity;
		
		if (transform.parent)
			qLocal = Quaternion.Euler(-transform.parent.rotation.eulerAngles);
		
		transform.localRotation = qLocal*qRot;
		
		UpdateScale();
	}
	
	private void UpdateScale()
	{
		m_fScaler+=m_fScaleRate*Time.deltaTime;
		
		float NewScale = Mathf.Lerp(m_fScaleMin,m_fScaleMax, ((Mathf.Sin(m_fScaler))+1.0f)/2.0f);
		
		if (m_bTurnOff)
		{
			m_fTurnOffScaler = Mathf.Lerp(m_fTurnOffScaler,0,0.1f);
			
			NewScale*=m_fTurnOffScaler;
			
			if (NewScale<0.05f)
			{
				gameObject.SetActive(false);
			}
		}
		else
		{
			if (m_fTurnOffScaler<0.99f)
			{
				//Turning on - or already on!
				m_fTurnOffScaler = Mathf.Lerp(m_fTurnOffScaler,1.0f,0.5f);
				NewScale*=m_fTurnOffScaler;
			}
		}
		
		
		transform.localScale = new Vector3(NewScale,NewScale,NewScale);

	}
	
	
	
	public void TurnOff()
	{
		TurnOn(false);
	}
	
	public void TurnOn(bool bTurnOn = true)
	{
		m_bTurnOff = !bTurnOn;
		
		
		if (bTurnOn)
		{
			gameObject.SetActive(bTurnOn);
			m_fTurnOffScaler = 0.0f;
		}
		else
		{
			m_fTurnOffScaler = 1.0f;	
		}
	}
	
	
}
