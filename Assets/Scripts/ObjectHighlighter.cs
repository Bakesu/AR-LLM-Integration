using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ObjectHighlighter : MonoBehaviour
{

    [SerializeField]
    public Material transparentMaterial;
    [SerializeField]
    public Material highlightMaterial;

    internal Dictionary<string, GameObject> imageTargets = new Dictionary<string, GameObject>();

    private List<GameObject> highlightedObjects = new List<GameObject>();

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
        highlightedObjects.Add(highlightObject);
    }

    public void ClearLabelHighlights()
    {
        foreach (var highlightedObject in highlightedObjects)
        {
            highlightedObject.GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        highlightedObjects.Clear();
    }
}
