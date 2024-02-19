using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System;
using System.Collections;
using GLTFast.Schema;

public class ImageCapture : MonoBehaviour
{
    private PhotoCapture photoCaptureObject = null;
    public GameObject screenshotObject;
    [SerializeField]
    public RequestHandler requestHandler;

    [SerializeField]
    public SpeechInput speechInput;

    public void Start()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    public void CaptureImageAndSendIt()
    {

        //photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        // Create a PhotoCapture object
        //PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        StartCoroutine(TakeScreenshotAndSendIt());
        //Debug.Log("CaptureImage() called");
    }

    /**
     * Take a screenshot of the screen (on computer) and make postrequest
     */
    IEnumerator TakeScreenshotAndSendIt()
    {
        yield return new WaitForEndOfFrame();
        var targetTexture = ScreenCapture.CaptureScreenshotAsTexture();
        StartCoroutine(ShowImage(targetTexture));
        //StartCoroutine(requestHandler.PuppeteerImageRequest(targetTexture.EncodeToPNG()));
        //StartCoroutine(requestHandler.ChatGptImageRequest(speechInput.dictationResult, targetTexture.EncodeToPNG()));
    }

    /**
     * This method is called when the PhotoCapture object has been created
     */
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        Debug.LogError("in OnPhotoCaptureCreated");
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
            // Take a picture when the camera is ready
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
            //Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(1920, 1080);
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            //// Save the image, here we use a simple method and save to persistentDataPath
            //StartCoroutine(ShowImage(targetTexture));

            //StartCoroutine(requestHandler.PuppeteerImageRequest(targetTexture.EncodeToPNG()));
            StartCoroutine(requestHandler.ImageRequest(speechInput.dictationResult, targetTexture.EncodeToPNG()));


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
}