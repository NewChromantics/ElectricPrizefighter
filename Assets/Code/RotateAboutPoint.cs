using UnityEngine;
using System.Collections;

public class RotateAboutPoint : MonoBehaviour 
{
	[System.Serializable]
	public enum RotateMode
	{
		Automatic = 0,
		Manual
	};

	public RotateMode	rotateMode		= RotateMode.Manual;
	public float 		rotateSpeed 	= 5.0f;
	public Transform	rotateAroundObj = null;

	public float 		deadZone		= 0.1f;

	private Vector3		vMousePressPoint = new Vector3();

	float rotationY = 0F;
	public float minimumY = -60F;
	public float maximumY = 60F;
	// Use this for initialization
	void Start () 
	{
	
	}

	void Update () 
	{
		if (rotateMode==RotateMode.Automatic)
			UpdateAutomatic();

		if (rotateMode==RotateMode.Manual)
			UpdateManual();

	}

	// Update is called once per frame
	void UpdateAutomatic () 
	{
		if (rotateAroundObj!=null)
			this.transform.RotateAround (rotateAroundObj.position, Vector3.up, rotateSpeed * Time.deltaTime);
		else
			this.transform.RotateAround (Vector3.zero, Vector3.up, rotateSpeed * Time.deltaTime);
	}

	void UpdateManual()
	{
		if (Input.GetMouseButtonDown(0))
			vMousePressPoint = new Vector3(Screen.width/2,Screen.height/2,0);

		if (Input.GetMouseButton(0))
		{
			Vector3 vDir = Input.mousePosition - vMousePressPoint;

			//this.transform.Rotate (this.transform.up, vDir.x * rotateSpeed  * Time.deltaTime,Space.World);
			//this.transform.Rotate (this.transform.right, vDir.y * rotateSpeed  * Time.deltaTime,Space.Self);

			float rotationX = transform.localEulerAngles.y + (vDir.x * rotateSpeed * Time.deltaTime);
			
			rotationY += (vDir.y * rotateSpeed * Time.deltaTime) ;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);


		}

	}

}
