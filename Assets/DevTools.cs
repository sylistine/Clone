using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DevTools : MonoBehaviour
{
    float timeScale;

#if (UNITY_IPHONE || UNITY_ANDROID)
    public static List<string> mobileDebugLog = new List<string>();
#endif

	void Start ()
    {
        if (timeScale < 0) timeScale = 0;
	}
	
	void Update ()
    {
	
	}

#if (UNITY_IPHONE || UNITY_ANDROID)
    void OnGUI ()
    {
        for(int i = 0; i < mobileDebugLog.Count; i++)
        {
            GUI.Box(new Rect(12, 12 + 36 * i, 480, 24), mobileDebugLog[i]);
        }
    }
#endif
}
