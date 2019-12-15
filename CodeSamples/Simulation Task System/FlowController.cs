using System;
using System.Linq;
using UniRx;
using UniRx.Diagnostics;

namespace Scenarios
{
	public abstract class FlowController : IFlowController
	{
		public IObservable<Unit> ProcessStartedStream => processStartedSubject.AsObservable();
		public IObservable<float> ProgressUpdateStream => Observable.Merge(subProcesses.Select(subProcess => subProcess.ProgressUpdateStream)).Select(_ => CalculateTotalProgress());
		public IObservable<IRaport[]> ProcessCompletedStream => processCompletedSubject.AsObservable();
		public abstract IObservable<SimulationBlock> SubprocessStartedStream { get; }

		public bool Completed { get; private set; }
		public bool Started { get; private set; }
		public float Progress { get; private set; }

		protected SimulationBlock[] subProcesses;

		private Subject<Unit> processStartedSubject = new Subject<Unit>();
		private Subject<IRaport[]> processCompletedSubject = new Subject<IRaport[]>();

		public void SetupProcess()
		{
			subProcesses.ForEach(subProcess => subProcess.SetupProcess());
			subProcesses.Select(subProcess => subProcess.ProcessCompletedStream).Merge().Last().ToArray().Subscribe(OnComplete);
		}
		public virtual void StartProcess()
		{
			Started = true;
			processStartedSubject.OnNext(Unit.Default);
			processStartedSubject.OnCompleted();
		}
		public virtual void UpdateProcess() { }
		public virtual void LateUpdateProcess()
		{
			Progress = subProcesses.Sum(subProcess => subProcess.Progress) / subProcesses.Count();
		}

		protected virtual void OnComplete(IRaport[] raports)
		{
			Completed = true;
			processCompletedSubject.OnNext(raports);
			processCompletedSubject.OnCompleted();
		}

		public void ForceCompleteProcess(bool completedCorrectly)
		{
			subProcesses
				.Where(subProcess => !subProcess.Completed)
				.ToList()
				.ForEach(subProcess => subProcess.ForceCompleteProcess(completedCorrectly));
		}

		private float CalculateTotalProgress()
		{
			return subProcesses.Sum(subProcess => subProcess.Progress) / subProcesses.Count();
		}
	}
}
