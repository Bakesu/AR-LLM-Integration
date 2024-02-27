using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    [SerializeField] private GameObject cpuObject;
    [SerializeField] private GameObject motherboardObject;
    public void OnResponseReceived(ExtractedData extractedData)
    {
        
        if (extractedData != null)
        {
            if(extractedData.Label == "A")
            {
                //highlight the motherboard object
                motherboardObject.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                cpuObject.gameObject.GetComponent<Renderer>().material.color = Color.white;

            }
            else if(extractedData.Label == "B")
            {
                //highlight the CPU object
                cpuObject.gameObject.GetComponent<Renderer>().material.color = Color.blue;
                motherboardObject.gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }
}
