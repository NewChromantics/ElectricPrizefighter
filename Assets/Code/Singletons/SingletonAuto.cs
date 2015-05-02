using UnityEngine;

public class SingletonAuto<T> : MonoBehaviour where T : MonoBehaviour
{
	private	static	T		_instance 	= null;
	private static	bool	_destroyed	= false;
	private static	object 	_lock		= new object();

	public static T Instance
	{
		get
		{
			lock(_lock)
			{
				if (_destroyed)
					return null;

				if (_instance == null)
				{
					GameObject	singleton = new GameObject();

					if (singleton)
					{
						singleton.name = "[Singleton] " + typeof(T).ToString();
						DontDestroyOnLoad(singleton);
					
						_instance = singleton.AddComponent<T>();
					}
				}

				return _instance;
			}
		}
	}

	protected virtual void OnDestroy()
	{
		_destroyed = true;
		_instance  = null;
	}

	public static bool ValidInstance()
	{
		return (_instance != null)? true : false;
	}
}
