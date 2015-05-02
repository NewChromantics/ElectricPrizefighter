using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HUDManager : Singleton<HUDManager> 
{

	public List<GameObject>	hudPrefabs	=	new List<GameObject>();

	public Dictionary<string, GameObject>	hudList = new Dictionary<string, GameObject>();


	public GameObject	hudPrefab_Prompt 	= null;	
	public GameObject	hudPrefab_Hinttext			= null;



	// Use this for initialization
	void Start () 
	{
		if (hudPrefab_Prompt!=null)
		{
			GameObject goPrompt = (GameObject)Instantiate(hudPrefab_Prompt, new Vector3(0.0f,0.0f,500.0f), Quaternion.identity);
			
			if (goPrompt)
			{
				goPrompt.name = hudPrefab_Prompt.name;
				goPrompt.transform.parent = this.transform;

				goPrompt.SetActive(false);
			}
		}

		if (hudPrefab_Hinttext!=null)
		{
			GameObject goHint = (GameObject)Instantiate(hudPrefab_Hinttext, Vector3.zero, Quaternion.identity);
			
			if (goHint)
			{
				goHint.name = hudPrefab_Hinttext.name;
				goHint.transform.parent = this.transform;
			}
		}

		CreateHUDs();

	}

	private void CreateHUDs()
	{
		//Instantiate each hud and add to dictionary

		foreach(GameObject hudPrefab in hudPrefabs)
		{
			if (hudPrefab!=null)
			{
				GameObject hudObj = Instantiate(hudPrefab);

				if (hudObj)
				{
					hudList.Add(hudPrefab.name, hudObj);
					hudObj.transform.parent = this.transform;
					hudObj.name = hudPrefab.name;	//remove the clone bit - yuk!
				}
			}
		}
	}

	public GameObject GetHUDObject(string hudName)
	{
		if (this.hudList.ContainsKey(hudName))
		{
			return this.hudList[hudName];
		}

		return null;
	}
	
	public void HideHUD(string hudName)
	{
		ShowHUD(hudName,false);
	}

	public void ShowHUD(string hudName, bool show=true)
	{
		GameObject hudObj = GetHUDObject(hudName);

		if (hudObj)
		{
			hudObj.SetActive(show);

			HUDPositioner hPos = hudObj.GetComponent<HUDPositioner>();

			if (hPos)
				hPos.UpdateHUD(true);

		}
	}



	// Update is called once per frame
	void Update () 
	{
	
	}
}
