using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBehaviour : MonoBehaviour {
	public float throwMultiplier;
	public float zoomOutMultiplier = 1.2f;
	public bool active = false;

	// Update is called once per frame
	void Update () {
        if(active) {
            Vector2 directionVector = MapToDiscreteCoordinates(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
            if(directionVector.x != 0 || directionVector.y != 0) {
                float heading = Mathf.Atan2(-directionVector.x, directionVector.y);
                transform.rotation = Quaternion.Euler(0f, 0f, heading * Mathf.Rad2Deg);
            }
        }
	}

    private Vector2 MapToDiscreteCoordinates(Vector2 _input) {
        if(Mathf.Abs(_input.x) > 0.25f) {
            _input.x = Mathf.Sign(_input.x);
        } else {
            _input.x = 0;
        }

        if(Mathf.Abs(_input.y) > 0.25f) {
            _input.y = Mathf.Sign(_input.y);
        } else {
            _input.y = 0;
        }

        return _input;
    }

	public float getAngle(){
		return this.gameObject.transform.eulerAngles.z;
	}

	public float getThrowMultiplier(){
		return this.throwMultiplier;
	}

	public void setActive(bool _activation){
		this.active = _activation;
	}

}
