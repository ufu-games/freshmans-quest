using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EDialogInputValue
{
    Next = -2,
    Back = -1,
}

public class DialogueManager : MonoBehaviour {
	
	public static DialogueManager instance;

	[Header("UI Elements")]
	public GameObject dialogueObject;
	public Image dialogImage;
	public Text nameText;
	public Text dialogueText;
	public GameObject continueButton;
	
	[Header("Node Editor Integration")]
	public DialogNodeCanvas dialogCanvas;
	private Dictionary<int, DialogNodeCanvas> m_dialogIdTracker;

	[Header("Configuration")]
	[Tooltip("If true, checks for 'SUBMIT' input instead of rendering a Continue button on screen")]
	public bool keyboardInput = true;
	public bool nextSceneWhenDialogueEnds = false;
	[HideInInspector]
	public bool isShowingDialogue;
	


	private Queue<string> m_sentences;
	private Queue<Dialogue> m_dialogues;
	private string m_currentSentenceBeingTyped;
	private bool m_isTypingSentence;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}

		m_dialogIdTracker = new Dictionary<int, DialogNodeCanvas>();
		m_dialogIdTracker.Clear();

		if(dialogCanvas) {
			foreach(int id in dialogCanvas.GetAllDialogId()) {
				m_dialogIdTracker.Add(id, dialogCanvas);
			}
		} else {
			Debug.LogWarning("O Dialogue Manager está presente na cena mas não há DialogNodeCanvas adicionado!");
		}

		m_sentences = new Queue<string>();
		m_dialogues = new Queue<Dialogue>();
		dialogueObject.SetActive(false);
		
		if(keyboardInput && continueButton) {
			continueButton.SetActive(false);
		}
	}

	void Update() {
		if(keyboardInput && Input.GetButtonDown("Submit")) {
			DisplayNextSentence();
		}
	}

	// ==============================================================================
	// ==============================================================================
	// STARTING DIALOGUE WITH THE DIALOGUE SCRIPTABLE OBJECT
	// ==============================================================================
	// ==============================================================================

	public void StartDialogue(Dialogue dialogue) {
		dialogueObject.SetActive(true);
		if(nameText) nameText.text = dialogue.characterName;
		if(this.dialogImage && dialogue.characterSprite) dialogImage.sprite = dialogue.characterSprite;
		m_sentences.Clear();
		if(dialogue.dialogueClip) SoundManager.instance.PlaySfx(dialogue.dialogueClip);

		foreach(string sentence in dialogue.sentences) {
			m_sentences.Enqueue(sentence);
		}

		isShowingDialogue = true;
		DisplayNextSentence();
	}

	public void StartDialogue(Dialogue[] dialogues) {
		
		m_dialogues.Clear();

		foreach(Dialogue dialogue in dialogues) {
			m_dialogues.Enqueue(dialogue);
		}

		StartDialogue(m_dialogues.Dequeue());
	}

	// ==============================================================================
	// ==============================================================================
	// STARTING DIALOGUE WITH THE DIALOGUE CANVAS (NODE EDITOR)
	// ==============================================================================
	// ==============================================================================

	public void StartDialogue(int id) {
		
		if(dialogCanvas == null) {
			Debug.LogError("Objeto DialogNodeCanvas não atribuido ao DialogueManager!!");
			return;
		}

		List<Dialogue> dialogues = new List<Dialogue>();

		DialogNodeCanvas nodeCanvas;
		if(m_dialogIdTracker.TryGetValue(id, out nodeCanvas)) {
			nodeCanvas.ActivateDialog(id, false);
		} else {
			Debug.LogWarning("Não há diálogo com id " + id);
		}
		BaseDialogNode dialogNode = nodeCanvas.GetDialog(id);
		
		// buscando todos os dialogos a partir desse id e colocando os no objeto
		// de dialogos que sera repassado para a cena
		bool fetchingDialogue = true;
		while(fetchingDialogue) {
			if(dialogNode == null) {
				fetchingDialogue = false;
				break;
			} else if(dialogNode is DialogStartNode) {
				dialogues.Add(new Dialogue(dialogNode.CharacterName, dialogNode.CharacterPotrait, dialogNode.DialogLine, dialogNode.SoundDialog));
			} else if(dialogNode is DialogNode) {
				dialogues.Add(new Dialogue(dialogNode.CharacterName, dialogNode.CharacterPotrait, dialogNode.DialogLine, dialogNode.SoundDialog));
			} else if(dialogNode is DialogMultiOptionsNode) {
				Debug.LogError("Dialogo com Opções ainda não implementado!!!");
				fetchingDialogue = false;
				break;
			}

			nodeCanvas.InputToDialog(id, (int)EDialogInputValue.Next);
			dialogNode = nodeCanvas.GetDialog(id);
		}

		Debug.Log("Quantidade de Dialogos: " + dialogues.Count);
		StartDialogue(dialogues.ToArray());
	}
	
	// ==============================================================================
	// ==============================================================================

	public void DisplayNextSentence() {
		if(m_isTypingSentence) {
			StopAllCoroutines();
			dialogueText.text = m_currentSentenceBeingTyped;
			m_isTypingSentence = false;
			return;
		}
		
		if(m_sentences.Count == 0) {
			EndDialog();
			return;
		}

		string sentence = m_sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence) {
		m_isTypingSentence = true;
		m_currentSentenceBeingTyped = sentence;

		dialogueText.text = "";
		foreach(char letter in sentence.ToCharArray()) {
			dialogueText.text += letter;
			yield return null;
		}

		m_isTypingSentence = false;
	}

	public void EndDialog() {
		if(m_dialogues.Count > 0) {
			StartDialogue(m_dialogues.Dequeue());
		} else {
			isShowingDialogue = false;
			dialogueObject.SetActive(false);
			if(nextSceneWhenDialogueEnds) {
				LevelManagement.LevelManager.instance.LoadNextLevel();
			}
		}
	}	
}
