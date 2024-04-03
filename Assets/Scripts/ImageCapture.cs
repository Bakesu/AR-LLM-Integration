using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System;
using System.Collections;
using GLTFast.Schema;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class ImageCapture : MonoBehaviour
{
    [SerializeField]
    private GameObject screenshotObject;
    [SerializeField]
    private RequestHandler requestHandler;

    internal PhotoCapture photoCaptureObject;
    private SpeechInput speechInput;    
    private ImageMerger imageMerger;

    private Texture2D targetTexture;

    private void Start()
    {
        imageMerger = gameObject.GetComponent<ImageMerger>();
        speechInput = gameObject.GetComponent<SpeechInput>();
    }
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
            PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);

        }
    }

    /**
     * Take a screenshot of the screen (on computer) and make postrequest
     */
    IEnumerator TakeScreenshotAndSendIt()
    {
        Debug.Log("editor only code");
        yield return new WaitForEndOfFrame();
        targetTexture = new Texture2D(1920, 1080);
        targetTexture = imageMerger.ApplyGridOnImage(targetTexture, 1f, 1f);
        StartCoroutine(ShowImage(targetTexture));
        //StartCoroutine(requestHandler.ImageRequest(speechInput.dictationResult, targetTexture.EncodeToPNG()));
    }

    /**
     * This method is called when the PhotoCapture object has been created
     */
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;        
        CameraParameters cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.5f;
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
            // Take a picture when the camera is ready
            try
            {
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when taking photo: " + e);
            }
            //photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);

        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // Create a texture and copy the photo capture's result into the texture
            targetTexture = new Texture2D(1920, 1080);
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            targetTexture = imageMerger.ApplyGridOnImage(targetTexture, 0.5f, 0.1f);
            var imageAsPNG = targetTexture.EncodeToPNG();
            
            //Use the device portal to get the image from the hololens. 
            string filePath = Path.Combine(Application.persistentDataPath, "capturedImage.png");
            File.WriteAllBytes(filePath, imageAsPNG);

            requestHandler.CreateImageRequest(speechInput.dictationResult, imageAsPNG);
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
        yield return new WaitForSeconds(2);
        //set to false if we want to hide the image after a few seconds
        screenshotObject.gameObject.SetActive(true);
    }


    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}