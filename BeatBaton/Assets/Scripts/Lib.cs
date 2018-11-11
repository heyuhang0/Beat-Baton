using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lib : MonoBehaviour {
	public static float MapRange(float value, float sourceLow, float sourceHigh, float dstLow, float dstHigh)
    {
        return (value - sourceLow) / (sourceHigh - sourceLow) * (dstHigh - dstLow) + dstLow;
    }
}
