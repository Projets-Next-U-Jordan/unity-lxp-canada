using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckableItem : MonoBehaviour
{

    public SuckableItemSO data;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void StartSucking()
    {
        _rigidbody.isKinematic = true;
    }

    public void StopSucking()
    {
        _rigidbody.isKinematic = false;
    }
}
