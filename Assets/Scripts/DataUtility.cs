using System;
using System.Collections.Generic;
using System.Linq;
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
            textcontent = response.Split("}")[1].TrimStart('.').TrimStart();
            char[] separators = new char[] { '{', '}', ','};
            string[] responseAsSubstrings = response.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            labels = new List<string>(responseAsSubstrings);

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
}