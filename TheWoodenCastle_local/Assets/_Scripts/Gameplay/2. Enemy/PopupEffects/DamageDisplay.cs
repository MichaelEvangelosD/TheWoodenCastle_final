using System.Collections;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] Transform damageAnchor; //Where to instantiate the DamagePopup
    [SerializeField] Transform missAnchor; //Where to instantiate the missed damage prefab
    [SerializeField] Transform blockAnchor; //Where to instantiate the blocked particle effect prefab
    [SerializeField] GameObject damagePopupPrefab; //The DamagePopup prefab
    [SerializeField] GameObject missDamagePrefab; //The missed damage prefab
    [SerializeField] GameObject blockParticlePrefab; //Attacked blocked prefab

    EnemyBehaviour enemyBehaviour; //This enemys EB script

    private void Start()
    {
        //Get this enemyBehaviour attached script
        enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    /// <summary>
    /// Set CurrentState of the enemy to Hurt and Display a DamagePopup above his head
    /// </summary>
    public void ShowDamage()
    {
        enemyBehaviour.CurrentState = EnemyState.Hurt;

        StartCoroutine(DisplayPopup(damageAnchor, damagePopupPrefab, PlayerBehaviour.S.PlayerAttack.PlayerDMG));
    }

    /// <summary>
    /// Display a miss Popup in missAnchor position
    /// </summary>
    public void ShowMiss()
    {
        StartCoroutine(DisplayPopup(missAnchor, missDamagePrefab));
    }

    /// <summary>
    /// Display a small flash effect indicating a sword-block from the enemy
    /// </summary>
    public void ShowBlock()
    {
        StartCoroutine(DisplayPopup(blockAnchor, blockParticlePrefab));
    }

    /// <summary>
    /// Instantiates a popupPrefab at the anchor child gameObject position
    /// </summary>
    /// <param name="dmgValue">If not 0 then we are instantiating a Damage Prefab</param>
    IEnumerator DisplayPopup(Transform anchor, GameObject popup, float dmgValue = 0)
    {
        //Instantiate the popup prefab at the Anchor position
        GameObject tempPopup = Instantiate(popup, anchor);

        yield return new WaitForEndOfFrame();

        //If dmgValue is not 0 then we instatiated a Damage prefab
        if (dmgValue != 0)
        {
            //Update the text mesh to display correct player damage
            tempPopup.GetComponent<TextMeshPro>().text = dmgValue.ToString();
        }

        yield return new WaitForEndOfFrame();

        //Use the transform of the prefab to ensure correct rotation in the world
        tempPopup.transform.rotation = popup.transform.rotation;
    }
}
