using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ExtractedData
{
    public List<string> Label { get; set; }
    public string TextContent { get; set; }
}

public static class DataUtility
{
    public static ExtractedData extractDataFromResponse(string response)
    {
        string textcontent = "";
        List<string> labels;
        try
        {
            var responseSplitted = response.Split("}");
            var labelsString = responseSplitted[0];
            textcontent = responseSplitted[1].TrimStart('.').TrimStart();
            char[] separators = new char[] { '{', '}', ','};
            string[] responseAsSubstrings = labelsString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            labels = new List<string>(responseAsSubstrings);
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i] = labels[i].Trim();
            }

        } catch (Exception e)
        {
            Debug.Log("No label was given " + e);
            textcontent = response;
            labels = null;
        }
        
        return new ExtractedData
        {
            Label = labels,
            TextContent = textcontent
        };
    }

    internal static string StringManip(string stringToTrim)
    {
        var tempString = stringToTrim.Split(':');
        char[] charsToTrim = { '"', '{','}' };     
        return tempString[1].Trim(charsToTrim);
    }
}