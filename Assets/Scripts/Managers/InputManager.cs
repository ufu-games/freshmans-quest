using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PS4: Sony Interactive Entertainment Wireless Controller
// SWITCH: Unknown Pro Controller
public abstract class BaseInput {
	public abstract bool PressedJump();
	public abstract bool ReleasedJump();
	public abstract bool PressingWallJump();
}

public class PS4Input : BaseInput {
	public override bool PressedJump() {
		return(
			Input.GetKeyDown(KeyCode.Joystick1Button0) ||
			Input.GetKeyDown(KeyCode.Joystick1Button1) ||
			Input.GetKeyDown(KeyCode.Joystick1Button2) ||
			Input.GetKeyDown(KeyCode.Joystick1Button3)
		);
	}

	public override bool ReleasedJump() {
		return(
			Input.GetKeyUp(KeyCode.Joystick1Button0) ||
			Input.GetKeyUp(KeyCode.Joystick1Button1) ||
			Input.GetKeyUp(KeyCode.Joystick1Button2) ||
			Input.GetKeyUp(KeyCode.Joystick1Button3)
		);
	}

	public override bool PressingWallJump() {
		return(
			Input.GetKey(KeyCode.Joystick1Button4) ||
			Input.GetKey(KeyCode.Joystick1Button5) ||
			Input.GetKey(KeyCode.Joystick1Button6) ||
			Input.GetKey(KeyCode.Joystick1Button7)
		);
	}
}

public class SwitchProInput : BaseInput {
	public override bool PressedJump() {
		return(
			Input.GetKeyDown(KeyCode.Joystick1Button0) ||
			Input.GetKeyDown(KeyCode.Joystick1Button1) ||
			Input.GetKeyDown(KeyCode.Joystick1Button2) ||
			Input.GetKeyDown(KeyCode.Joystick1Button3)
		);
	}

	public override bool ReleasedJump() {
		return(
			Input.GetKeyUp(KeyCode.Joystick1Button0) ||
			Input.GetKeyUp(KeyCode.Joystick1Button1) ||
			Input.GetKeyUp(KeyCode.Joystick1Button2) ||
			Input.GetKeyUp(KeyCode.Joystick1Button3)
		);

		
	}

	public override bool PressingWallJump() {
		return(
			Input.GetKey(KeyCode.Joystick1Button4) ||
			Input.GetKey(KeyCode.Joystick1Button5) ||
			Input.GetKey(KeyCode.Joystick1Button6) ||
			Input.GetKey(KeyCode.Joystick1Button7)
		);
	}
}

public class StandardInput : BaseInput {
	public override bool PressedJump() {
		return Input.GetButtonDown("Jump");
	}

	public override bool ReleasedJump() {
		return Input.GetButtonUp("Jump");
	}

	public override bool PressingWallJump() {
		return Input.GetButton("StickToWall");
	}
}

public class InputManager : MonoBehaviour {

	public static InputManager instance;

	public void WhatIsPressed() {
		if(Input.GetKeyDown(KeyCode.JoystickButton0)) {
			Debug.Log("Joystick Button 0");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton1)) {
			Debug.Log("Joystick Button 1");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton2)) {
			Debug.Log("Joystick Button 2");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton3)) {
			Debug.Log("Joystick Button 3");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton4)) {
			Debug.Log("Joystick Button 4");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton5)) {
			Debug.Log("Joystick Button 5");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton6)) {
			Debug.Log("Joystick Button 6");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton7)) {
			Debug.Log("Joystick Button 7");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton8)) {
			Debug.Log("Joystick Button 8");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton9)) {
			Debug.Log("Joystick Button 9");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton10)) {
			Debug.Log("Joystick Button 10");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton11)) {
			Debug.Log("Joystick Button 11");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton12)) {
			Debug.Log("Joystick Button 12");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton13)) {
			Debug.Log("Joystick Button 13");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton14)) {
			Debug.Log("Joystick Button 14");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton15)) {
			Debug.Log("Joystick Button 15");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton16)) {
			Debug.Log("Joystick Button 16");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton17)) {
			Debug.Log("Joystick Button 17");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton18)) {
			Debug.Log("Joystick Button 18");
		} else if(Input.GetKeyDown(KeyCode.JoystickButton19)) {
			Debug.Log("Joystick Button 19");
		}
	}

	public enum EInputDevice {
		PS4_controller,
		SWITCH_controller,
		XBOX_controller,
		none
	}

	private EInputDevice m_activeDevice;
	private BaseInput m_inputDevice;

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}	

	void Start () {
		string[] names= Input.GetJoystickNames();
		foreach(string name in names) {
			Debug.Log("Connected Controller: " + name);
			if(name == "Unknown Pro Controller") {
				Debug.Log("Using Switch Pro Controller");
				m_activeDevice = EInputDevice.SWITCH_controller;
				m_inputDevice = new SwitchProInput();
			} else if (name == "Sony Interactive Entertainment Wireless Controller") {
				Debug.Log("Using PS4 Controller");
				m_activeDevice = EInputDevice.PS4_controller;
				m_inputDevice = new PS4Input();
			} else {
				Debug.Log("Using Standard Input");
				m_activeDevice = EInputDevice.none;
				m_inputDevice = new StandardInput();
			}
		} 	
	}

	public bool PressedJump() {
		return m_inputDevice.PressedJump();
	}

	public bool ReleasedJump() {
		return m_inputDevice.ReleasedJump();
	}

	public bool PressedWallJump() {
		return m_inputDevice.PressingWallJump();
	}
}
