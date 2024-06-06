using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    public int level = 1;
    public int health = 100;
    private bool menu = false;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private const float ACCELERATION = 12f;
    private const float AIMING_Y_OFFSET_TARGET = 80f;
    private const float AIM_PREPARE_DURATION = 4f; // second / n
    private const float AIM_WEIGHT = 0.5f;
    private const float IDLE_WEIGHT = 0.1f;
    public HealthBar healthbar;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator bowAnimator;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private AnimationCurve aimPrepareCurve;
    [SerializeField] private MultiAimConstraint[] spineBones;
    [SerializeField] private MultiAimConstraint shoulderBone;
    [SerializeField] private GameObject arrowHead;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private SoundSystem soundSystem;
    private AudioSource releaseArrow;
    private AudioSource loadArrow;
    private AudioSource redGotHit;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float groundRadius;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private GameObject blueCircleVFX;
    [SerializeField] private GameObject purpleCircleVFX;
    [SerializeField] private GameObject blueFireVFX;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private Rigidbody myRigidbody;
    public Bow bow;
    private InputManager playerInput;
    private float moveAnimationSpeedTarget;
    private float currentMoveX;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public float walkingSpeed;
    public float rotationSpeed;
    private Vector3 walkingChange;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Camera mainCamera;
    public Transform targetTransform;
    public LayerMask mouseAimMask;
    public LayerMask groundLayer;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private float targetAimDelta;
    private float weightStart;
    private float weightTarget;
    private float startAimDelta;
    private float aimDeltaTime;
    private bool isAimStateChangeing;
    private AIMING_STATE aimingState;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool hasSpawnedEffect = false;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private float drawTension;
    private float time;
    private bool builtUp;
    private bool loading;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private GameObject effectBlue;
    private GameObject effectPurple;
    private GameObject effectBlueFire;
    public float destructionTime = 5.0f;
    public float shrinkPower = 1.5f;
    private bool needsToStopBuild = false;
    private Coroutine deactivate;
    private Coroutine buildupCoroutine;
    private float interpolatedValue;
    public WaveSpawner waveSpawner;
    public List<GameObject> remainingEnemies;
    public List<GameObject> leftToSpawnEnemies;
    public int CurrentHealth;
    public FloatValue maxHealth;
    public Canvas canvas;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private enum AIMING_STATE
    {
        IDLE,
        AIMING,
        CHARGING
    }

    private bool NeedBackwards
    {
        get
        {
            return targetTransform.position.x > transform.position.x && walkingChange.x == 1? false : 
                targetTransform.position.x < transform.position.x && walkingChange.x == -1 ? false : true;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        canvas.gameObject.SetActive(false);
        maxHealth.value = 100;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        myRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<InputManager>();
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        SetAimingState(AIMING_STATE.AIMING, true);
        mainCamera = Camera.main;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        loading = true;
        remainingEnemies = waveSpawner.spawnedEnemies;
        leftToSpawnEnemies = waveSpawner.enemiesToSpawn;
        healthbar.SetMaxHealth(100);
        if (CurrentHealth != healthbar.slider.value && CurrentHealth != 0)
        {
            
            healthbar.ApplyDamage(Convert.ToInt32(maxHealth.value - CurrentHealth));
        }
    }
   
    private void Update()
    {

        remainingEnemies = waveSpawner.spawnedEnemies;
        leftToSpawnEnemies = waveSpawner.enemiesToSpawn;
        //Debug.Log(remainingEnemies.Count);

        if (playerInput.IsButtonHeld(InputManager.PLAYER_ACTION.SHOOTING))
        {
            if (!soundSystem.LoadArrow.isPlaying && loading)
            {
                soundSystem.LoadArrow.Play();
            }
            if(playerAnimator.GetFloat("DrawTension") == 1)
            {
                loading = false;
            }
            else
            {
                loading = true;
            }
        }
        else
        {
            if(soundSystem.LoadArrow.isPlaying)
            {
                soundSystem.LoadArrow.Stop();
            }
        }
    
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        GetInput();
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    public void SetPlayerState(int _state)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        aimingState = (AIMING_STATE)_state;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    private void SetAimingState(AIMING_STATE _state = AIMING_STATE.IDLE, bool _isInstant = false)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        aimingState = _state;
        isAimStateChangeing = true;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (aimingState == AIMING_STATE.IDLE)
        {
            startAimDelta = 1;
            targetAimDelta = 0;
            weightStart = AIM_WEIGHT;
            weightTarget = IDLE_WEIGHT;
            shoulderBone.weight = 0f;
            if (_isInstant)
            {
                foreach (MultiAimConstraint constraint in spineBones)
                {
                    constraint.data.offset = new Vector3(0, 0, 0);
                    constraint.weight = IDLE_WEIGHT;
                }
                playerAnimator.SetLayerWeight(1, targetAimDelta);
                isAimStateChangeing = false;
            }
        }
        else
        {
            shoulderBone.weight = 0.3f;
            weightStart = IDLE_WEIGHT;
            weightTarget = AIM_WEIGHT;
            startAimDelta = 0;
            targetAimDelta = 1;
            if (_isInstant)
            {
                playerAnimator.SetLayerWeight(1, targetAimDelta);
               
                foreach (MultiAimConstraint constraint in spineBones)
                {
                    constraint.data.offset = new Vector3(0, AIMING_Y_OFFSET_TARGET, 0);
                    constraint.weight = AIM_WEIGHT;
                   
                }
                isAimStateChangeing = false;
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    private void UpdateAimingState()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (isAimStateChangeing)
        {
            aimDeltaTime = Mathf.Clamp01(aimDeltaTime + Time.deltaTime * AIM_PREPARE_DURATION);
            playerAnimator.SetLayerWeight(1, Mathf.Lerp(startAimDelta, targetAimDelta, aimPrepareCurve.Evaluate(aimDeltaTime)));
            float _yAxis = Mathf.Lerp(startAimDelta, targetAimDelta * AIMING_Y_OFFSET_TARGET, aimPrepareCurve.Evaluate(aimDeltaTime));
            foreach (MultiAimConstraint constraint in spineBones)
            {
                constraint.data.offset = new Vector3(0, _yAxis, 0);
                constraint.weight = Mathf.Lerp(weightStart, weightTarget, aimPrepareCurve.Evaluate(aimDeltaTime));
            }
            if (aimDeltaTime == 1)
            {
                isAimStateChangeing = false;
                aimDeltaTime = 0;
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    private void GetInput()
    {
        if (!menu)
        {
            //--------------------------------------------------------------------------------------------------------------------------------------
            playerAnimator.SetBool("IsGrounded", IsInGroundRadius());
            //--------------------------------------------------------------------------------------------------------------------------------------
            if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.JUMPING))
            {
                //Debug.Log("SOKT REIK");
                if (playerAnimator.GetBool("CanJump") && !builtUp)
                {
                    soundSystem.Jump.Play();
                    isJumping = true;
                    StartCoroutine(ResetJump(jumpDuration));
                }

            }
        }

        if (playerInput.ListenForClick(InputManager.PLAYER_ACTION.EXIT))
        {
            if (menu)
            {
                // Unload the menu scene after a short delay
                StartCoroutine(UnloadMenuSceneAfterDelay());
            }
            else
            {
                // Load the menu scene additively
                SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
                menu = true;
            }
        }




        if (!menu)
        {
            //--------------------------------------------------------------------------------------------------------------------------------------
            if (!playerAnimator.GetBool("IsJumping"))
            {
                playerAnimator.SetBool("IsFalling", !IsInGroundRadius());
            }
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            if (playerAnimator.GetBool("IsFalling"))
            {

                jumpTimer += Time.fixedDeltaTime;
                float jumpProgress = jumpTimer / jumpDuration;


                Vector2 newVelocity = Vector2.Lerp(myRigidbody.velocity, Vector2.down * jumpForce, jumpProgress);


                //Debug.Log("Final velocity: " + myRigidbody.velocity.y);
            }
            //--------------------------------------------------------------------------------------------------------------------------------------
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
            {
                targetTransform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z - 2f);
            }
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            walkingChange = playerInput.GetAxis(InputManager.AXIS.MOVE);
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            if (isAimStateChangeing)
            {
                return;
            }
            switch (aimingState)
            {

                case AIMING_STATE.IDLE:
                    return;
                case AIMING_STATE.AIMING:
                    playerAnimator.SetLayerWeight(1, 1);
                    return;
                case AIMING_STATE.CHARGING:
                    if (playerInput.IsButtonHeld(InputManager.PLAYER_ACTION.SHOOTING))
                    {
                        needsToStopBuild = false;
                        time += Time.deltaTime;

                        if (deactivate != null)
                        {
                            StopCoroutine(deactivate);
                        }

                        drawTension = Mathf.Clamp01(drawTension + Time.deltaTime);
                        playerAnimator.SetFloat("DrawTension", drawTension);
                        if (time > 1.45f && !hasSpawnedEffect)
                        {
                            builtUp = true;
                            cameraFollow.needShake = true;

                            effectBlueFire = Instantiate(blueFireVFX, arrowHead.transform);
                            effectBlue = Instantiate(blueCircleVFX, this.transform);
                            effectPurple = Instantiate(purpleCircleVFX, this.transform);
                            effectBlueFire.GetComponent<ParticleSystem>().Play();
                            PlayVFX(effectPurple, effectBlue, 2f, true);

                            hasSpawnedEffect = true;
                        }

                    }
                    else
                    {
                        if (drawTension != 0)
                        {
                            Destroy(effectBlueFire);
                            cameraFollow.needShake = false;
                            if (buildupCoroutine != null)
                            {
                                builtUp = false;
                                StopCoroutine(buildupCoroutine);

                                PlayVFX(effectPurple, effectBlue, 2f, false);

                            }
                            hasSpawnedEffect = false;

                            mainCamera.GetComponent<CameraFollow>().ShakeAfterRelease();

                            if (time >= 1.45)
                            {
                                bow.Fire(playerAnimator.GetFloat("DrawTension") * Mathf.Clamp(time - 1.45f, 1, 5f), time);
                            }
                            else
                            {
                                bow.Fire(playerAnimator.GetFloat("DrawTension"), time);
                            }

                            time = 0;
                            soundSystem.ReleaseArrow.Play();
                            drawTension = 0;
                            playerAnimator.SetFloat("DrawTension", drawTension);
                            playerAnimator.SetTrigger("Shoot");
                            SetPlayerState((int)AIMING_STATE.AIMING);

                        }
                        playerAnimator.SetFloat("DrawTension", drawTension);
                    }
                    return;

            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    private void PlayVFX(GameObject vfxGameObjectPurple, GameObject vfxGameObjectBlue, float buildupTime, bool buildUp)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ParticleSystem vfxComponentPurple = vfxGameObjectPurple.GetComponent<ParticleSystem>();
        ParticleSystem vfxComponentBlue = vfxGameObjectBlue.GetComponent<ParticleSystem>();
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (vfxComponentPurple != null && buildUp)
        {
            buildupCoroutine = StartCoroutine(BuildupEffect(vfxComponentPurple, vfxComponentBlue, buildupTime));
            // Sounding
            // AudioSource audioSource = vfxGameObject.GetComponent<AudioSource>();
            // if (audioSource != null)
            // {
            //     audioSource.Play();
            // }
        }
        else if(vfxComponentPurple != null && !buildUp)
        {
           
            deactivate = StartCoroutine(GradualDestruction(vfxGameObjectPurple, vfxGameObjectBlue));
        }
        else
        {
            Debug.LogWarning("VFX component not found in the instantiated prefab.");
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    IEnumerator BuildupEffect(ParticleSystem vfxComponentPurple, ParticleSystem vfxComponentBlue, float buildupTime)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (!needsToStopBuild)
        {
            float timer = 0f;
            float originalStartLifetimePurple = vfxComponentPurple.main.startLifetime.constant;
            float originalStartLifetimeBlue = vfxComponentBlue.main.startLifetime.constant;

            Vector3 initialScale = Vector3.one * 0.1f; // Start from a very small scale
            Vector3 finalScale = Vector3.one;

            
            while (timer < buildupTime)
            {

                timer += Time.deltaTime;

                // Calculate the interpolation factor
                float t = timer / buildupTime;

                // Adjust the scale gradually
                vfxComponentPurple.transform.localScale = Vector3.Lerp(initialScale, new Vector3(0.75f, 0.75f, .75f), t);
                vfxComponentBlue.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
                // Get the current value of the "MoveSpeed" parameter
                float currentMoveSpeed = playerAnimator.GetFloat("MoveSpeed");

                // Interpolate between the current value and 0 with factor 't'
                interpolatedValue = Mathf.Lerp(currentMoveSpeed, 0, t);

                // Set the interpolated value back to the "MoveSpeed" parameter
                playerAnimator.SetFloat("MoveSpeed", interpolatedValue);

                // Calculate the interpolation factor for start lifetime
                float lifetimeT = Mathf.Pow(t, 2); // You can adjust the power for a different scaling effect

                // Adjust the start lifetime gradually
                var mainModulePurple = vfxComponentPurple.main;
                var mainModuleBlue = vfxComponentBlue.main;
                mainModulePurple.startLifetime = Mathf.Lerp(0f, originalStartLifetimePurple, lifetimeT);
                mainModuleBlue.startLifetime = Mathf.Lerp(0f, originalStartLifetimeBlue, lifetimeT);
                yield return null;
            }
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Ensure the VFX reaches its maximum effect
            vfxComponentPurple.transform.localScale = new Vector3(0.75f, 0.75f, .75f);
            vfxComponentBlue.transform.localScale = finalScale;
            // Reset the start lifetime to its original value
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            var finalMainModulePurple = vfxComponentPurple.main;
            var finalMainModuleBlue = vfxComponentBlue.main;
            finalMainModulePurple.startLifetime = originalStartLifetimePurple;
            finalMainModuleBlue.startLifetime = originalStartLifetimeBlue;
            // Play the VFX after the buildup
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------
            vfxComponentPurple.Play();
            vfxComponentBlue.Play();
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------


        }
    }


    IEnumerator GradualDestruction(GameObject vfxGameObjectPurple, GameObject vfxGameObjectBlue)
    {

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        float timer = 0f;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        while (timer < destructionTime)
        {
            timer += Time.deltaTime;

            float t = timer / destructionTime * shrinkPower * 2;

            // Apply a power function for a stronger effect
            float adjustedT = Mathf.Pow(t, shrinkPower);

            // Gradually scale down the GameObject
            vfxGameObjectPurple.transform.localScale = Vector3.Lerp(effectPurple.transform.localScale, Vector3.zero, adjustedT);
            vfxGameObjectBlue.transform.localScale = Vector3.Lerp(effectBlue.transform.localScale, Vector3.zero, adjustedT);
            yield return null;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Ensure the GameObject is very small before destroying
        vfxGameObjectPurple.transform.localScale = Vector3.zero;
        vfxGameObjectBlue.transform.localScale = Vector3.zero;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Destroy the GameObject after the gradual shrinking
        Destroy(vfxGameObjectPurple);
        Destroy(vfxGameObjectBlue);
        deactivate = null;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }


    private IEnumerator SetToFall()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(1 / 2f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        myRigidbody.mass = 1000f;
        playerAnimator.SetBool("IsJumping", false);
        myRigidbody.velocity = Vector2.zero;
        playerAnimator.SetBool("IsFalling", true);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
    private IEnumerator ResetJump(float jumpDuration)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(jumpDuration + 1.45f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        playerAnimator.SetBool("CanJump", true);
        jumpTimer = 0f;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    private void FixedUpdate()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        UpdateMovement();
        UpdateRotation();
        UpdateAimingState();
        UpdateJumpingCycle();
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    private void UpdateJumpingCycle()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (isJumping)
        {
            
            jumpTimer += Time.fixedDeltaTime;
            float jumpProgress = jumpTimer / jumpDuration;

            // Interpolate between the current velocity and the desired jump velocity
            Vector2 newVelocity = Vector2.Lerp(myRigidbody.velocity, Vector2.up * jumpForce, jumpProgress);

            // Apply the new velocity to the rigidbody
            myRigidbody.velocity = newVelocity;

            playerAnimator.SetBool("IsJumping", true);
            //playerAnimator.SetBool("IsGrounded", false);
            myRigidbody.mass = 1.0f;
            StartCoroutine(SetToFall());
            playerAnimator.SetBool("CanJump", false);
            // Check if the jump duration is complete
            if (jumpTimer >= jumpDuration)
            {
                isJumping = false;
                
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
    private void UpdateMovement()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Vector3 movement;
        playerAnimator.SetFloat("MoveSpeed", 0);
        currentMoveX = Mathf.Lerp(currentMoveX, walkingChange.x, Time.deltaTime * ACCELERATION);
        moveAnimationSpeedTarget = Mathf.Lerp(moveAnimationSpeedTarget, walkingChange.magnitude, Time.deltaTime * (ACCELERATION / 2));

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (!builtUp)
        {
            if (NeedBackwards)
            {

                playerAnimator.SetFloat("MoveSpeed", moveAnimationSpeedTarget * (-1));
                movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed / 2 * Time.fixedDeltaTime * (builtUp ? 0 : 1);
            }
            else
            {
                playerAnimator.SetFloat("MoveSpeed", moveAnimationSpeedTarget);
                movement = new Vector3(currentMoveX, 0, 0) * walkingSpeed * Time.fixedDeltaTime * (builtUp ? 0 : 1);
            }
        }
        else
        {
            playerAnimator.SetFloat("MoveSpeed", interpolatedValue);
            movement = Vector3.zero;
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        myRigidbody.MovePosition(myRigidbody.position + movement);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if(other.gameObject.name == "TailEnd")
        {
            healthbar.ApplyDamage(10);
            other.gameObject.SetActive(false);
            StartCoroutine(Activate(other.gameObject));
            CurrentHealth = Convert.ToInt32(healthbar.slider.value);
            if (CurrentHealth == 0)
            {
                canvas.gameObject.SetActive(true);
            }
        }
        else if(other.gameObject.name == "R_Hand")
        {
            healthbar.ApplyDamage(20);
            other.gameObject.SetActive(false);
            StartCoroutine(Activate(other.gameObject));
            CurrentHealth = Convert.ToInt32(healthbar.slider.value);
            if (CurrentHealth == 0)
            {
                canvas.gameObject.SetActive(true);
            }
        }



    }
    private IEnumerator Activate(GameObject other)
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        yield return new WaitForSeconds(1 / 2f);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        other.gameObject.SetActive(true);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
    private void UpdateRotation()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Quaternion target = Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x)));
        myRigidbody.rotation = Quaternion.Lerp(myRigidbody.rotation, target, Time.deltaTime * rotationSpeed);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    private bool IsInGroundRadius()
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        bool isInRadius = false;
        // Check for GROUND objects within the specified radius
        Collider[] colliders = Physics.OverlapSphere(myRigidbody.position, groundRadius, groundLayer);
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Iterate through the colliders to see if any of them have the "GROUND" tag
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Ground"))
            {
               
                // The "GROUND" object is within the specified radius
                isInRadius = true;
                // You can perform additional actions here if needed
            }
        }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        return isInRadius;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
    IEnumerator UnloadMenuSceneAfterDelay()
    {
        yield return null; // Wait for one frame to ensure the menu scene is fully loaded

        // Unload the menu scene
        SceneManager.UnloadSceneAsync("Menu");
        menu = false;
    }
    




}
