using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using Utilities;

namespace Platformer {
    public class PlayerController : MonoBehaviour {
        [Header("References")]
        [SerializeField] Rigidbody rb;
        [SerializeField] Animator animator;
        [SerializeField] CinemachineFreeLook freeLookVCam;
        [SerializeField] InputReader input;
        
        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 3f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;
        [SerializeField] float speedMultiplier=2f;
         [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] float jumpMaxHeight=2f;
        [SerializeField] GroundChecker groundChecker;
        [Header("Shooting")]
        [SerializeField] private Rig rig;
        [SerializeField] LayerMask mouseColliderLayerMask;
        [SerializeField] Transform TransformTarget; 
        [SerializeField] Transform spawnBulletTrans;
        [SerializeField] GameObject pfBulletProjectile;
        [SerializeField] GameObject pistolTeleGO;
        [Header("Sliding")]
        private bool isSliding=false;
        private Vector3 slopeSlideVelocity;
        public bool isTeleported=false;
        const float ZeroF = 0f;
        Transform mainCam;
        float currentSpeed;
        float multiplerSpeedCurrent = 1;
        float velocity;
        float jumpVelocity;
        float aimRigWeight;
        Vector3 movement;
        bool pistolTele;
        bool isAttack;
        List<Utilities.Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake() {
            Cursor.visible=true;
            Cursor.lockState=CursorLockMode.Locked;
            mainCam = Camera.main.transform;
            freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);
            
            rb.freezeRotation = true;
            jumpTimer= new CountdownTimer(jumpDuration);
            jumpCooldownTimer= new CountdownTimer(jumpCooldown);
            timers=new List<Utilities.Timer>(2) {jumpTimer,jumpCooldownTimer};
        }
        void OnEnable() {
            input.Jump += OnJump;
            input.HandPistol += OnPistolTelePort;
            input.Attack += PlayerOnAttack;
        }

        private void PlayerOnAttack(bool attack)
        {
            isAttack=attack;
        }

        private void OnPistolTelePort()
        {
            if(!pistolTele){
                pistolTele =true;
                aimRigWeight=1f;
            }
            else {
                pistolTele=false;
                aimRigWeight=0f;
            }
        }


        public float GetCurrentSpeed(){
            return currentSpeed ;
        }
        private void OnJump(bool performed)
        {
            if(performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded){
                jumpTimer.Start();
            }
            else if(!performed && jumpTimer.IsRunning){
                jumpTimer.Stop();
            }
        }

        void OnDisable() {
            input.Jump -= OnJump;
            
            input.HandPistol -= OnPistolTelePort;
        }
        void Start() {
            input.EnablePlayerActions();
            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }
            

        void Update() {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            HandelTimers();
            UpdateAnimator();
        }
        private void FixedUpdate() {
            // if(!isSliding){
           // }
            HandleMovement();
            HandleJump();
            HandelAimming();
            HandleTransAnimator();
        }
        private void SetSlopeSlideVelocity(){
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.down,out RaycastHit hitInfo,5)){
                float angle = Vector3.Angle(hitInfo.normal,Vector3.up);
                if(angle >= 45){
                    Debug.Log(slopeSlideVelocity);
                    slopeSlideVelocity= Vector3.ProjectOnPlane(new Vector3(0,jumpForce,0),hitInfo.normal);
                    return ;
                }
            }
            Debug.Log("Slope is zeoro");
            slopeSlideVelocity = Vector3.zero;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + Vector3.down * 5);
        }
        private void HandleTransAnimator(){
            rig.weight = Mathf.Lerp(rig.weight,aimRigWeight,Time.deltaTime *20f);
            if(pistolTele){
                animator.SetLayerWeight(1,Mathf.Lerp(animator.GetLayerWeight(1),1f,Time.deltaTime*10f));
                if(animator.GetLayerWeight(1) > 0.5f){
                    pistolTeleGO.SetActive(true);
                }
            }
            else {
                 animator.SetLayerWeight(0,Mathf.Lerp(animator.GetLayerWeight(1),1f,Time.deltaTime*10f));
                 if(animator.GetLayerWeight(1) < 0.5f){
                    pistolTeleGO.SetActive(false);
                }
            }
        }
        private void HandelAimming(){
                Vector3 mouseWorldPosition = Vector3.zero;
                Vector2 screenCenterPoint= new Vector2(Screen.width /2f,Screen.height/2f);
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if(Physics.Raycast(ray,out RaycastHit raycastHit,999f,mouseColliderLayerMask)){
                    TransformTarget.position=raycastHit.point;
                    mouseWorldPosition=raycastHit.point;
                }
                if(pistolTele){
                    if(isAttack){
                        Vector3 aimDir=(mouseWorldPosition -spawnBulletTrans.position).normalized;
                        GameObject GOSpwan= MyPoolManager.Instance.GetFromPool(pfBulletProjectile,MyPoolManager.Instance.transform);
                        GOSpwan.transform.position=spawnBulletTrans.position;
                        GOSpwan.transform.rotation=Quaternion.LookRotation(aimDir,Vector3.up);
                        BulletProjectile bulletRigidbody = GOSpwan.GetComponent<BulletProjectile>();
                        bulletRigidbody.SetVelocity(aimDir);
                        isAttack=false;
                    }
                }
               
            
        }
        private void HandelTimers()
        {
            foreach(var timer in timers){
                timer.Tick(Time.deltaTime);
            }
        }
        void HandleJump(){
            if(jumpTimer.IsRunning && groundChecker.IsGrounded){
                jumpVelocity=ZeroF;
                jumpTimer.Stop();
                return;
            }
            if(jumpTimer.IsRunning){
                float launchPoint=0.9f;
                if(jumpTimer.Progress > launchPoint){
                    jumpVelocity =Mathf.Sqrt(2*jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                }
                else {
                    jumpVelocity += (1-jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
                }
            }
            else {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            rb.velocity = new Vector3(rb.velocity.x,jumpVelocity,rb.velocity.z);
        }

        void UpdateAnimator() {
            animator.SetFloat(Speed, currentSpeed);
        }

        public void HandleMovement() {
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            multiplerSpeedCurrent = Input.GetKey(KeyCode.LeftShift) ? speedMultiplier : 1;
            if (adjustedDirection.magnitude > ZeroF) {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                float targetValueSmooth=multiplerSpeedCurrent/speedMultiplier;
                SmoothSpeed(adjustedDirection.magnitude* targetValueSmooth);
            } else {
                SmoothSpeed(ZeroF);
                rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
            }
        }   
        public bool isPistol(){
            return pistolTele;
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection) {
            Vector3 velocity = adjustedDirection * (moveSpeed * multiplerSpeedCurrent * Time.fixedDeltaTime);
            Debug.LogWarning(velocity);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
            Debug.Log(rb.velocity);
        }

        void HandleRotation(Vector3 adjustedDirection) {
                if(pistolTele) return ;
                var targetRotation = Quaternion.LookRotation(adjustedDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}
