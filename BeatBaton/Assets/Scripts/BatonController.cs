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
    public Vector2 batonRangeX, batonRangeY;
    public int serverPort;
    public GameObject batonTemplate;
    public GameObject scoreTemplate;

    private WebCamTexture webCamTexture;
    private BackgroundSubtractorMOG2 backgroundSubtractor;
    private Mat frame, blurred, maskBG, maskColor, mask, hsv, nm, debugMask, debugFrame;
    private Texture2D tex;
    private int cm;

    void Start()
    {
        batonTemplate.SetActive(false);
        scoreTemplate.SetActive(false);

        cm = Setting.a.cameraMultipiler;

        webCamTexture = new WebCamTexture(WebCamTexture.devices[Setting.a.cameraIndex].name, 320*cm, 480*cm, 120);
        webCamTexture.Play();
        debugWindow.enabled = Setting.a.cameraWindowEnable;

        backgroundSubtractor = BackgroundSubtractorMOG2.Create(500, 16, true);
        frame = new Mat();
        blurred = new Mat();
        maskBG = new Mat();
        maskColor = new Mat();
        mask = new Mat();
        hsv = new Mat();
        nm = new Mat();
        debugFrame = new Mat();

        NetworkServer.RegisterHandler(64, OnServerReceived);
        NetworkServer.Listen(serverPort);
        NetworkServer.maxDelay = 0;

        foreach (BatonProfile b in Setting.a.batonProfiles) {
            b.score = 0;
        }
    }

    void OnDestroy () {
        webCamTexture.Stop();
        NetworkServer.Shutdown();
    }

    private void OnServerReceived(NetworkMessage netMsg)
    {
        UserMessage Msg = netMsg.ReadMessage<UserMessage>();
        foreach (BatonProfile profile in Setting.a.batonProfiles) {
            if (profile.profileName == Msg.profile) {
                profile.direction = Msg.orientation;
                profile.SetActive();
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (webCamTexture.didUpdateThisFrame && webCamTexture.isPlaying)
        {
            CamUpdate();
        }
        UpdateBatons();
    }

    private List<GameObject> batons = new List<GameObject>();
    private List<GameObject> scores = new List<GameObject>();
    void UpdateBatons() {
        List<BatonProfile> activeProfiles = new List<BatonProfile>();
        foreach (BatonProfile profile in Setting.a.batonProfiles) {
            if (profile.IsActive()) {
                activeProfiles.Add(profile);
            }
        }
        CheckBatonNum(activeProfiles.Count);
        for (int i = 0; i < activeProfiles.Count; i++) {
            batons[i].transform.rotation = activeProfiles[i].direction;
            Vector2 Pos2D = activeProfiles[i].position;
            batons[i].transform.position = new Vector3(Pos2D.x, Pos2D.y, zPosition);
            batons[i].transform.Translate(new Vector3(0, -1.0f, 0), Space.Self);
            batons[i].GetComponent<Renderer>().material.color = activeProfiles[i].color;

            scores[i].GetComponent<Text>().text = activeProfiles[i].score.ToString();
            scores[i].GetComponentInChildren<RawImage>().color = activeProfiles[i].color;
        }
    }

    void CheckBatonNum (int expectedNum) {
        while (batons.Count < expectedNum) {
            batons.Add(Instantiate(batonTemplate));
            GameObject newScoreObj = Instantiate(scoreTemplate);
            newScoreObj.transform.SetParent(scoreTemplate.transform.parent);
            scores.Add(newScoreObj);
        }
        for (int i = 0; i < batons.Count; i++) {
            batons[i].SetActive(i < expectedNum);
            scores[i].SetActive(i < expectedNum);
        }
    }

    void CamUpdate()
    {
        CvUtil.GetWebCamMat(webCamTexture, ref frame);
        Cv2.GaussianBlur(frame, blurred, new Size(2*cm + 1, 2*cm + 1), 0);

        backgroundSubtractor.Apply(blurred, maskBG);
        Cv2.Erode(maskBG, maskBG, nm, default(Point?), 1*cm);
        Cv2.Dilate(maskBG, maskBG, nm, default(Point?), 2*cm);

        Cv2.CvtColor(blurred, hsv, ColorConversionCodes.RGB2HSV);

        if (DebugWindowAvailable())
        {
            debugMask.SetTo(0);
        }

        foreach (BatonProfile profile in Setting.a.batonProfiles) {
            if (!profile.IsActive()) {
                continue;
            }

            for (int additionalRange = 0; additionalRange < 5; additionalRange ++) {
                Vector3 hsvLower = Lib.Limit(profile.hsvLower - additionalRange * new Vector3(2, 20, 20), -50, 255, 60, 255, 30, 255);
                Vector3 hsvUpper = Lib.Limit(profile.hsvUpper + additionalRange * new Vector3(2, 20, 20), -50, 255, 60, 255, 30, 255);
                BetterCv2.InRangeHSV(hsv, hsvLower, hsvUpper, maskColor);
                Cv2.Erode(maskColor, maskColor, nm, default(Point?), 1*cm);
                Cv2.Dilate(maskColor, maskColor, nm, default(Point?), 2*cm);

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

                    float x = Lib.MapRange(center.X, webCamTexture.width, 0, batonRangeX.x, batonRangeX.y);
                    float y = Lib.MapRange(center.Y, webCamTexture.height, 0, batonRangeY.x, batonRangeY.y);
                    profile.position = new Vector2(x, y);

                    Debug.Log(additionalRange + "\t" + hsvLower.ToString() + "\t" + hsvUpper.ToString());
                    break;
                }
                else if (additionalRange == 4) {
                    Debug.Log("Fuck you!!!!!");
                }
            }

            if (DebugWindowAvailable())
            {
                Cv2.BitwiseOr(mask, debugMask, debugMask);
            }
        }

        if (DebugWindowAvailable())
        {
            Cv2.BitwiseNot(debugMask, debugMask);
            frame.SetTo(0, debugMask);
            UpdateDebugWindow(frame, false);
        }
    }

    bool DebugWindowAvailable() {
        if (!debugWindow.IsActive())
            return false;
        // Make sure the camera is working (Avoid crash on Mac)
        if (webCamTexture.width > 100) {
            if (tex == null) {  
                tex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
                debugMask = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC1);
            }
            return true;
        }
        return false;
    }

    void UpdateDebugWindow(Mat frame, bool convert = false)
    {
        if (DebugWindowAvailable())
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
