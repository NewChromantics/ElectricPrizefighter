using UnityEngine;
using System.Collections;

public class HUD_AnimatedText : MonoBehaviour 
{
	public TextMesh		textMesh 	= null;
	public Animator		anim		= null;

	// Use this for initialization
	void Start () 
	{
		anim = textMesh.GetComponent<Animator>();
	}

	public void PlayText(string text)
	{
		textMesh.text = text;

		/*
		if (this.anim==null)
		{
			anim = this.GetComponent<Animation>();
		}
		*/
		if (this.anim!=null)
			anim.SetTrigger("PlayAppear");

	}

}
