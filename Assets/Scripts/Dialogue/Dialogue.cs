using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {
	public string characterName;
	public Sprite characterSprite;
	
	[TextArea(3, 10)]
	public string[] sentences;
	public AudioClip dialogueClip;

	public Dialogue(string characterName, Sprite characterSprite, string[] sentences) {
		this.characterName = characterName;
		this.characterSprite = characterSprite;
		this.sentences = sentences;
	}

	public Dialogue(string characterName, Sprite characterSprite, string sentence) {
		this.characterName = characterName;
		this.characterSprite = characterSprite;
		this.sentences = new string[]{ sentence };
	}

	public Dialogue(string characterName, Sprite characterSprite, string sentence, AudioClip clip) {
		this.characterName = characterName;
		this.characterSprite = characterSprite;
		this.sentences = new string[]{ sentence };
		this.dialogueClip = clip;
	}
}
