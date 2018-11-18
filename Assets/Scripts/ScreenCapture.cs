using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;
using UnityEngine.UI;
using System.Linq;

public class ScreenCapture : MonoBehaviour {

   // public RawImage ri;
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;
    InputField inputField;
    xflyOCR ocr;

    public bool shotdone = false;

    // Use this for initialization
    void Start () {
        inputField = GameObject.Find("InputField").GetComponent<InputField>();
        ocr = GameObject.Find("manager").GetComponent<xflyOCR>();

    }

    public void ocrStart() {
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
        // Copy the raw image data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        // Create a GameObject to which the texture can be applied
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));

        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;

      //  ri.texture = targetTexture;
        shotdone = true;
    }

    public bool getshot() {
        if(shotdone) {
            shotdone = false;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update () {
        if(getshot()) {
            ocr.getOCR(targetTexture);
        }
        string res = "";
        if((res = ocr.getResult()) != null) {
            inputField.text = res;
            Debug.Log("ocr:" + res);
        }
	}

    //void AnalyzeScene() {
    //    InfoPanel.text = "CALCULATION PENDING";
    //    UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    //}

    //UnityEngine.XR.WSA.WebCam.PhotoCapture _photoCaptureObject = null;
    //void OnPhotoCaptureCreated(UnityEngine.XR.WSA.WebCam.PhotoCapture captureObject) {
    //    _photoCaptureObject = captureObject;

    //    Resolution cameraResolution =
    //                                 UnityEngine.XR.WSA.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) =>
    //    res.width * res.height).First();

    //    UnityEngine.XR.WSA.WebCam.CameraParameters c = new UnityEngine.XR.WSA.WebCam.CameraParameters();
    //    c.hologramOpacity = 0.0f;
    //    c.cameraResolutionWidth = cameraResolution.width;
    //    c.cameraResolutionHeight = cameraResolution.height;
    //    c.pixelFormat = UnityEngine.XR.WSA.WebCam.CapturePixelFormat.BGRA32;

    //    captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    //}

    //private void OnPhotoModeStarted(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result) {
    //    if(result.success) {
    //        string filename = string.Format(@"terminator_analysis.jpg");
    //        string filePath = System.IO.Path.Combine(Application.persistentDataPath,
    //                                          filename);
    //        _photoCaptureObject.TakePhotoAsync(filePath, UnityEngine.XR.WSA.WebCam.PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
    //    }
    //    else {
    //        DiagnosticPanel.text = "DIAGNOSTIC\n**************\n\nUnable to start photo mode.";
    //        InfoPanel.text = "ABORT";
    //    }
    //}

    //void OnPhotoCaptureCreated(PhotoCapture captureObject) {
    //    photoCaptureObject = captureObject;

    //    Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

    //    CameraParameters c = new CameraParameters();
    //    c.hologramOpacity = 0.0f;
    //    c.cameraResolutionWidth = cameraResolution.width;
    //    c.cameraResolutionHeight = cameraResolution.height;
    //    c.pixelFormat = CapturePixelFormat.BGRA32;

    //    captureObject.StartPhotoModeAsync(c, false, OnPhotoModeStarted);
    //}

    //void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result) {
    //    photoCaptureObject.Dispose();
    //    photoCaptureObject = null;
    //}

    //private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result) {
    //    if(result.success) {
    //        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    //    }
    //    else {
    //        Debug.LogError("Unable to start photo mode!");
    //    }
    //}

    //void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame) {
    //    if(result.success) {
    //        // Create our Texture2D for use and set the correct resolution
    //        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
    //        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
    //        // Copy the raw image data into our target texture
    //        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
    //        // Do as we wish with the texture such as apply it to a material, etc.
    //    }
    //    // Clean up
    //    photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    //}
}
