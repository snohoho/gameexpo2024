using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private PlatformPlayer playerScript;
    private float currentSpeed;
    private bool dashing;
    private bool dashingDeccel;
    private float sizeCheck;

    void Start() 
    {
        playerScript = GetComponent<PlatformPlayer>();
    }

    void Update()
    {
        currentSpeed = playerScript.currentSpeed;
        cam.transform.position = transform.position + new Vector3(0,0,-10f);

        sizeCheck = 15.0f * (1 + Mathf.Abs(currentSpeed)/70);
        
        if(sizeCheck <= (1 + Mathf.Abs(25)/70) && !dashing && !dashingDeccel) {
            cam.orthographicSize = sizeCheck;
        }
        if(playerScript.dashing && playerScript.dashCount >= 0) {
            StartCoroutine(DashCameraBoost());
        }
        if(cam.orthographicSize >= 20.1f && !dashing) {
            StartCoroutine(DashCameraDeccel());
        }
    }

    IEnumerator DashCameraBoost() 
    {
        dashing = true;
        
        while(cam.orthographicSize < 25.0f) {
            cam.orthographicSize += 0.8f;
            yield return new WaitForEndOfFrame();
        }

        dashing = false;       
    }

    IEnumerator DashCameraDeccel() {
        dashingDeccel = true;
        
        while(cam.orthographicSize > sizeCheck) {
            cam.orthographicSize -= 0.8f;
            yield return new WaitForEndOfFrame();
        }

        dashingDeccel = false;  
    }
}
