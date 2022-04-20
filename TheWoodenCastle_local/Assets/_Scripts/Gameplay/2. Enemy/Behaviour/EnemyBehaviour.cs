using UnityEngine;

/// <summary>
/// All the states the enemy can exist in, Idle is the default state
/// </summary>
public enum EnemyState
{
    Idle,
    BattleReady,
    Attacking,
    Hurt,
    Dead,
}

[RequireComponent(typeof(Animator), typeof(BoxCollider2D))]
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] float visionDistance = 2; //How far can the enemy detect the player and enter BattleReady state
    [Range(0f, 0.75f), SerializeField] float attackDistance; //In what range the enemy can attack the player (from his pivot center)
    [SerializeField] LayerMask playerLayer; //In what layer to detect physics collisions

    #region BEHAVIOUR_VARIABLES
    Animator enemyAnimator;
    Collider2D boxCollider;

    float attackDistanceOffset = 0.1f; //Apply a slight distance offset so the enemy can't attack the player behind him
    RaycastHit2D hit, backHit; //Hit info from physics collisions
    Vector2 facingTo = Vector2.zero; //Dynamically changes based on the enemy's Y rotation
    bool flipped = false;

    bool canChangeState = true;
    EnemyState currentState;
    /// <summary>
    /// When SET it automatically calles OnStateChange()
    /// </summary>
    public EnemyState CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState == value || canChangeState == false)
                return;

            currentState = value;

            OnStateChange();
        }
    }
    #endregion

    #region BEHAVIOUR_CACHING
    private EnemyAttack _enemyAttack;
    public EnemyAttack EnemyAttack
    {
        get { return _enemyAttack; }
        private set
        {
            _enemyAttack = value;
        }
    }

    private EnemyHealth _enemyHealth;
    public EnemyHealth EnemyHealth
    {
        get { return _enemyHealth; }
        private set
        {
            _enemyHealth = value;
        }
    }

    private DamageDisplay _damageDisplay;
    public DamageDisplay DamageDisplay
    {
        get { return _damageDisplay; }
        private set
        {
            _damageDisplay = value;
        }
    }

    private EnemyAudio enemyAudio;
    #endregion

    #region STARTUP_SETUP
    private void Start()
    {
        CacheReferences();

        //Set enemy's default rotation
        DetermineRotation();
    }

    /// <summary>
    /// Cache the enemy behaviour references
    /// </summary>
    void CacheReferences()
    {
        enemyAnimator = GetComponent<Animator>();
        boxCollider = GetComponent<Collider2D>();

        _enemyAttack = GetComponent<EnemyAttack>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _damageDisplay = GetComponent<DamageDisplay>();
        enemyAudio = GetComponent<EnemyAudio>();
    }
    #endregion

    private void Update()
    {
        //If the enemy is dead, do nothing
        if (CurrentState == EnemyState.Dead)
        {
            boxCollider.isTrigger = true;
            return;
        }

        //Check for collisions of back and front of the enemy gameObject
        BackDetection();
        FrontDetection();

        //If we hit the player
        if (hit && hit.transform.CompareTag("Player"))
        {
            //Change to BattleReady state
            CurrentState = EnemyState.BattleReady;

            //And then if the player comes in attackDistance
            if (hit.distance <= attackDistance + attackDistanceOffset)
            {
                //Change to Attacking state
                CurrentState = EnemyState.Attacking;
                _enemyAttack.inAttackState = true;
            }
            else
            {
                _enemyAttack.inAttackState = false;
            }
        }

        //If we hit nothing, change to Idle state
        if (!hit)
        {
            CurrentState = EnemyState.Idle;
        }
    }

    #region PLAYER_DETECTION
    /// <summary>
    /// Create an invisible box in the facing direction of the enemy
    /// based on the vision distance
    /// and check if a collider is overlaping any point of its space.
    /// Detects collisions only in the player layer mask.
    /// </summary>
    void FrontDetection()
    {
        hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size / 2, 0f, facingTo, visionDistance, playerLayer);
    }

    /// <summary>
    /// Create an invisible box in the back direction of the enemy
    /// based on the vision distance
    /// and check if a collider is overlaping any point of its space.
    /// Detects collisions only in the player layer mask 
    /// </summary>
    void BackDetection()
    {
        backHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size / 2, 0f, -facingTo, visionDistance, playerLayer);
        if (backHit)
        {
            FlipEnemy();
            DetermineRotation();
        }
    }
    #endregion

    /// <summary>
    /// Called every time the enemy currentState is changed to a new state
    /// </summary>
    void OnStateChange()
    {
        switch (currentState)
        {
            case EnemyState.BattleReady: //Spotted the player
                enemyAnimator.SetBool("isBattleReady", true);
                PlayerBehaviour.S.PlayerAttack.SetMeAsTarget(this);
                break;
            case EnemyState.Attacking: //Can attack
                _enemyAttack.InitiateAttack();
                break;
            case EnemyState.Hurt: //Hurt from the player
                enemyAnimator.Play("Hurt");
                break;
            case EnemyState.Dead: //Enemy is dead
                //Play the death sound clip of the enemy
                enemyAudio.PlayDeathSound();

                //Notify the subs of this event that an enemy has been killed
                GameEvents.S.OnEnemyDeath();

                //When the enemy enters the Death state, forcefully play the Hurt animation
                //one last time to visualize the last hit
                enemyAnimator.Play("Hurt");

                //Stop the enemy from being able to change states
                //and Destroy his gameObject after 2secs
                enemyAnimator.SetBool("isDead", true);
                canChangeState = false;
                Destroy(gameObject, 2f);
                break;
            default:
                //Reset to Idle 
                CurrentState = EnemyState.Idle;
                PlayerBehaviour.S.PlayerAttack.SetMeAsTarget(null);
                enemyAnimator.SetBool("isBattleReady", false);
                break;
        }
    }

    #region UTILITIES
    /// <summary>
    /// Flip the enemy gameObject to face the opposite side of his current facing
    /// </summary>
    void FlipEnemy()
    {
        if (!flipped)
        {
            //Look right
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            //Look left
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    /// <summary>
    /// Automatically determine the facing of the enemy based on his rotation.y value
    /// </summary>
    void DetermineRotation()
    {
        if (transform.rotation.y > 0 || transform.rotation.y < 0)
        {
            facingTo = Vector2.left;
            flipped = false;
        }
        else
        {
            facingTo = Vector2.right;
            flipped = true;
        }
    }
    #endregion
}
