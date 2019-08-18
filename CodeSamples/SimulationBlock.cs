using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Scenarios
{
	public abstract class SimulationBlock : MonoBehaviour, IRaportProcess
	{
		[SerializeField] private string message = default;

		public abstract IObservable<Unit> ProcessStartedStream { get; }
		public abstract IObservable<IRaport> ProcessCompletedStream { get; }
		public abstract IObservable<float> ProgressUpdateStream { get; }
		public abstract IObservable<IEnumerable<SimulationBlock>> BlockChainStream { get; }
		public abstract bool Started { get; }
		public abstract bool Completed { get; }
		public abstract bool CompletedCorrectly { get; }
		public abstract float Progress { get; }
		public string Message => message;

		public abstract void Build();
		public abstract void SetupProcess();
		public abstract void StartProcess();
		public abstract void UpdateProcess();
		public abstract void LateUpdateProcess();
		public abstract void ForceCompleteProcess(bool completedCorrectly);
		public abstract IEnumerable<SimulationBlock> GetSubBlocks();
		public abstract IEnumerable<Task> CollectTasks();
		public abstract bool TryFindBlock(SimulationBlock block, ref List<SimulationBlock> blockChain);
	}
}