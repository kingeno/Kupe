using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : MonoBehaviour {

    public bool hasAPlayerCubeOnIt;

	void Start () {
        hasAPlayerCubeOnIt = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasAPlayerCubeOnIt = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            hasAPlayerCubeOnIt = false;
    }
}
