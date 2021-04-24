using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{

    public enum LootItem
    {
        questKey,
        medicine,
        lampOil
    }

    public LootItem item;
    public float fuelRecover;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out StateController player))
        {
            player.RecieveLoot(this);
            Destroy(gameObject);
        }
    }
}
