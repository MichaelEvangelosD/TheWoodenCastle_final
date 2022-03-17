using UnityEngine;

interface IPlayerInteractable
{
    void CacheReferences();
    void SetTriggerDefaults();

    void OnTriggerEnter2D(Collider2D collision);
    void OnTriggerExit2D(Collider2D collision);
}
