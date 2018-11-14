using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable {


	public enum Option {Aleatorio, Centro};
	public Option OndeTeleportarOJogador = Option.Centro;
	private GameObject Children;
	private GameObject Player;
	[Space(10)]
	[TextArea]
	public string ComoUtilizar = "Coloque o \"Teleporter\" e o \"Teleport Target\" onde você quiser, pode alterar o transform dele como desejar, mas não mexa no Collider. Se o jogador encostar no \"Teleporter\" ele vai pro \"Teleport Target\".\nCaso queira que o jogador seja teleportado para o centro do \"Teleport Target\",\ncoloque \"Centro\",\ncaso queira que seja teleportado aleatoriamente dentro da area vermelha,\ncoloque \"Aleatorio\"";

	void Start(){
		Children = transform.GetChild(0).gameObject;
		Player = GameObject.FindWithTag("Player");
		this.GetComponent<SpriteRenderer>().enabled = false;
		Children.gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}

	public void Interact(){
		if(OndeTeleportarOJogador == Option.Centro) {
			Player.transform.position = Children.transform.position;
		} else {
			Vector2 vect;
			vect.x = Random.Range(Children.transform.position.x - Children.transform.localScale.x/2,Children.transform.position.x + Children.transform.localScale.x/2);
			vect.y = Random.Range(Children.transform.position.y - Children.transform.localScale.y/2,Children.transform.position.y + Children.transform.localScale.y/2);
			Player.transform.position = vect;
		}
	}
}
