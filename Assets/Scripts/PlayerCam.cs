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
        
        if(sizeCheck <= maxSize && !dashing) {
            cam.orthographicSize = sizeCheck;
        }
        if(playerScript.dashing && playerScript.dashCount >= 0) {
            StartCoroutine(DashCameraBoost());
        }
    }

    IEnumerator DashCameraBoost() 
    {
        dashing = true;
        int count = 0;
        
        while(count < 2) {
            cam.orthographicSize += 3f;
            
            yield return new WaitForSeconds(0.02f);

            count++;
        }

        while(count < 4) {
            cam.orthographicSize -= 3f;

            yield return new WaitForSeconds(0.02f);

            count++;
        }

        dashing = false;       
    }
}
