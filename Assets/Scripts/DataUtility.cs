using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ExtractedLabelData
{
    public List<string> Label { get; set; }
    public string TextContent { get; set; }
}

public static class DataUtility
{
    //Creates component list based on the children of the objectHighlighter Gameobject
    internal static string CreateComponentList(Dictionary<string, GameObject> imageTargets)
    {
        var sceneObjectList = "[";
        foreach (var qrObjectPair in imageTargets)
        {
            string listAppend = qrObjectPair.Value.gameObject.name + ":" + qrObjectPair.Key + ", ";
            sceneObjectList = string.Concat(sceneObjectList, listAppend);
        }
        char[] charsToTrim = { ',', ' ' };
        sceneObjectList = sceneObjectList.TrimEnd(charsToTrim);
        sceneObjectList = string.Concat(sceneObjectList, "]");
        return sceneObjectList;
    }

    public static bool IsLabelResponse(string response)
    {
        if (response.StartsWith("{"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static ExtractedLabelData extractDataFromResponse(string response)
    {
        string textcontent = "";
        List<string> labels;
        try
        {
            var responseSplitted = response.Split("}");
            var labelsString = responseSplitted[0];
            textcontent = responseSplitted[1].TrimStart('.').TrimStart();
            char[] separators = new char[] { '{', '}', ',' };
            string[] responseAsSubstrings = labelsString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            labels = new List<string>(responseAsSubstrings);
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i] = labels[i].Trim();
            }

        }
        catch (Exception e)
        {
            Debug.Log("No label was given " + e);
            textcontent = response;
            labels = null;
        }

        return new ExtractedLabelData
        {
            Label = labels,
            TextContent = textcontent
        };
    }

    internal static string ExtractFunctionArgumentsFromFCString(string stringToManipulate)
    {
        if (stringToManipulate == null || stringToManipulate == "") return "";
        char[] charsToTrim = { '"', '{', '}' };        
        var tempString = stringToManipulate.Split(':');
        return tempString[1].Trim(charsToTrim);

    }
}