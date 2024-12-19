using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester : MonoBehaviour {

	static ThreadedDataRequester instance;
	Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

	// Inicializa a instância
	void Awake() {
		instance = FindObjectOfType<ThreadedDataRequester>();
	}

	// Solicita dados em uma nova thread
	public static void RequestData(Func<object> generateData, Action<object> callback) {
		ThreadStart threadStart = delegate {
			instance.DataThread(generateData, callback);
		};

		new Thread(threadStart).Start();
	}

	// Executa a geração de dados em uma thread separada
	void DataThread(Func<object> generateData, Action<object> callback) {
		object data = generateData();
		lock (dataQueue) {
			dataQueue.Enqueue(new ThreadInfo(callback, data));
		}
	}

	// Atualiza a fila de dados no thread principal
	void Update() {
		if (dataQueue.Count > 0) {
			for (int i = 0; i < dataQueue.Count; i++) {
				ThreadInfo threadInfo = dataQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	// Estrutura para armazenar informações de thread
	struct ThreadInfo {
		public readonly Action<object> callback;
		public readonly object parameter;

		public ThreadInfo(Action<object> callback, object parameter) {
			this.callback = callback;
			this.parameter = parameter;
		}
	}
}
