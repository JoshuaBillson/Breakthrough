using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour {

	[SerializeField] private GameObject UIParent;
	[SerializeField] private Button restartButton;
	[SerializeField] private TextMeshProUGUI finishText;

	internal void HideUI() {
		UIParent.SetActive(false);
	}

	internal void OnGameFinished(string winner) {
		UIParent.SetActive(true);
		finishText.text = string.Format("{0} is the Winner!", winner);
	}
}
