using UnityEngine;
using System.Collections;

public class HideOnPlay : MonoBehaviour {

	// Usado para inicialização
	void Start() {
		// Desativa o objeto quando o jogo começa
		gameObject.SetActive(false);
	}
	
	// Atualização é chamada uma vez por frame
	void Update() {
		// Nenhuma operação necessária na atualização
	}
}