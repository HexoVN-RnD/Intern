using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoolAtCamera : MonoBehaviour
{
    private void Update()
    {
        if (Camera.main != null) 
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}
