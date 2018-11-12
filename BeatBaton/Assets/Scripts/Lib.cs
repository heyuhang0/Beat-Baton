using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Lib : MonoBehaviour {
	public static float MapRange(float value, float sourceLow, float sourceHigh, float dstLow, float dstHigh)
    {
        return (value - sourceLow) / (sourceHigh - sourceLow) * (dstHigh - dstLow) + dstLow;
    }
}

public class UserMessage : MessageBase
{	
    public string profile;
	public Quaternion orientation;
}