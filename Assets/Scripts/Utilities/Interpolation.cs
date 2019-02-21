using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Interpolation {

	public static float EaseIn(float v) {
		return (1 - Mathf.Cos(v * Mathf.PI * 0.5f));
	}

	public static float EaseOut(float v) {
		return (Mathf.Sin(v * Mathf.PI * 0.5f));
	}

	public static float SmoothStep(float v) {
		return (v*v*(3 - 2*v));
	}

	public static float SmootherStep(float v) {
		return (v*v*v*(v*(v*6 - 15) + 10));
	}

	// Reference
	// https://github.com/acron0/Easings/blob/master/Easings.cs
	public static float BounceEaseOut(float v) {
		if(v < 4/11.0f) {
			return (121 * v * v)/16.0f;
		} else if(v < 8/11.0f) {
			return (363/40.0f * v * v) - (99/10.0f * v) + 17/5.0f;
		}
		else if(v < 9/10.0f) {
			return (4356/361.0f * v * v) - (35442/1805.0f * v) + 16061/1805.0f;
		} else {
			return (54/5.0f * v * v) - (513/25.0f * v) + 268/25.0f;
		}
	}

	static public float BounceEaseIn(float v) {
		return 1 - BounceEaseOut(1 - v);
	}

	static public float BounceEaseInOut(float v) {
		if(v < 0.5f) {
			return 0.5f * BounceEaseIn(v*2);
		} else {
			return 0.5f * BounceEaseOut(v * 2 - 1) + 0.5f;
		}
	}
}