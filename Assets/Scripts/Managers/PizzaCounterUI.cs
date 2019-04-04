using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PizzaCounterUI : MonoBehaviour {

	public static PizzaCounterUI instance;
	public GameObject pizzaCounterObject;
	public TextMeshProUGUI pizzaCounter;

	private RectTransform m_pizzaCounterRectTransform;
	private Vector2 m_onScreenAnchoredPosition;
	private Vector2 m_offScreenAnchoredPosition;
	private Vector3 m_originalScale = new Vector3(1,1,1);
	private Vector3 m_juicyScale = new Vector3(1.05f, 1.05f, 1f);
	private const float m_offsetOnX = 150f;
	private const float m_timeToOffset = 0.75f;
	private const float m_timeToJuice = 0.5f;

	void Awake() {
		if(instance == null) instance = this;
		else Destroy(gameObject);

		m_pizzaCounterRectTransform = pizzaCounterObject.GetComponent<RectTransform>();
		m_onScreenAnchoredPosition = m_pizzaCounterRectTransform.anchoredPosition;
		m_offScreenAnchoredPosition = m_onScreenAnchoredPosition;
		m_offScreenAnchoredPosition.x -= m_offsetOnX;
	}

	void Start() {
		m_pizzaCounterRectTransform.anchoredPosition = m_offScreenAnchoredPosition;	
	}

	public void UpdateCounter(int count) {
		StartCoroutine(UpdateCounterRoutine(count));
	}

	public void UpdateCounterWithoutRoutine(int count) {
		pizzaCounter.text = count.ToString();
	}

	private IEnumerator UpdateCounterRoutine(int count) {
		yield return StartCoroutine(OffsetPizzaCounter(m_pizzaCounterRectTransform.anchoredPosition, m_onScreenAnchoredPosition));

		yield return StartCoroutine(UpdatePizzaCounterText(count));

		StartCoroutine(OffsetPizzaCounter(m_pizzaCounterRectTransform.anchoredPosition, m_offScreenAnchoredPosition));
	}

	private IEnumerator OffsetPizzaCounter(Vector2 initialPosition, Vector2 finalPosition) {
		float timeElapsed = 0f;

		while(timeElapsed < m_timeToOffset) {
			timeElapsed += Time.deltaTime;
			float t = Interpolation.SmootherStep(timeElapsed / m_timeToOffset);
			m_pizzaCounterRectTransform.anchoredPosition = Vector2.Lerp(initialPosition, finalPosition, t);
			yield return null;
		}
	}

	private IEnumerator UpdatePizzaCounterText(int count) {
		float halfJuiceTime = (m_timeToJuice / 2f);
		float timeElapsed = 0f;
		RectTransform m_pizzaRectTransform = m_pizzaCounterRectTransform.GetComponentInChildren<Image>().GetComponent<RectTransform>();

		while(timeElapsed < halfJuiceTime) {
			timeElapsed += Time.deltaTime;
			float t = (timeElapsed / halfJuiceTime);
			m_pizzaRectTransform.localScale = Vector3.Lerp(m_originalScale, m_juicyScale, t);
			yield return null;
		}

		pizzaCounter.text = count.ToString();

		while(timeElapsed < m_timeToJuice) {
			timeElapsed += Time.deltaTime;
			float t = (timeElapsed / m_timeToJuice);
			m_pizzaRectTransform.localScale = Vector3.Lerp(m_juicyScale, m_originalScale, t);
			yield return null;
		}
	}
}
