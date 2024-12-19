using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

	// Gera um mapa de falloff de tamanho especificado
	public static float[,] GenerateFalloffMap(int size) {
		float[,] map = new float[size, size];

		// Itera sobre cada posição no mapa
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				// Calcula as coordenadas normalizadas
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				// Calcula o valor de falloff
				float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				map[i, j] = Evaluate(value);
			}
		}

		return map;
	}

	// Avalia a função de falloff para um valor dado
	static float Evaluate(float value) {
		float a = 3;
		float b = 2.2f;

		// Retorna o valor calculado da função de falloff
		return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
	}
}