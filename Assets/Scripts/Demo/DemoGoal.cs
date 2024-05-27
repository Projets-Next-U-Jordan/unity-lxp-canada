using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGoal : MonoBehaviour
{
    public DemoController controller;

    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            controller.NextScenario();
        }
    }
}
