using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelector : MonoBehaviour
{
	[SerializeField] private Material freeSquareMaterial;
	[SerializeField] private Material enemySquareMaterial;
	[SerializeField] private GameObject selectorPrefab;
	private List<GameObject> instantiatedSelectors = new List<GameObject>();

	public void ShowSelection(Dictionary<Vector3, bool> squareData) {

        /* Delete Existing Selectors */
		ClearSelection();

        /* Set Square Selecter Material for Each Square */
		foreach (var data in squareData) {
			GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity);
			instantiatedSelectors.Add(selector);
			foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>()) {
				setter.SetSingleMaterial(data.Value ? freeSquareMaterial : enemySquareMaterial);
			}
		}
	}

    /* Destroy Selector Objects */
	public void ClearSelection() {
		for (int i = 0; i < instantiatedSelectors.Count; i++) {
			Destroy(instantiatedSelectors[i]);
		}
	}
}
