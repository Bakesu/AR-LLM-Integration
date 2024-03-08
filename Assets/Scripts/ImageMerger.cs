using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMerger : MonoBehaviour
{
    [SerializeField] private Texture2D gridImage;

    private void Start()
    {
        //Merge();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) {
        //    Texture2D imageWithGrid = ApplyGridOnImage(backgroundImage, 0.8f, 0.2f);
        //    imageObject.GetComponent<Renderer>().material.mainTexture = imageWithGrid;
        //}    
    }
    public Texture2D ApplyGridOnImage(Texture2D targetImage, float gridAndLabelOpacity, float whiteBackgroundOpacity)
    {


        for (int x = 0; x < targetImage.width; x++)
        {
            for (int y = 0; y < targetImage.height; y++)
            {                
                var currentGridPixel = gridImage.GetPixel(x, y);

                if (currentGridPixel.r > 0.8f && currentGridPixel.g < 0.9f || currentGridPixel.b > 0.8f && currentGridPixel.g < 0.9f)
                {                    
                    Color interpolatedColor = Color.Lerp(targetImage.GetPixel(x, y), currentGridPixel, gridAndLabelOpacity);
                    targetImage.SetPixel(x, y, interpolatedColor);
                }
                else
                {
                    Color interpolatedColor = Color.Lerp(targetImage.GetPixel(x, y), currentGridPixel, whiteBackgroundOpacity);
                    var grayscalePixel = new Color(0.299f * interpolatedColor.r, 0.587f * interpolatedColor.g, 0.114f * interpolatedColor.b, whiteBackgroundOpacity);
                    targetImage.SetPixel(x, y, grayscalePixel);
                }
            }

        }
        targetImage.Apply();
        return targetImage;
    }
}
