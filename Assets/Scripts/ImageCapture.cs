using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System;
using System.Collections;
using GLTFast.Schema;
using UnityEngine.UIElements;

public class ImageCapture : MonoBehaviour
{
    private PhotoCapture photoCaptureObject = null;
    public GameObject screenshotObject;
    [SerializeField]
    public RequestHandler requestHandler;

    [SerializeField]
    public SpeechInput speechInput;


    /**
     * If we are in the editor, take a screenshot and send it, else take a photo and send it
     */

    public void CaptureImageAndSendIt()
    {
        
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Debug.Log("windows editor");
            StartCoroutine(TakeScreenshotAndSendIt());
        }
        else
        {
            //Debug.Log("If this shows on Hololens, we are in the correct statement");
            // Create a PhotoCapture object
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
            
        }
    }

    /**
     * Take a screenshot of the screen (on computer) and make postrequest
     */
    IEnumerator TakeScreenshotAndSendIt()
    {
        yield return new WaitForEndOfFrame();
        var targetTexture = ScreenCapture.CaptureScreenshotAsTexture();
        StartCoroutine(ShowImage(targetTexture));
    }

    /**
     * This method is called when the PhotoCapture object has been created
     */
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
        //Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();
        CameraParameters cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = 1920;
        cameraParameters.cameraResolutionHeight = 1080;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        // Activate the camera
        captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Camera ready");
            // Take a picture when the camera is ready
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.LogError("in OnCapturedPhotoToMemory");
        if (result.success)
        {
            // Create a texture and copy the photo capture's result into the texture
            Texture2D targetTexture = new Texture2D(1920, 1080);
            Debug.Log("Just before uploading image data to texture");
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            // Save the image, here we use a simple method and save to persistentDataPath
            // We may want this later for conducting user studies.
            //string filePath = Path.Combine(Application.persistentDataPath, "capturedImage.png");
            //File.WriteAllBytes(filePath, imageAsPNG);
            Debug.Log("Show image, see if it delays the general code or not");
            StartCoroutine(requestHandler.ImageRequest(speechInput.dictationResult, targetTexture.EncodeToPNG()));
            StartCoroutine(ShowImage(targetTexture));
        }
        else
        {
            Debug.LogError("Failed to capture photo to memory.");
        }

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    private IEnumerator ShowImage(Texture2D target)
    {     
        screenshotObject.GetComponent<Renderer>().material.mainTexture = target;
        screenshotObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(10);
        screenshotObject.gameObject.SetActive(false);
    }


    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    void ApplyGridFilterToImage(Texture2D targetTexture)
    {
        Texture2D newTex = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.ARGB32, false);
        newTex.SetPixels(targetTexture.GetPixels());
        //combine textures
        newTex.Apply();
        // Apply a grid filter to the image
        targetTexture.Apply();
    }
}