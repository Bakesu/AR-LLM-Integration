using System;
using System.Linq;
using UnityEngine;

public class ExtractedData
{
    public string Label { get; set; }
    public string TextContent { get; set; }
}

public static class Utility
{
    public static ExtractedData extractDataFromResponse(string response)
    {
        string textcontent = response.Split("}")[1].TrimStart('.').TrimStart();

        char[] separators = new char[] { '{', '}', ',' };
        string[] responseAsSubstrings = response.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        
        return new ExtractedData
        {
            Label = responseAsSubstrings?[0],
            TextContent = textcontent
        };
    }
}