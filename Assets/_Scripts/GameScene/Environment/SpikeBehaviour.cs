using System.Collections;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField, Range(0, 0.1f)] float timeBetweenHits; //How long should we wait until the next hit

    BoxCollider2D spikeCollider; //The spike trigger area
    bool triggered = false;

    private void Start()
    {

        if ((spikeCollider = GetComponent<BoxCollider2D>()) != true)
        { Utils.PrintMissingComponentMsg("BoxCollider2D component", this); }

        //Set trigger default state
        triggered = false;
        if (!spikeCollider.isTrigger) spikeCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;

            StartCoroutine(InstaKillPlayer());
        }
    }

    /// <summary>
    /// Call to instantly hit the player with 3 damage
    /// </summary>
    IEnumerator InstaKillPlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerBehaviour.S.PlayerHealth.DecreaseHealth();
            yield return new WaitForSeconds(timeBetweenHits);
        }

        yield return null;
    }
}
