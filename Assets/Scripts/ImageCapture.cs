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
using Vuforia;
using UnityEngine.SceneManagement;

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



    public IEnumerator CaptureImageAndSendIt()
    {

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Debug.Log("windows editor");
            StartCoroutine(TakeScreenshotAndSendIt());
        }
        else
        {
            Debug.Log("Capture Image");
            
            // Create a PhotoCapture object

            PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);

        }
        yield return null;
    }

    /**
     * Take a screenshot of the screen (on computer) and make postrequest
     */
    IEnumerator TakeScreenshotAndSendIt()
    {
        yield return new WaitForEndOfFrame();
        targetTexture = ScreenCapture.CaptureScreenshotAsTexture();
        requestHandler.CreateImageRequest(speechInput.dictationResult, targetTexture.EncodeToPNG(), false);
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
            var imageAsPNG = targetTexture.EncodeToPNG();
            
            //Use the device portal to get the image from the hololens. 
            string filePath = Path.Combine(Application.persistentDataPath, "capturedImage.png");
            File.WriteAllBytes(filePath, imageAsPNG);
            requestHandler.CreateImageRequest(speechInput.dictationResult, imageAsPNG, false);
            StartCoroutine(ShowImage(targetTexture));
            // Deactivate the camera
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        else
        {
            Debug.LogError("Failed to capture photo to memory.");
        }
        
    }

    private IEnumerator ShowImage(Texture2D target)
    {
        screenshotObject.GetComponent<Renderer>().material.mainTexture = target;
        screenshotObject.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        //set to false if we want to hide the image after a few seconds
        //screenshotObject.gameObject.SetActive(false);
    }


    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;        
    }
}