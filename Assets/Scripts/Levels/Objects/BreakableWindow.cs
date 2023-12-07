using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWindow : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col) {
        Debug.Log("collision detected");
        if (col.gameObject.GetComponent<MovingPlatform>() != null) {
            //MovingElementContoller moveController = col.transform.parent.gameObject.GetComponent<MovingElementController>();
            Levels.Objects.Platform.ChaosEffector chaosEffector = col.transform.parent.gameObject.GetComponent<Levels.Objects.Platform.ChaosEffector>();
            if (chaosEffector.VirusThresholdMet()) {
                Debug.Log("window break");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("trigger detected");
        if (col.gameObject.GetComponent<MovingPlatform>() != null) {
            //MovingElementContoller moveController = col.transform.parent.gameObject.GetComponent<MovingElementController>();
            Levels.Objects.Platform.ChaosEffector chaosEffector = col.transform.parent.gameObject.GetComponent<Levels.Objects.Platform.ChaosEffector>();
            if (chaosEffector.VirusThresholdMet()) {
                Debug.Log("window break trigger");
            }
        }
    }

    void OnTriggerStay2D(Collider2D col) {
        Debug.Log("trigger stay active");
    }
}
