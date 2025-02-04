using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using Vuforia;

public class ObjectHighlighter : MonoBehaviour
{

    [SerializeField]
    public Material transparentMaterial;
    [SerializeField]
    public Material highlightMaterial;
    [SerializeField]
    public GameObject MBCenter;

    internal Dictionary<string, GameObject> imageTargets = new Dictionary<string, GameObject>();

    private List<GameObject> highlightedLabels = new List<GameObject>();

    private LineRenderer lineRenderer = null;
    private GameObject fromObject;
    private GameObject toObject;
    private bool isLineRendererActive = false;
    private List<string> labelList = new List<string>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            string targetName = child.GetComponent<ImageTargetBehaviour>().TargetName;
            imageTargets.Add(targetName, child.gameObject);
        }
    }

    private void Update()
    {
        if (isLineRendererActive)
        {
            lineRenderer = GameObject.Find("Arrow").GetComponent<LineRenderer>();


            lineRenderer.SetPosition(0, fromObject.transform.position);
            lineRenderer.SetPosition(1, toObject.transform.position);
            if (!ObjectsAreWithinRange(fromObject, toObject))
            {
                lineRenderer.GetComponent<LineRenderer>().enabled = true;
            }
            else
            {
                lineRenderer.GetComponent<LineRenderer>().enabled = false;
            }
        }
    }

    private bool ObjectsAreWithinRange(GameObject fromObject, GameObject toObject)
    {
        if (Vector3.Distance(fromObject.transform.position, toObject.transform.position) < 0.3f)
        {
            return true;
        }
        else
        {
            return false;
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
            highlightedLabels.Add(highlightObject);
        }
    }


    internal void HighlightObject(string targetObject)
    {
        GameObject value;
        imageTargets.TryGetValue(targetObject, out value);
        if (imageTargets.ContainsKey(targetObject) || value.activeInHierarchy)
        {
            var outline = imageTargets[targetObject].transform.GetChild(0);
            outline.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Component not found");
        }
    }

    public void CreateArrowObject(string from, string to)
    {
        LineRenderer lineRenderer = new GameObject("Arrow").AddComponent<LineRenderer>();
        lineRenderer.gameObject.tag = "Highlight";
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.001f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, imageTargets[from].transform.position);
        lineRenderer.SetPosition(1, imageTargets[to].transform.position);

        fromObject = imageTargets[from];
        toObject = imageTargets[to];
        if(toObject.name == "Motherboard")
        {
            toObject = MBCenter;
        }
        //CalculateArrowEnd(toLabelList);
        isLineRendererActive = true;

    }

    //Maybe this is redundant - we dont need the line to be directly to the highlight area as long as it points to the middle of the object
    //private void CalculateArrowEnd(List<string> toLabelList)
    //{
    //    var centerPosition = Vector3.zero;
    //    foreach (var label in toLabelList)
    //    {
    //        centerPosition += imageTargets[label].transform.position;
    //    }
    //    centerPosition /= toLabelList.Count;
    //    var arrowEndPositionObject = new GameObject("ArrowEnd");
    //    arrowEndPositionObject.transform.parent = toObject.transform;
    //    arrowEndPositionObject.transform.position = centerPosition;

    //}

    public Vector3 GetCurvePoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p1 = Vector3.Lerp(a, b, t);
        Vector3 p2 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p1, p2, t);
    }

    public IEnumerator ClearAllHighlights()
    {
        ClearLabelHighlights();
        ClearObjectHighlights();
        RemoveLineRenderer();
        yield return "";
    }

    public void ClearLabelHighlights()
    {
        foreach (var highlightedObject in highlightedLabels)
        {
            highlightedObject.GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        highlightedLabels.Clear();
    }


    public void ClearObjectHighlights()
    {
        foreach (var imageTarget in imageTargets)
        {
            var outline = imageTarget.Value.transform.GetChild(0);
            outline.gameObject.SetActive(false);
        }
    }

    public void RemoveLineRenderer()
    {
        if (lineRenderer != null) { Destroy(lineRenderer.gameObject); };
        isLineRendererActive = false;
    }
}
