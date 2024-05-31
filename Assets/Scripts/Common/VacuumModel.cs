using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumModel : MonoBehaviour
{

    public delegate void OnItemSuck(SuckableItem item);

    public OnItemSuck onItemSuck;
    
    public Collider suckingCollider;


    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObject = other.gameObject;
        SuckableItem suckableItem = gameObject.GetComponent<SuckableItem>();
        if (!suckableItem) return;
        onItemSuck?.Invoke(suckableItem);
    }
}
