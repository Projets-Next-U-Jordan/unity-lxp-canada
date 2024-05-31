using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerModelPoolScriptableObject", menuName = "ScriptableObjects/PlayerModelPool")]
public class PlayerModelPoolSO : ScriptableObject
{

    public List<PlayerModelSO> playerModels;

}
