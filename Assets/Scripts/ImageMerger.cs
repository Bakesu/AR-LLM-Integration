using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMerger : MonoBehaviour
{
    [SerializeField] private Texture2D gridImage = null;
    [SerializeField] private Texture2D backgroundImage = null;
    [SerializeField] private Texture2D resultingImage = null;
    [SerializeField] private GameObject imageObject = null;

    private void Start()
    {
        Merge();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Merge();
        }    
    }
    private void Merge()
    {
        resultingImage = new Texture2D(736, 508);

        for (int x = 0; x < backgroundImage.width; x++)
        {
            for (int y = 0; y < backgroundImage.height; y++)
            {                
                var currentGridPixel = gridImage.GetPixel(x, y);                
                //Debug.Log(currentGridPixel);
                //hvis overlayimage har en pixel som er farvet, så brug den, ellers brug baggrundsbilledet
                if (currentGridPixel.r <= 0.8 && currentGridPixel.b >= 0.8f)
                {
                    resultingImage.SetPixel(x, y, Color.blue);
                }
                
            }

        }
        imageObject.GetComponent<Renderer>().material.mainTexture = resultingImage;
        resultingImage.Apply();
    }
}
