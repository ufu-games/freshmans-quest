using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// PS4: Sony Interactive Entertainment Wireless Controller
// SWITCH: Unknown Pro Controller
public class InputManager : MonoBehaviour {

	public static InputManager instance;
	private InControl.InputDevice activeDevice;
	private InControl.InControlInputModule incontrolInputModule;

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		incontrolInputModule = FindObjectOfType<InControl.InControlInputModule>();
	}

	void Update() {
        activeDevice = InControl.InputManager.ActiveDevice;
		Debug.Log(activeDevice.Name);

		
		// if we are on a UI heavy page...
		if(incontrolInputModule) {
			// activeDevice.Action1.WasPressed = (activeDevice.Action1.WasPressed || Input.GetKeyDown(KeyCode.Return));
		}
    }

	public bool PressedJump() {
		return (activeDevice.Action1.WasPressed || Input.GetKeyDown(KeyCode.Space));
	}

	public bool ReleasedJump() {
		return (activeDevice.Action1.WasReleased || Input.GetKeyUp(KeyCode.Space));
	}

	public bool PressedConfirm() {
		return activeDevice.Action1.WasPressed || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
	}

	public bool PressedStart() {
		return activeDevice.Command.WasPressed || Input.GetKeyDown(KeyCode.Escape);
	}

	public bool PressedCancel() {
		return activeDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Escape);
	}

	public bool PressedStartDialogue() {
		return activeDevice.Action4.WasPressed || Input.GetKeyDown(KeyCode.E);
	}

	public bool PressedDeleteProfile() {
		return activeDevice.Action3.WasPressed || Input.GetKeyDown(KeyCode.Delete);
	}

	public void Vibrate(float left, float right) {
		activeDevice.Vibrate(left, right);
	}

	public void Vibrate(float amount) {
		activeDevice.Vibrate(amount);
	}
}
