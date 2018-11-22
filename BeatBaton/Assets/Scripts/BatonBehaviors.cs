using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatonBehaviors : MonoBehaviour {
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Cube")) {
			foreach (BatonProfile b in Setting.a.batonProfiles) {
				if (b.color == GetComponent<Renderer>().material.color) {
					b.score ++;
				}
			}
		}
	}
}
