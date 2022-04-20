using UnityEngine;

/// <summary>
/// The available layers the player can exist on
/// </summary>
public enum CharacterLayers
{
    Player = 6, //The default player layer
    EvasionLayer = 11, //Move here to make to make the player Evade
    Untargetable = 31, //Set active when the player dies
}

/// <summary>
/// Current facing of the Player
/// </summary>
public enum CharacterFacing
{
    Left,
    Right,
}

[DefaultExecutionOrder(420)]
public class PlayerBehaviour : MonoBehaviour
{
    //PseudoSingleton
    public static PlayerBehaviour S;

    [Header("Set in inspector")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] CharacterLayers startingLayer;

    #region BEHAVIOUR_VARIABLES
    //Movement and action variables
    Rigidbody2D playerRB;
    Vector2 moveDir;
    float horizontalInput;
    float inputTempFloat;

    bool isAlive = true;
    bool canFlip;
    bool canJump;
    bool canAttack;
    #endregion

    #region BEHAVIOUR_CACHING
    private bool _playerActive;
    public bool PlayerActive
    {
        get { return _playerActive; }
        set { _playerActive = value; }
    }

    private PlayerAnimations _playerAnimation;
    public PlayerAnimations PlayerAnimation
    {
        get { return _playerAnimation; }
        private set { _playerAnimation = value; }
    }

    private PlayerSprite _playerSprite;
    public PlayerSprite PlayerSprite
    {
        get { return _playerSprite; }
        private set { _playerSprite = value; }
    }

    private PlayerAttack _playerAttack;
    public PlayerAttack PlayerAttack
    {
        get { return _playerAttack; }
        private set { _playerAttack = value; }
    }

    private PlayerHealth _playerHealth;
    public PlayerHealth PlayerHealth
    {
        get { return _playerHealth; }
        private set { _playerHealth = value; }
    }

    private PlayerEvasion _playerEvasion;
    public PlayerEvasion PlayerEvasion
    {
        get { return _playerEvasion; }
        private set { _playerEvasion = value; }
    }

    private PlayerAudio _playerAudio;
    public PlayerAudio PlayerAudio
    {
        get { return _playerAudio; }
        private set { _playerAudio = value; }
    }
    #endregion

    #region STARTUP_SETUP
    private void Awake()
    {
        S = this; //Set the PseudoSingleton

        GetComponentReferences();

        GameEvents.S.onGameSceneChanged += PlayEntranceAnimation;

        GetBehaviourReferences();
    }

    /// <summary>
    /// Cache the needed component references
    /// </summary>
    void GetComponentReferences()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Call to cache the player behaviour scripts at scene awake
    /// </summary>
    void GetBehaviourReferences()
    {
        PlayerAnimation = GetComponent<PlayerAnimations>();
        PlayerSprite = GetComponent<PlayerSprite>();
        PlayerAttack = GetComponent<PlayerAttack>();
        PlayerHealth = GetComponent<PlayerHealth>();
        PlayerEvasion = GetComponent<PlayerEvasion>();
        PlayerAudio = GetComponent<PlayerAudio>();
    }

    void Start()
    {
        //Set the default startup behaviour of the Player gameObject
        canFlip = canJump = canAttack = isAlive = true;

        SetActiveLayer(startingLayer);
    }
    #endregion

    #region BEHAVIOUR HANDLING
    void Update()
    {
        if (!PlayerActive)
        {
            PlayerAnimation.SetWalkingAnimation(false);
            PlayerAnimation.SetJumpingAnimation(false);

            return;
        }

        //If the player health falls below 0 and he is still alive - isAlive is used like a flag
        if (PlayerHealth.IsDead() && isAlive)
        {
            //Play the player death aucio clip
            PlayerAudio.PlayAudio(PlayerAudioClips.Death, false);

            isAlive = false; //Set the flag boolean to false so we don't enter this if statement again

            //Move him to Untargetable layerMask
            SetActiveLayer(CharacterLayers.Untargetable);

            //Play his death animation
            PlayerAnimation.PlayDeathAnimation();

            //...and lastly notify the subscribers that the player is dead
            GameEvents.S.OnPlayerDeath();
        }

        //If the player is set to active state and his alive he can move, jump and attack
        if (isAlive)
        {
            HandleJump(); //We jump in update so we don't skip any input

            GetMovementInput(); //Get the movement input

            HandleAttack();
        }
    }

    /// <summary>
    /// Get the horizontal axis and cache it to inputTempFloat,
    /// <para>if it's greater than 0 set it to 1, if it's smaller than 0 set it to -1</para>
    /// Defaults to 0
    /// </summary>
    void GetMovementInput()
    {
        horizontalInput = (inputTempFloat = Input.GetAxisRaw("Horizontal")) > 0 ? 1 : inputTempFloat < 0 ? -1 : 0;
    }

    //Set movement input 
    private void FixedUpdate()
    {
        //Execute only IF the player is still alive and active
        if (PlayerActive && isAlive)
        {
            HandleMovement();
        }
    }

    /// <summary>
    /// Changes players velocity based on user input
    /// <para>Also it controls the Sprite and animator states based on current input</para>
    /// </summary>
    void HandleMovement()
    {
        //If the X-axis input is negative, move left
        if (horizontalInput < 0)
        {
            //Executes only once, just like Input.GetKeyDown()
            EvadeGetKeyDown(CharacterFacing.Right);

            //Apply the input value to the players' velocity
            SetAndApplyVelocityX(horizontalInput * moveSpeed * Time.deltaTime);

            //Flip the player
            CheckIfFlip(CharacterFacing.Left);

            //Play the corresponding animation inside the player animator
            PlayerAnimation.SetWalkingAnimation(true);
        }
        else
        {
            //If the X-axis input is positive then move right
            if (horizontalInput > 0)
            {
                //Executes only once, just like Input.GetKeyDown()
                EvadeGetKeyDown(CharacterFacing.Left);

                //Apply the input value to the players' velocity
                SetAndApplyVelocityX(horizontalInput * moveSpeed * Time.deltaTime);

                //Flip the player
                CheckIfFlip(CharacterFacing.Right);

                PlayerAnimation.SetWalkingAnimation(true);
            }
            else //If we press nothing, just stand still
            {
                //Stop the walking animation...
                PlayerAnimation.SetWalkingAnimation(false);

                //... and stop the player from moving
                SetAndApplyVelocityX(0f);
            }
        }
    }

    /// <summary>
    /// Call to start the evading sequence only if the current facing of the player is equal
    /// to the given parameter.
    /// <para>Executes once per direction change.</para>
    /// </summary>
    void EvadeGetKeyDown(CharacterFacing directionFacing)
    {
        if (PlayerSprite.CurrentFacing == directionFacing)
        {
            HandleEvasion();
        }
    }

    /// <summary>
    /// Call to set the vector which then gets applied to the playerRBs' velocity.
    /// <para>Always sets the Y velocity to playerRB.velocity.y</para>
    /// </summary>
    void SetAndApplyVelocityX(float xVelocityValue)
    {
        moveDir.Set(xVelocityValue, playerRB.velocity.y);
        playerRB.velocity = moveDir;
    }

    /// <summary>
    /// Call to rotate the player sprite only if can flip is set to true
    /// </summary>
    void CheckIfFlip(CharacterFacing flipTo)
    {
        if (canFlip)
        {
            PlayerSprite.FlipCharacter(flipTo);
        }
    }

    /// <summary>
    /// Call to apply a 1*jumpForce to Y velocity of the player.
    /// <para>Then set isJumping bool to true in the Player Animator</para>
    /// </summary>
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            canJump = false;
            playerRB.velocity = Vector2.up * jumpForce;

            PlayerAnimation.SetJumpingAnimation(true);

            //Evade for this jump
            HandleEvasion(true);
        }
    }

    /// <summary>
    /// When the player touches ground reset his jump and animation states
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Environment") || collision.CompareTag("Destructable"))
        {
            PlayerAnimation.SetJumpingAnimation(false);

            canJump = true;
        }
    }

    /// <summary>
    /// Whenever the player starts moving or jumping make the character evade any attack
    /// </summary>
    /// <param name="jumpEvade">If jumpEvade is true then we called this method for a jump</param>
    void HandleEvasion(bool jumpEvade = false)
    {
        if (jumpEvade && !PlayerEvasion.justEvaded)
        {
            PlayerEvasion.StartEvade();
        }
        else if (!PlayerEvasion.justEvaded)
        {
            PlayerEvasion.StartEvade();
        }
    }

    /// <summary>
    /// When Fire1 gets pressed and the player can attack
    /// set (canAttack, canMove) => false and play the BasicAttack animation
    /// </summary>
    void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1") && canAttack)
        {
            canAttack = false;
            canFlip = false;
            moveSpeed /= 2f;

            PlayerAnimation.PlayAttackAnimation();
            PlayerAudio.PlayAudio(PlayerAudioClips.Attack, true);
        }
    }

    //Falling handling
    private void LateUpdate()
    {
        //When the player starts falling
        if (playerRB.velocity.y < 0)
        {
            ApplyRealisticFalling();
        }
    }

    /// <summary>
    /// Call to apply a small pull downwards on the players velocity
    /// </summary>
    void ApplyRealisticFalling()
    {
        playerRB.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
    }
    #endregion

    #region UTILITIES
    /// <summary>
    /// Called from BasicAttack animation event to set flipping, attacking to true 
    /// and players' moveSpeed back to the inspector value
    /// </summary>
    public void ResetAttack()
    {
        canFlip = true;
        canAttack = true;
        moveSpeed *= 2;
    }

    /// <summary>
    /// Call to set the active collision layer of the player
    /// </summary>
    /// <param name="layer">The layer to replace the current active layer</param>
    public void SetActiveLayer(CharacterLayers layer)
    {
        gameObject.layer = (int)layer;
    }

    /// <summary>
    /// Call to forcefully play the player entrance animation.
    /// <para>Useable only in the tutorial game state</para>
    /// </summary>
    public void PlayEntranceAnimation(GameScenes state)
    {
        if (state == GameScenes.InTutorialScene || state == GameScenes.InWorstTutorial)
        {
            PlayerAnimation.PlayEntranceAnimation();
        }
        else
        {
            ActivatePlayer();
        }
    }

    /// <summary>
    /// Called from the end of TutorialEntrance animation event 
    /// OR from PlayEntranceAnimation() method when NOT in the Tutorial level
    /// </summary>
    public void ActivatePlayer()
    {
        //Unity bug, re-activate rootMotion so we can move
        PlayerAnimation.PlayerAnimator.applyRootMotion = true;

        //Transit to idle animation
        PlayerAnimation.SetActiveBool(true);

        //Activate the players rigidbody so we can interact with physics
        playerRB.isKinematic = false;

        //Activate the player behaviour
        PlayerActive = true;

        //Enable the pause menu
        GameEvents.S.OnPauseMenuStateChange(true);
    }
    #endregion

    private void OnDestroy()
    {
        S = null;

        //Unsub from game events
        GameEvents.S.onGameSceneChanged -= PlayEntranceAnimation;
    }
}