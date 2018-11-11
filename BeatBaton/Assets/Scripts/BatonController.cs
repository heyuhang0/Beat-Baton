using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using NWH;

public class BatonController : MonoBehaviour
{
    public float zPosition;
    public RawImage debugWindow;
    public Vector3 greenLower, greenUpper;
    public Vector2 batonRangeX, batonRangeY;
    public int serverPort;

    private WebCamTexture webCamTexture;
    private BackgroundSubtractorMOG2 backgroundSubtractor;
    private Scalar greenLowerMat, greenUpperMat;
    private Mat frame, blurred, maskBG, maskColor, maskYCrCb, mask, hsv, yCrCb, nm, debugFrame;
    private Texture2D tex;
    private float realBatonX, realBatonY;
    private Quaternion nextDirection;
    private int cm = Setting.cameraMultipiler;

    void Start()
    {
        webCamTexture = new WebCamTexture(WebCamTexture.devices[Setting.cameraIndex].name, 320*cm, 480*cm, 120);
        webCamTexture.Play();
        debugWindow.enabled = Setting.cameraWindowEnable;

        tex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
        backgroundSubtractor = BackgroundSubtractorMOG2.Create(500, 16, true);
        frame = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC4);
        blurred = new Mat();
        maskBG = new Mat();
        maskColor = new Mat();
        maskYCrCb = new Mat();
        mask = new Mat();
        hsv = new Mat();
        yCrCb = new Mat();
        nm = new Mat();
        debugFrame = new Mat();

        greenLowerMat = new Scalar(greenLower[0], greenLower[1], greenLower[2]);
        greenUpperMat = new Scalar(greenUpper[0], greenUpper[1], greenUpper[2]);

        NetworkServer.RegisterHandler(64, OnServerReceived);
        NetworkServer.Listen(serverPort);
        NetworkServer.maxDelay = 0;
    }

    void OnDestroy () {
        webCamTexture.Stop();
        NetworkServer.Shutdown();
    }

    private void OnServerReceived(NetworkMessage netMsg)
    {
        UserMessage Msg = netMsg.ReadMessage<UserMessage>();
        nextDirection = Msg.orientation;
    }

    // Update is called once per frame
    void Update()
    {
        if (webCamTexture.didUpdateThisFrame && webCamTexture.isPlaying)
        {
            CamUpdate();
        }

        transform.rotation = nextDirection;
        transform.position = new Vector3(realBatonX, realBatonY, zPosition);
        transform.Translate(new Vector3(0, -1.0f, 0), Space.Self);
        // Debug.Log("FPS: " + 1.0f / Time.deltaTime);
    }

    void CamUpdate()
    {
        CvUtil.GetWebCamMat(webCamTexture, ref frame);
        Cv2.GaussianBlur(frame, blurred, new Size(2*cm + 1, 2*cm + 1), 0);

        backgroundSubtractor.Apply(blurred, maskBG);
        Cv2.Erode(maskBG, maskBG, nm, default(Point?), 1*cm);
        Cv2.Dilate(maskBG, maskBG, nm, default(Point?), 2*cm);

        Cv2.CvtColor(blurred, hsv, ColorConversionCodes.BGR2HSV);
        Cv2.InRange(hsv, greenLowerMat, greenUpperMat, maskColor);
        Cv2.Erode(maskColor, maskColor, nm, default(Point?), 1*cm);
        Cv2.Dilate(maskColor, maskColor, nm, default(Point?), 2*cm);

        //Cv2.CvtColor(blurred, yCrCb, ColorConversionCodes.BGR2YCrCb);
        //Cv2.InRange(yCrCb, new Scalar(0, 0, 0), new Scalar(160, 120, 120), maskYCrCb);

        Cv2.BitwiseAnd(maskBG, maskColor, mask);
        Cv2.Dilate(mask, mask, nm, default(Point?), 5*cm);
        Cv2.Erode(mask, mask, nm, default(Point?), 5*cm);

        Point[][] points;
        HierarchyIndex[] indexs;
        Cv2.FindContours(mask, out points, out indexs, RetrievalModes.External, ContourApproximationModes.ApproxSimple);


        if (points.Length > 0)
        {
            int indexOfMax = 0;
            double maximumArea = 0;
            for (int i = 0; i < points.Length; i++)
            {
                double size = Cv2.ContourArea(points[i]);
                if (size > maximumArea)
                {
                    maximumArea = size;
                    indexOfMax = i;
                }
            }
            Point2f center;
            float radius;
            Cv2.MinEnclosingCircle(points[indexOfMax], out center, out radius);

            realBatonX = Lib.MapRange(center.X, webCamTexture.width, 0, batonRangeX.x, batonRangeX.y);
            realBatonY = Lib.MapRange(center.Y, webCamTexture.height, 0, batonRangeY.x, batonRangeY.y);
        }

        if (debugWindow.IsActive())
        {
            Cv2.BitwiseNot(mask, mask);
            frame.SetTo(0, mask);
            UpdateDebugWindow(frame, false);
        }
    }

    void UpdateDebugWindow(Mat frame, bool convert = false)
    {
        if (debugWindow.IsActive())
        {
            if (convert)
            {
                Cv2.CvtColor(frame, debugFrame, ColorConversionCodes.GRAY2BGRA);
                CvConvert.MatToTexture2D(debugFrame, ref tex);
            }
            else
            {
                CvConvert.MatToTexture2D(frame, ref tex);
            }
            debugWindow.texture = tex;
        }
    }

    public static void vibrate() {
        NetworkServer.SendToAll(44, new UserMessage());
    }
}

public class UserMessage : MessageBase
{	
	public Quaternion orientation;
}