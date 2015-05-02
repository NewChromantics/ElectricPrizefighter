using UnityEngine;
using System.Collections;

//An extension to the pooling system that allows us to 'auto' delete pooled objects
//Handling for.....
//		Animation runs out


public class PoolObject : MonoBehaviour 
{
	public PoolingSystem<PoolObject>	poolOwner 			= null;
	private Animation					anim				= null;
	private ParticleSystem				particle			= null;
	public float						timeout				= -1.0f;
	private float						timer				= 0.0f;

	public enum DieOn
	{
		NONE,
		ANIMEND,
		PARTICLEEND,
		TIMEOUT
	}
	
	public DieOn						dieOn				= DieOn.NONE;	
	
	
	bool CheckForDieOnRelease()
	{
		//for objects that have animations - we check to see if the animation has finished playing
		//If it has then it is able to be released
		if ((this.dieOn == DieOn.ANIMEND))
		{
			if (this.anim)
			{
				if (this.anim.isPlaying == false)
				{
					poolOwner.ReleaseElement(this, true);
					return true;
				}
			}
		}
		else if (this.dieOn == DieOn.PARTICLEEND)
		{
			if (this.particle)
			{
				if((this.particle.particleCount == 0) && (this.particle.IsAlive() == false))
				{
					poolOwner.ReleaseElement(this, true);
					return true;
				}
			}
		}
		else if (this.dieOn == DieOn.TIMEOUT)
		{
			if (this.timer<0.0f)
			{
				this.timer = this.timeout;
				poolOwner.ReleaseElement(this, true);
				return true;
			}
			else
			{
				this.timer-=Time.deltaTime;
			}
		}
		
		
		return false;
	}

	void Update()
	{
		if (this.poolOwner!=null)
			CheckForDieOnRelease();
	}
	
	void Awake()
	{
		this.anim = GetComponentInChildren<Animation>();
		this.particle = GetComponentInChildren<ParticleSystem>();
		this.timer = this.timeout;

	}
	
	
	
}
