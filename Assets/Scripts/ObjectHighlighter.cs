using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    public void OnResponseReceived(ExtractedData extractedData)
    {
        if (extractedData == null) return;
        GameObject gridCellObject = GameObject.Find(extractedData.Label);

        if (gridCellObject == null) return;
        gridCellObject.SetActive(true);
    }
}
