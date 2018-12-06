using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using OpenCvSharp;

public class Lib : MonoBehaviour {
	public static float MapRange(float value, float sourceLow, float sourceHigh, float dstLow, float dstHigh)
    {
        return (value - sourceLow) / (sourceHigh - sourceLow) * (dstHigh - dstLow) + dstLow;
    }

    public static Vector3 Limit(Vector3 input, float xl, float xh, float yl, float yh, float zl, float zh) {
        float x = Mathf.Min(Mathf.Max(input.x, xl), xh);
        float y = Mathf.Min(Mathf.Max(input.y, yl), yh);
        float z = Mathf.Min(Mathf.Max(input.z, zl), zh);
        return new Vector3(x, y, z);
    }
}

public class MyNetworkServer {
    readonly static int port = 7755;
    public static bool started = false;
    private static void Init() {
        NetworkServer.Listen(7755);
        NetworkServer.maxDelay = 0;
        started = true;
    }

    public static void RegisterHandler(short msgType, NetworkMessageDelegate handler) {
        if (!started) {
            Init();
        }
        NetworkServer.RegisterHandler(msgType, handler);
    }

    public static void UnregisterHandler(short msgType) {
        NetworkServer.UnregisterHandler(msgType);
    }
}

public class BetterCv2 {
    public static void InRangeHSV(Mat frame, Vector3 hsvLower, Vector3 hsvUpper, Mat outputMask) {
        Scalar hsvLowerMat, hsvUpperMat;
        if (hsvLower.x >= 0 && hsvUpper.x <= 180) {
            hsvLowerMat = new Scalar(hsvLower.x, hsvLower.y, hsvLower.z);
            hsvUpperMat = new Scalar(hsvUpper.x, hsvUpper.y, hsvUpper.z);
            Cv2.InRange(frame, hsvLowerMat, hsvUpperMat, outputMask);
            return;
        } else if (hsvLower.x < 0) {
            hsvLowerMat = new Scalar(0, hsvLower.y, hsvLower.z);
            hsvUpperMat = new Scalar(hsvUpper.x, hsvUpper.y, hsvUpper.z);
            Cv2.InRange(frame, hsvLowerMat, hsvUpperMat, outputMask);
            Mat tempMask = new Mat();
            hsvLowerMat = new Scalar(180 + hsvLower.x, hsvLower.y, hsvLower.z);
            hsvUpperMat = new Scalar(180, hsvUpper.y, hsvUpper.z);
            Cv2.InRange(frame, hsvLowerMat, hsvUpperMat, tempMask);
            Cv2.BitwiseOr(outputMask, tempMask, outputMask);
            tempMask.Release();
        } else {
            hsvLowerMat = new Scalar(hsvLower.x, hsvLower.y, hsvLower.z);
            hsvUpperMat = new Scalar(180, hsvUpper.y, hsvUpper.z);
            Cv2.InRange(frame, hsvLowerMat, hsvUpperMat, outputMask);
            Mat tempMask = new Mat();
            hsvLowerMat = new Scalar(0, hsvLower.y, hsvLower.z);
            hsvUpperMat = new Scalar(hsvUpper.x - 180, hsvUpper.y, hsvUpper.z);
            Cv2.InRange(frame, hsvLowerMat, hsvUpperMat, tempMask);
            Cv2.BitwiseOr(outputMask, tempMask, outputMask);
            tempMask.Release();
        }
    }
}

public class UserMessage : MessageBase
{	
    public string profile;
	public Quaternion orientation;
}