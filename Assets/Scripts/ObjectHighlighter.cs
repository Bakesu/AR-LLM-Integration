using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ObjectHighlighter : MonoBehaviour
{
    public Dictionary<string, GameObject> imageTargets = new Dictionary<string, GameObject>();

    [SerializeField]
    public Material transparentMaterial;
    [SerializeField]
    public Material highlightMaterial;

    void Start()
    {
        foreach (Transform child in transform)
        {
            string targetName = child.GetComponent<ImageTargetBehaviour>().TargetName;
            imageTargets.Add(targetName, child.gameObject);
        }
    }
    public void HighlightLabel(string labelName)
    {
        if (labelName == null) return;
        GameObject highlightObject = GameObject.Find(labelName);

        if (highlightObject == null) return;
        highlightObject.GetComponent<MeshRenderer>().material = highlightMaterial;
    }
}
