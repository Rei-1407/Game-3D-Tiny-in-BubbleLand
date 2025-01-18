using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Platformer;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigibody;
    private PlayerController player;
    TeleportCamera teleportCamera;
    [SerializeField] public float speed;
    private void Awake() {
        bulletRigibody= GetComponent<Rigidbody>();
        player=GameObject.Find("Player").GetComponent<PlayerController>();
        teleportCamera= GameObject.Find("Camera").GetComponent<TeleportCamera>();
        if(!player || ! teleportCamera){
            Debug.LogError("NULL CAM OR PLAYER");
        }
    }
    public void SetVelocity(Vector3 Dir){
        bulletRigibody.velocity = Dir * speed;
    }


    private void OnCollisionEnter(Collision other) {
        if(player.isTeleported){
            gameObject.SetActive(false);
            return;
        }
        teleportCamera.SetTargetObject(transform);
        DOVirtual.DelayedCall(0.1f,(()=>{
            teleportCamera.SwapPriorityCamrera();
            player.transform.DOMove(other.transform.position,0.2f).SetEase(Ease.InOutExpo).OnComplete(()=>{
                teleportCamera.SwapPriorityCamrera();
                //teleportCamera.SetTargetObject(null);
                gameObject.SetActive(false);
                player.isTeleported=true;
        });
        }));
       
    }
   
}
