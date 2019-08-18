using System;
using System.Linq;
using UniRx;

namespace Scenarios
{
	public class ParallelFlowController : FlowController
	{
		public override IObservable<SimulationBlock> SubprocessStartedStream { get { return subprocessStartedSubject.AsObservable(); } }
		private Subject<SimulationBlock> subprocessStartedSubject = new Subject<SimulationBlock>();

		public ParallelFlowController(SimulationBlock[] subProcesses)
		{
			this.subProcesses = subProcesses;
		}
		public override void StartProcess()
		{
			base.StartProcess();
			foreach (SimulationBlock subProcess in subProcesses)
			{
				subProcess.StartProcess();
				subprocessStartedSubject.OnNext(subProcess);
			}
		}
		public override void UpdateProcess()
		{
			base.UpdateProcess();
			subProcesses
				.Where(subProcess => !subProcess.Completed)
				.ToList()
				.ForEach(subProcess => subProcess.UpdateProcess());
		}
		public override void LateUpdateProcess()
		{
			base.LateUpdateProcess();
			subProcesses
				.Where(subProcess => !subProcess.Completed)
				.ToList()
				.ForEach(subProcess => subProcess.LateUpdateProcess());
		}
	}
}
