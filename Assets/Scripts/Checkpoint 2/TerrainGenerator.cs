using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 25f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

	public int colliderLODIndex;
	public LODInfo[] detailLevels;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureSettings;

	public Transform viewer;
	public Material mapMaterial;

	public MapGenerator mapGenerator; // Adicione uma referência ao MapGenerator

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	float meshWorldSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	// Inicializa o gerador de terreno
	void Start() {
		textureSettings.ApplyToMaterial(mapMaterial);
		textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		UpdateVisibleChunks();
	}

	// Atualiza o gerador de terreno a cada frame
	void Update() {
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld) {
			foreach (TerrainChunk chunk in visibleTerrainChunks) {
				chunk.UpdateCollisionMesh();
			}
		}

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks();
		}
	}
		
	// Atualiza os chunks visíveis
	void UpdateVisibleChunks() {
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
		for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--) {
			alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
			visibleTerrainChunks[i].UpdateTerrainChunk();
		}
			
		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
					if (terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
						terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					} else {
						// Gere o mapa de cavernas para este chunk
						mapGenerator.GenerateMap();
						int[,] caveMap = mapGenerator.GetMap();

						 // Certifique-se de que o caveMap tenha o mesmo tamanho que o heightMap
						int size = meshSettings.numVertsPerLine;
						if (caveMap.GetLength(0) != size || caveMap.GetLength(1) != size) {
							caveMap = new int[size, size];
						}

						// Crie um novo chunk de terreno com o mapa de cavernas
						TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial, caveMap);
						terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
						newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
						newChunk.Load();
					}
				}
			}
		}
	}

	// Callback chamado quando a visibilidade de um chunk muda
	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
		if (isVisible) {
			visibleTerrainChunks.Add(chunk);
		} else {
			visibleTerrainChunks.Remove(chunk);
		}
	}
}

[System.Serializable]
public struct LODInfo {
	[Range(0, MeshSettings.numSupportedLODs - 1)]
	public int lod;
	public float visibleDstThreshold;

	// Propriedade para obter o quadrado do limite de distância visível
	public float sqrVisibleDstThreshold {
		get {
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}