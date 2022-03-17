using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    EnemyBehaviour enemyBehaviour;

    [Header("Set in inspector")]
    [Range(0f, 1f), SerializeField] float attackPointRadius; //The radious of the damage point of the enemy
    [SerializeField] LayerMask detectionLayers; //In which layer(s) to detect collisions in
    [SerializeField] float attackCooldown;

    [HideInInspector] public bool inAttackState = false;
    Transform attackPoint;
    Collider2D playerHit;
    Animator enemyAnimator;
    float attackTimer;
    bool canAttack;

    private void Start()
    {
        CacheScriptReferences();

        inAttackState = false;

        ResetTimer();
    }

    /// <summary>
    /// Cache the required enemy script references
    /// </summary>
    void CacheScriptReferences()
    {
        //Cache the component references
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        enemyAnimator = GetComponent<Animator>();
        attackPoint = transform.GetChild(2);
    }

    private void Update()
    {
        if (!inAttackState)
            return;

        //When the timer is zero or below the attack cooldown has passed
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0.0f)
        {
            canAttack = true;
        }
    }

    /// <summary>
    /// Initiates the attack sequence of the enemy
    /// if enemy canAttack = true based on the attackCooldown
    /// </summary>
    public void InitiateAttack()
    {
        if (!canAttack)
            return;

        //Create an invisible sphere and check if a collider is overlaping any point of its space at attackPoint cordinates that detects collisions only in the player layer mask
        playerHit = Physics2D.OverlapCircle(attackPoint.position, attackPointRadius, detectionLayers);

        if (playerHit != null)
        {
            //If the player is in the Evasion Layer - display miss 
            if (playerHit.gameObject.layer == (int)CharacterLayers.EvasionLayer)
            {
                enemyBehaviour.DamageDisplay.ShowMiss();

                PlayAttackAnimation();
                ResetTimer();
            }
            else // Else damage him
            {
                //Access the PlayerHealth script from inside PlayerBehaviour and decrease his health by 1
                PlayerBehaviour.S.PlayerHealth.DecreaseHealth();
                PlayerBehaviour.S.PlayerHealth.ShowDamage();

                PlayAttackAnimation();
                ResetTimer();
            }
        }
    }

    /// <summary>
    /// Play the attack animation of the enemy and set canAttack to false
    /// </summary>
    void PlayAttackAnimation()
    {
        enemyAnimator.Play("Attack");
        canAttack = false;
    }

    /// <summary>
    /// Sets the value of attack timer to a randomized number between 1 and attackCooldown
    /// </summary>
    void ResetTimer()
    {
        attackTimer = GenerateRandomAttackTime();
    }

    /// <summary>
    /// Generates a random value between 0 and attackCoolown
    /// </summary>
    /// <returns>Random value between 0 and attackCooldown</returns>
    float GenerateRandomAttackTime()
    {
        System.Random randomizer = new System.Random((int)DateTime.Now.Ticks);
        float tempFloat = attackCooldown * ((float)randomizer.NextDouble());

        if (tempFloat <= 0.4f)
        {
            tempFloat = 0.5f;
        }

        return tempFloat;
    }
}
