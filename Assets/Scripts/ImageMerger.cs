using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMerger : MonoBehaviour
{
    [SerializeField] private Texture2D gridImage = null;
    [SerializeField] private Texture2D backgroundImage = null;
    //[SerializeField] private Texture2D resultingImage = null;
    [SerializeField] private GameObject imageObject = null;

    private void Start()
    {
        //Merge();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Texture2D imageWithGrid = ApplyGridOnImage(backgroundImage, 0.8f, 0.2f);
            imageObject.GetComponent<Renderer>().material.mainTexture = imageWithGrid;
        }    
    }
    private Texture2D ApplyGridOnImage(Texture2D targetImage, float gridAndLabelOpacity, float whiteBackgroundOpacity)
    {
        //Only necessary if you want to keep the original image - which we want when developing
        Texture2D resultingImage = new Texture2D(targetImage.width, targetImage.height);
        resultingImage.SetPixels(targetImage.GetPixels());
        resultingImage.Apply();

        for (int x = 0; x < backgroundImage.width; x++)
        {
            for (int y = 0; y < backgroundImage.height; y++)
            {                
                var currentGridPixel = gridImage.GetPixel(x, y);

                if (currentGridPixel.r > 0.8f && currentGridPixel.g < 0.9f || currentGridPixel.b > 0.8f && currentGridPixel.g < 0.9f)
                {
                    Color interpolatedColor = Color.Lerp(backgroundImage.GetPixel(x, y), currentGridPixel, gridAndLabelOpacity);
                    resultingImage.SetPixel(x, y, interpolatedColor);
                } else
                {
                    Color interpolatedColor = Color.Lerp(backgroundImage.GetPixel(x, y), currentGridPixel, whiteBackgroundOpacity);
                    resultingImage.SetPixel(x, y, interpolatedColor);
                }
            }

        }
        resultingImage.Apply();
        return resultingImage;
    }
}
