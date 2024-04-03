using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;


public class AIBehaviourHandler : MonoBehaviour
{
    [SerializeField]
    private ObjectHighlighter objectHighlighter;
    internal string sceneComponentList;
    public void highlight_objects(string componentName)
    {
        Debug.Log(componentName);
        if(objectHighlighter.imageTargets.ContainsKey(componentName))
        {            
            objectHighlighter.highlightObject(componentName);
        }
        else
        {
            Debug.Log("Component not found");
        }
        //string[] labels = parameters.Split(',');
        //objectHighlighter.highlightObject();
    }

    //Creates component list based on the children of the objectHighlighter Gameobject
    internal string CreateComponentList()
    {
        var sceneObjectList = "[";
        foreach (var qrObjectPair in objectHighlighter.imageTargets)
        {
            string listAppend = qrObjectPair.Key + ":" + qrObjectPair.Value.gameObject.name + ", ";
            sceneObjectList = string.Concat(sceneObjectList, listAppend);
        }
        char[] charsToTrim = { ',', ' ' };
        sceneObjectList = sceneObjectList.TrimEnd(charsToTrim);
        sceneObjectList = string.Concat(sceneObjectList, "]");
        return sceneObjectList;
    }
}
