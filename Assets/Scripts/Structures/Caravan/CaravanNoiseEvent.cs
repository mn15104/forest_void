using UnityEngine;
using System.Collections;

public class CaravanNoiseEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.GetComponentInChildren<OVRPlayerController>() || other.gameObject.GetComponentInChildren<HumanController>())
        //{
        //        FindObjectOfType<MainAudioController>().ForcePause();
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.GetComponentInChildren<OVRPlayerController>() || other.gameObject.GetComponentInChildren<HumanController>())
        //{
        //    FindObjectOfType<MainAudioController>().ForcePlay();
        //}
    }
}
