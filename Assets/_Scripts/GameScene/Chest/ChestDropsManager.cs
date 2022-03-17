using System.Collections.Generic;
using UnityEngine;

public class ChestDropsManager : MonoBehaviour
{
    public static ChestDropsManager S;

    //This list is used from any ChestBehaviour in the scene to retrieve a random drop item
    public List<GameObject> chestDrops;
    public int listCount;

    private void Awake()
    {
        //Set the Singleton to this script to cache it's reference
        S = this;
    }

    private void Start()
    {
        listCount = chestDrops.Count;
    }

    private void OnDestroy()
    {
        S = null;
    }
}
