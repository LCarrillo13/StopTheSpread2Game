using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAnimation : MonoBehaviour
{
	public float initialY = -720;
	public float finalY = 740;
	public float scrollSpeed = 1;
	public GameObject content;
	bool animateCredits = false;

	public void StartAnimation()
	{
		content.GetComponent<RectTransform>().localPosition = new Vector3(content.GetComponent<RectTransform>().localPosition.x, initialY, content.GetComponent<RectTransform>().localPosition.z);
		animateCredits = true;
	}

	public void ShowWithNoAnimation()
	{
		animateCredits = false;
		content.GetComponent<RectTransform>().localPosition = new Vector3(content.GetComponent<RectTransform>().localPosition.x, 0, content.GetComponent<RectTransform>().localPosition.z);
	}

	private void Update()
	{
		if (!animateCredits)
			return;
		content.GetComponent<RectTransform>().Translate(Vector3.up * Time.deltaTime * scrollSpeed * 40);
		if (content.GetComponent<RectTransform>().localPosition.y > finalY)
			content.GetComponent<RectTransform>().localPosition = new Vector3(content.GetComponent<RectTransform>().localPosition.x, initialY, content.GetComponent<RectTransform>().localPosition.z);
	}
}
