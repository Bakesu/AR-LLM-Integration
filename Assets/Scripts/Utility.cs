using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtractedData
{
    public List<string> Label { get; set; }
    public string TextContent { get; set; }
}

public static class Utility
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

        } catch (Exception e)
        {
            Debug.Log("No label was given");
            textcontent = response;
            labels = null;
        }
        
        return new ExtractedData
        {
            Label = labels,
            TextContent = textcontent
        };
    }
}