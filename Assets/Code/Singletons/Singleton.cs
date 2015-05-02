using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private	static	T		_instance 	= null;
	private static	object 	_lock		= new object();

	public static T Instance
	{
		get
		{
			lock(_lock)
			{
#if UNITY_EDITOR
				if (FindObjectsOfType(typeof(T)).Length > 1)
					Debug.LogError("More than one instance of singleton " + typeof(T).ToString());
#endif
				if (_instance == null)
					_instance = (T)FindObjectOfType(typeof(T));

				return _instance;
			}
		}
	}

	public static bool ValidInstance()
	{
		return (_instance != null)? true : false;
	}
}
