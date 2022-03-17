using UnityEngine;

/* CLASS DOCUMENTATION *\
 * 
 * [Variable Specifics]
 * Inspector values: enemyHealth MUST be set from the inspector
 * Dynamically changed: The reference variables are cached and changed throughout the game
 * 
 * [Class Flow]
 * 1. This script is used solely for the purpose of storing and depleting THIS enemys' health.
 * 2. The Health property can only be set through the DecreaseHealth(...) method inside this class.
 *  2a. Health value can be retrieved through the Health property
 * 
 * [Must Know]
 * 1. If _thisEnemyHealth is set BELOW or EQUAL to 0 it automatically sets THIS enemys' state to 
 *      EnemyState.Dead and isAlive to false;
 */

public class EnemyHealth : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] float startingHealth = 5; //How many hits the enemy can withstand

    //Reference variables
    EnemyBehaviour enemyBehaviour; //THIS enemys' behaviour
    [HideInInspector] public bool isAlive = true;

    private float _thisEnemyHealth;
    public float Health
    {
        get { return _thisEnemyHealth; }
        private set
        {
            _thisEnemyHealth = value;

            if (_thisEnemyHealth <= 0)
            {
                isAlive = false;
                enemyBehaviour.CurrentState = EnemyState.Dead;
            }
        }
    }

    private void Start()
    {
        //Cache the EnemyBehaviour script of THIS enemy instance
        enemyBehaviour = GetComponent<EnemyBehaviour>();

        Health = startingHealth;
        isAlive = true;
    }

    /// <summary>
    /// Call to decrease THIS enemys' health by the given value
    /// <para>For external use</para>
    /// </summary>
    /// <param name="value">Damage value</param>
    public void DecreaseHealth(float value)
    {
        Health -= value;
    }
}
