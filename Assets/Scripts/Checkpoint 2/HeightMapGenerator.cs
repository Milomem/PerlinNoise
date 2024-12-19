using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {

	// Gera um mapa de altura com base nas configurações fornecidas
	public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
		// Gera um mapa de ruído
		float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

		// Cria uma curva de altura segura para threads
		AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		// Itera sobre cada posição no mapa de altura
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				// Ajusta os valores de altura com base na curva e no multiplicador
				values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

				// Atualiza os valores mínimo e máximo
				if (values[i, j] > maxValue) {
					maxValue = values[i, j];
				}
				if (values[i, j] < minValue) {
					minValue = values[i, j];
				}
			}
		}

		// Retorna o mapa de altura gerado
		return new HeightMap(values, minValue, maxValue);
	}

}

// Estrutura que representa um mapa de altura
public struct HeightMap {
	public readonly float[,] values;
	public readonly float minValue;
	public readonly float maxValue;

	// Construtor para inicializar o mapa de altura
	public HeightMap(float[,] values, float minValue, float maxValue) {
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}