using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateWall : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] BoxCollider2D invisibleWall; //The invisible wall to deactivate

    bool isTriggered = false;

    private void Awake()
    {
        //Check if the invisibleWall variable is empty
        if (invisibleWall != null)
        {
            invisibleWall.isTrigger = false;
        }
        else { Utils.PrintMissingComponentMsg("Invisible wall", this); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            if (invisibleWall != null) invisibleWall.isTrigger = true;

            isTriggered = true;
        }
    }
}
