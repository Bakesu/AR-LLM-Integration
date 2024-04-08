using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingNotifier : MonoBehaviour
{
    public void OnTrackingFound()
    {
        Debug.Log("Tracking Found");
    }
}
