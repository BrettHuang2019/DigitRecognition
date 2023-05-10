using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    void Start()
    {
#if UNITY_IOS
        bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        if (deviceIsIpad)
        {
            transform.localPosition = new Vector3(-3.3f, 0.3f, -11);
            GetComponent<Camera>().orthographicSize = 2.3f;
        }
#endif
    }


    
}
