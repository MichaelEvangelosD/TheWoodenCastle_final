using UnityEngine;

public class CoinDrop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.S.OnCoinPickup();
    }
}
