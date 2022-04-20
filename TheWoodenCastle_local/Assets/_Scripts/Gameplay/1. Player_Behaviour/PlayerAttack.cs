using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField, Tooltip("You can selecet multiple layers")]
    LayerMask detectionLayers; //In what layers should we detect collisions on
    [SerializeField, Range(0.01f, 1f)] float blockChance; //The chance of current hit being a block

    [Header("Set dynamically")]
    [SerializeField] float playerDamage = 1; //How much damage does the player give to enemies

    /// <summary>
    /// Sets playerDamage to given value
    /// <para>Writes the damage value to PlayerPrefs with key: playerDMG</para>
    /// </summary>
    public float PlayerDMG
    {
        get { return playerDamage; }
        private set
        {
            playerDamage = value;
            PlayerPrefs.SetFloat("playerDMG", value);
        }
    }

    /// <summary>
    /// <seealso cref="SetMeAsTarget(EnemyBehaviour)">Click here</seealso>
    /// </summary>
    EnemyBehaviour attackedEB; //Currently active enemy that receives damage

    Transform attackPoint; //Center of the overlap sphere
    Collider2D hit; //Hit info variable
    float pointRadius = 0.4f; //The radious of the Circle collider in which the enemies get detected
    bool isBlock; //If true the current hit is a blocked

    private void Awake()
    {
        //Set default state
        isBlock = false;
    }

    private void Start()
    {
        //Get the transform of the attack point
        attackPoint = transform.GetChild(0);

        //Set player damage to 1
        playerDamage = 1;
    }

    #region ATTACK_BEHAVIOUR
    /// <summary>
    /// Gets called from an animation event in Player_Attack animation and enables the circle attack point in front of the player
    /// </summary>
    public void EnableAttackPoint()
    {
        //Store the first objects info in hit
        hit = Physics2D.OverlapCircle(attackPoint.position, pointRadius, detectionLayers);

        //Below code executes only if we hit something that's inside the detectionLayers
        if (!hit)
        { return; }

        //If we've hit an enemy...
        if (hit.CompareTag("Enemy"))
        {
            //Execute only if an enemy is set as a target
            if (attackedEB == null) return;

            //Attack the targeted Enemy Behaviour
            ExecuteAttack();
        }
        else if (hit.CompareTag("Destructable")) //If we've hit a pot
        {
            hit.GetComponent<PotVisuals>().DestroyPot();
        }
        else { return; }
    }
    #endregion

    #region ATTACK_TYPES
    void ExecuteAttack()
    {
        //Below code executes only if the enemy is alive
        if (!attackedEB.EnemyHealth.isAlive)
            return;

        IsBlock(); //Determine if the attack will be a block or not

        //If it is not a block
        if (!isBlock)
        {
            //Decrease active EB health point
            DecreaseEnemyHP();

            //And show the damage on the enemy
            attackedEB.DamageDisplay.ShowDamage();
        }
        else //if it a block
        {
            //Show a small flash on the enemy sword
            attackedEB.DamageDisplay.ShowBlock();

            //Vibrate the controller - executes only on windows builds
#if UNITY_STANDALONE_WIN
            GameEvents.S.OnVibrateCall();
#endif
            //Distort the camera lens
            LensDistorter.S.DistortLens();

            //...and play the *ching* sound clip
            PlayerBehaviour.S.PlayerAudio.PlayAudio(PlayerAudioClips.SwordBlock, true);
        }
    }

    int consecutiveBlocks = 0;
    /// <summary>
    /// Randomize a block chance with a dice, if diceValue <= (blockChance - 0.01f) then isBlocked equals true
    /// </summary>
    void IsBlock()
    {
        System.Random randomizer = new System.Random((int)DateTime.Now.Ticks);

        double diceValue = randomizer.NextDouble();

        if (diceValue <= (blockChance - 0.01f))
        {
            if (consecutiveBlocks > 2)
            {
                consecutiveBlocks = 0;
                isBlock = false;
                return;
            }

            consecutiveBlocks++;
            isBlock = true;

        }
        else
        {
            isBlock = false;
        }
    }

    /// <summary>
    /// Decrease attacked enemy HP by PLAYER_DMG 
    /// </summary>
    void DecreaseEnemyHP()
    {
        attackedEB.EnemyHealth.DecreaseHealth(playerDamage);
    }
    #endregion

    #region UTILITIES
    /// <summary>
    /// Called from an enemy when he enters BattleReady state
    /// </summary>
    /// <param name="myEB">The detected enemy's EnemyBehaviour reference</param>
    public void SetMeAsTarget(EnemyBehaviour myEB)
    {
        attackedEB = myEB;
    }

    /// <summary>
    /// Call to increase PlayerDMG by the given value
    /// </summary>
    /// <param name="resetDmg">Set to true if you want to zero the PlayerDMG before increasing it</param>
    public void IncreasePlayerDamage(float damageIncrease, bool resetDmg = false)
    {
        if (resetDmg) PlayerDMG = 0;

        PlayerDMG += damageIncrease;
    }
    #endregion
}
