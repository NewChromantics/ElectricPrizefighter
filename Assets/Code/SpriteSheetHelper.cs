using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteSheetHelper : MonoBehaviour 
{
	private SpriteRenderer	spriteRenderer;

	public List<Sprite>		spriteSheet = new List<Sprite>();

	public Dictionary<string,Sprite>	spriteDictionary = new Dictionary<string, Sprite>();

	public bool randomiseOnStart 	= false;
	public bool randomAngle			= false;



	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		foreach (Sprite sprite in this.spriteSheet)
		{
			this.spriteDictionary.Add(sprite.name, sprite);
		}

		if (this.randomiseOnStart)
		{
			SetRandomSprite();
		}

		if (this.randomAngle)
		{
			this.spriteRenderer.transform.Rotate(new Vector3(0.0f,0.0f,Random.Range(0,360.0f)));
		}

	}

	public void SetRandomSprite()
	{
		this.spriteRenderer.sprite = this.spriteSheet[Random.Range(0,this.spriteSheet.Count)];

		if (this.randomAngle)
		{
			this.spriteRenderer.transform.Rotate(new Vector3(0.0f,0.0f,Random.Range(0,360.0f)));
		}
	}

	public void SetSprite(string spriteName)
	{
		if (spriteDictionary.ContainsKey(spriteName))
			this.spriteRenderer.sprite = this.spriteDictionary[spriteName];
	}

	public Sprite GetSprite(string spriteName)
	{
		if (spriteDictionary.ContainsKey(spriteName))
			return this.spriteDictionary[spriteName];

		return null;
	}

	public int SetSpriteByIndex(int Index)
	{
		if (Index<0)
			Index+=spriteSheet.Count;

		Index = Index%spriteSheet.Count;
		this.spriteRenderer.sprite = this.spriteSheet[Index];

		return Index;
	}

	public Sprite GetSpriteByIndex(int Index)
	{
		if (Index<0)
			Index+=spriteSheet.Count;
		
		Index = Index%spriteSheet.Count;
		return this.spriteSheet[Index];

	}

}
