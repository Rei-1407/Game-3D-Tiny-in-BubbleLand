using System.Collections;
using System.Collections.Generic;
using Platformer;
using UnityEngine;

public class CharacterAiming : MonoBehaviour
{
    public float turnSpeed;
    Camera mainCam;

    PlayerController playerController;
    private void Awake() {
        playerController=GetComponent<PlayerController>();
    }
    private void Start() {
        mainCam=Camera.main;
        Cursor.visible=true;
        Cursor.lockState=CursorLockMode.Locked;
    }
    void FixedUpdate()
    {   
        if(playerController.GetCurrentSpeed()-0.01f<=0 || playerController.isPistol()){
            float yamCamera=mainCam.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,yamCamera,0), turnSpeed * Time.deltaTime);

        }
    }
}
