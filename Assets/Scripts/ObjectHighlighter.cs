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
    public void HighlightLabels(List<string> labelNames)
    {
        if (labelNames == null) return;
        foreach (var labelName in labelNames)
        {
            GameObject highlightObject = GameObject.Find(labelName);

            if (highlightObject == null) continue;
            highlightObject.GetComponent<MeshRenderer>().material = highlightMaterial;
            highlightedObjects.Add(highlightObject);
        }
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
