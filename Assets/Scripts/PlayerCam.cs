using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private PlatformPlayer playerScript;
    private float currentSpeed;
    private bool dashing;

    private float minSize = 13f;
    private float maxSize; 
    private float speedMult = 50f;
    private float sizeCheck;
    float initVel = 0f;

    void Start() 
    {
        playerScript = GetComponent<PlatformPlayer>();
        dashing = false;
        maxSize = minSize * (1f + 25f/speedMult);
    }

    void Update()
    {
        currentSpeed = playerScript.currentSpeed;
        cam.transform.position = transform.position + new Vector3(0,0,-10f);

        sizeCheck = minSize * (1 + Mathf.Abs(currentSpeed)/speedMult);

        if(sizeCheck < maxSize && !dashing) {
            cam.orthographicSize = sizeCheck;
        }
        else if(sizeCheck >= maxSize && !dashing) {
            cam.orthographicSize = maxSize;
        }
        if(playerScript.dashing && playerScript.dashCount >= 0 && !dashing) {
            StartCoroutine(DashCameraBoost());
        }
    }

    IEnumerator DashCameraBoost() 
    {
        dashing = true;  
        float target = cam.orthographicSize + 5f;

        while(cam.orthographicSize < target) {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, target, ref initVel, 0.02f); 
            yield return new WaitForEndOfFrame(); 
        }
        
        dashing = false;    
    }
}
