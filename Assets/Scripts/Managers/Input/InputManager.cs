using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// PS4: Sony Interactive Entertainment Wireless Controller
// SWITCH: Unknown Pro Controller
public class InputManager : MonoBehaviour {

	public static InputManager instance;
	private InControl.InputDevice activeDevice;

	private const KeyCode jumpKey = KeyCode.Space;
	private const KeyCode startDialogueKey = KeyCode.E;
	private const KeyCode petTheDogKey = KeyCode.R;

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	void Update() {
    	activeDevice = InControl.InputManager.ActiveDevice;
    }

	public bool PressedJump() {
		if(activeDevice != null) {
			return (activeDevice.Action1.WasPressed || Input.GetKeyDown(jumpKey));
		} else {
			return Input.GetKeyDown(KeyCode.Space);
		}
	}

	public bool ReleasedJump() {
		if(activeDevice != null) {
			return (activeDevice.Action1.WasReleased || Input.GetKeyUp(jumpKey));
		} else {
			return Input.GetKeyUp(KeyCode.Space);
		}
	}

	public bool PressedConfirm() {
		if(activeDevice != null) {
			return activeDevice.Action1.WasPressed || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
		} else {
			return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
		}
	}

	public bool PressedStart() {
		if(activeDevice != null) {
			return activeDevice.Command.WasPressed || Input.GetKeyDown(KeyCode.Escape);
		} else {
			return Input.GetKeyDown(KeyCode.Escape);
		}
	}

	public bool PressedCancel() {
		if (activeDevice != null) {
			return activeDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Escape);
		} else {
			return Input.GetKeyDown(KeyCode.Escape);
		}
		
	}

	public bool PressedStartDialogue() {
		if(activeDevice != null) {
			return activeDevice.Action4.WasPressed || Input.GetKeyDown(startDialogueKey);
		} else {
			return Input.GetKeyDown(startDialogueKey);
		}
	}

	public bool PressedDeleteProfile() {
		if(activeDevice != null) {
			return activeDevice.Action3.WasPressed || Input.GetKeyDown(KeyCode.Delete);
		} else {
			return Input.GetKeyDown(KeyCode.Delete);
		}
	}

	public bool PressedToPetTheDog() {
		if(activeDevice != null) {
			return activeDevice.Action3.WasPressed || Input.GetKeyDown(petTheDogKey);
		} else {
			return Input.GetKeyDown(petTheDogKey);
		}
	}

	// maybe in the future pet and stop petting should be the same key
	public bool StopPettingTheDog() {
		if(activeDevice != null) {
			return activeDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.T);
		} else {
			return Input.GetKeyDown(KeyCode.T);
		}
	}

	public void Vibrate(float left, float right) {
		activeDevice.Vibrate(left, right);
	}

	public void Vibrate(float amount) {
		activeDevice.Vibrate(amount);
	}

	public void VibrateWithTime(float amount, float time) {
		StartCoroutine(VibrateWithTimeRoutine(amount, time));
	}

	private IEnumerator VibrateWithTimeRoutine(float amount, float time) {
		Vibrate(amount);
		yield return new WaitForSeconds(time);
		Vibrate(0f);
	}
}
