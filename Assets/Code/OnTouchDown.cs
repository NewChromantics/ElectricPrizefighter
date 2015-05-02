
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OnTouchDown : MonoBehaviour
{
	GameObject		objMouseDown 	= null;
	
	bool			debugLog 		= false;
	public Camera 	camera 			= null;
	public bool		vrCamType		= false;

	public bool		debug			= false;

	void Update () 
	{
		if (this.camera == null)
			this.camera = Camera.main;


		/*
		foreach (Touch touch in Input.touches) 
		{
			Ray touchRay = this.camera.ScreenPointToRay(touch.position);

			//ColliderView.Marker3D.AddMarker(touchRay.origin+touchRay.direction,500.0f,Color.cyan);

		}
		*/

		Vector3 vAimPos = Input.mousePosition;

		if (vrCamType)
		{
			//Just aim down the centre of the camera
			vAimPos = new Vector3(Screen.width/2,Screen.height/2,0);
		}

		// Construct a ray from the current touch coordinates
		Ray ray = this.camera.ScreenPointToRay(vAimPos);


		//ColliderView.Marker3D.AddMarker(ray.origin+ray.direction,250.0f,Color.blue);

		float raylength = 5000.0f;
		
		//RaycastHit[] hits;
		
		//TODO add a layer and cast to that
		//hits = Physics.RaycastAll(ray.origin,ray.direction, raylength).OrderBy(h=>h.distance).ToArray();

		//hits = Physics.RaycastAll(ray.origin,ray.direction, raylength, layer).OrderBy(h=>h.distance).ToArray();
		int layerMask =  1<<((int)Layers.User_Touch);
		RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, raylength,layerMask);
		Array.Sort(hits, (x,y)=> x.distance.CompareTo(y.distance)); 

		if (debug)
		{
			Debug.DrawRay(this.camera.transform.position,ray.direction*raylength,Color.yellow);

			if (hits.Count()>0)
			{
				Debug.DrawLine(hits[0].point-Vector3.left,hits[0].point+Vector3.left, Color.green);
				Debug.DrawLine(hits[0].point-Vector3.up,hits[0].point+Vector3.up, Color.green);
				Debug.DrawLine(hits[0].point-Vector3.forward,hits[0].point+Vector3.forward, Color.green);
			}
		}



		bool bInvalidateObjMouseDown = false;
		
		GameObject	newObjMouseDown = null;
		
		//Debug.Log(string.Format("Number of Hits {0}", hits.Length));
		if (hits.Length>0) 
		{
			GameObject hitObject = hits[0].collider.gameObject;
		
			//float fSize = 200.0f;

			////ColliderView.Marker3D .Render(hits[0].point,fSize,Color.blue);
			//ColliderView.Marker3D.AddMarker(hits[0].point,fSize,Color.blue);

			//Debug.Log("raycast testing '" + hitObject.name + "'");

			if (Input.GetMouseButtonDown(0)&&(!vrCamType))
			{
				newObjMouseDown = hitObject;
				hitObject.SendMessage("HandleMousePress",SendMessageOptions.DontRequireReceiver);
				//ColliderView.Marker3D.AddMarker(hits[0].point,fSize,Color.yellow);
				//if (this.debugLog) //Debug.Log("HandleMousePress " + hitObject.name);
	     	}
			else if (Input.GetMouseButtonUp(0)&&(!vrCamType)) 
			{
				hitObject.SendMessage("HandleMouseUp",SendMessageOptions.DontRequireReceiver);
			
				//if (this.debugLog) //Debug.Log("HandleMouseUp " + hitObject.name);
				
				if ((this.objMouseDown!=hitObject)&&(this.objMouseDown))
				{
					bInvalidateObjMouseDown = true;
				}
				//ColliderView.Marker3D.AddMarker(hits[0].point,fSize,Color.red);
			}
			else if (Input.GetMouseButton(0)||(vrCamType)) 
			{
				
				hitObject.SendMessage("HandleMouseOver",SendMessageOptions.DontRequireReceiver);
				//if (this.debugLog) 
				//	//Debug.Log("HandleMouseOver " + hitObject.name);
				
				if ((this.objMouseDown!=hitObject)&&(this.objMouseDown))
				{
					bInvalidateObjMouseDown = true;
				}
				newObjMouseDown = hitObject;
				//ColliderView.Marker3D.AddMarker(hits[0].point,fSize,Color.green);
	     	}
			else
			{
				bInvalidateObjMouseDown = true;
				//ColliderView.Marker3D.AddMarker(hits[0].point,fSize,Color.magenta);
			}

		}
		else
		{
			bInvalidateObjMouseDown = true;
		}

		if ((bInvalidateObjMouseDown)||(this.objMouseDown!=newObjMouseDown))
		{
			if (this.objMouseDown && this.objMouseDown.activeSelf)
			{
				if (this.debugLog) 
				{
					if (objMouseDown.transform.parent!=null)
					{
						if (objMouseDown.transform.parent.parent!=null)
						{
							////Debug.Log("OnMouseDownInvalidate " + objMouseDown.transform.parent.parent.name + " " +objMouseDown.transform.parent.name + " " + objMouseDown.transform.name);
						}
						else
						{
							////Debug.Log("OnMouseDownInvalidate " + objMouseDown.transform.parent.name + " " + objMouseDown.transform.name);
						}
					}
					//else 
					//	//Debug.Log("OnMouseDownInvalidate " + objMouseDown.name);
				}

				this.objMouseDown.SendMessage("OnMouseDownInvalidate",SendMessageOptions.DontRequireReceiver);
			}
			
			this.objMouseDown = newObjMouseDown;
		}
	}
	
	void OnGUI()
	{
		GUI.contentColor = Color.red;
		if (this.objMouseDown)
			GUI.Label(new Rect(20,20,100,50), string.Format("onTouch {0}", this.objMouseDown.name));
		else
			GUI.Label(new Rect(20,20,100,50), string.Format("onTouch NONE"));
			
	}

/*
#if UNITY_EDITOR
	
#else
	void Update () 
	{
		//return;
		// Code for OnMouseDown in the iPhone. Unquote to test.
		RaycastHit hit = new RaycastHit();
		for (int i = 0; i < Input.touchCount; ++i) 
		{
			if (Input.GetTouch(i).phase.Equals(TouchPhase.Began)) 
			{
				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				if (Physics.Raycast(ray, out hit)) 
				{
					hitObject.SendMessage("OnMouseDown2");
		     	}
		  	}
			else if (Input.GetTouch(i).phase.Equals(TouchPhase.Moved)) 
			{
				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				if (Physics.Raycast(ray, out hit)) 
				{
					hitObject.SendMessage("OnMouseOver2");
		     	}
		  	}

			
	   	}
	}
#endif
*/
}
