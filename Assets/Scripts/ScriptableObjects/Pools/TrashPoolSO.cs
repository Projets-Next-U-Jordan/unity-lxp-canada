using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TrashPoolSO", menuName = "ScriptableObjects/TrashPool")]
public class TrashPoolSO : ScriptableObject
{

    public List<TrashSO> trash;

}
