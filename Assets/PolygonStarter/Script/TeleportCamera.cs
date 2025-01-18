using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TeleportCamera : MonoBehaviour
{
    [SerializeField]  CinemachineVirtualCamera cameraTargetObject;

    [SerializeField] CinemachineFreeLook cameraPlayer;

    GameObject targetObject;

    public void SetTargetObject(Transform gg){
        targetObject=gg.gameObject;
        cameraTargetObject.Follow=gg;
        cameraTargetObject.LookAt=gg;
    }
    public void SwapPriorityCamrera(){
        if(cameraPlayer.Priority > cameraTargetObject.Priority){
            cameraPlayer.Priority=5;
            cameraTargetObject.Priority=10; 
        }
        else {
            cameraPlayer.Priority=10;
            cameraTargetObject.Priority=5;
        }
    }
}
