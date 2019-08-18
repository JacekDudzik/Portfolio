using Scenarios.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Scenarios
{
	public abstract class Task : SimulationBlock
	{
		[SerializeField] private bool holdProgress = true;
		[SerializeField] private float progressDropSpeed = 0;

		public override IObservable<Unit> ProcessStartedStream => processStartedSubject.AsObservable();
		public override IObservable<IRaport> ProcessCompletedStream => processCompletedSubject.AsObservable();
		public override IObservable<float> ProgressUpdateStream => progressUpdateSubject.AsObservable();
		public override IObservable<IEnumerable<SimulationBlock>> BlockChainStream => blockChainSubject.AsObservable();
		public override bool Started => started;
		public override bool Completed => completed;
		public override bool CompletedCorrectly => completedCorrectly;
		public override float Progress => progress;
		public bool HoldProgress { get { return holdProgress; } }

		private Subject<Unit> processStartedSubject = new Subject<Unit>();
		private Subject<float> progressUpdateSubject = new Subject<float>();
		private Subject<IRaport> processCompletedSubject = new Subject<IRaport>();
		private Subject<IEnumerable<SimulationBlock>> blockChainSubject = new Subject<IEnumerable<SimulationBlock>>();
		private bool started;
		private bool completed;
		private bool completedCorrectly;
		private float progress;

		private IAddOn[] addons;
		private IRequirement[] requirements;
		private IBreakdownPoint[] breakdownPoints;
		private ISimulationEvent[] simulationEvents;
		private IFailCondition[] failConditions;

		public IBreakdownPoint[] GetAvaibleBreakdownPoints()
		{
			GetBreakdownPoints();
			return breakdownPoints;
		}
		public override void Build()
		{
			GetAddOns();
			GetRequirements();
			GetBreakdownPoints();
			GetEvents();
			GetFailConditions();
		}
		private void GetAddOns()
		{
			addons = GetComponents<IAddOn>();
		}
		private void GetRequirements()
		{
			requirements = GetComponents<Requirement>();
		}
		private void GetBreakdownPoints()
		{
			breakdownPoints = GetComponents<BreakdownPoint>();
		}
		private void GetEvents()
		{
			simulationEvents = GetComponents<SimulationEvent>();
		}
		private void GetFailConditions()
		{
			failConditions = GetComponents<IFailCondition>();
		}

		public override void SetupProcess()
		{
			SetupAddOns();
			SetupBreakdownPoints();
			SetupFailConditions();
		}
		private void SetupAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.SetupAddOn();
			}
		}
		private void SetupBreakdownPoints()
		{
			if (breakdownPoints == null) return;
			foreach (IBreakdownPoint breakdownPoint in breakdownPoints)
			{
				if (breakdownPoint.IsBroken)
					breakdownPoint.SetupBroken();
				else
					breakdownPoint.SetupDefault();
			}
		}
		private void SetupFailConditions()
		{
			failConditions.ForEach(condition => condition.Setup());
		}

		public override void StartProcess()
		{
			started = true;
			StartAddOns();
			StartBreakdownPoints();
			StartEventsOnStart();
			processStartedSubject.OnNext(Unit.Default);
			processStartedSubject.OnCompleted();
			blockChainSubject.OnNext(new SimulationBlock[] { this });
		}
		private void StartAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.StartAddOn();
			}
		}
		private void StartBreakdownPoints()
		{
			if (breakdownPoints == null) return;
			foreach (IBreakdownPoint breakdownPoint in breakdownPoints)
			{
				if (breakdownPoint.IsBroken)
					breakdownPoint.StartBroken();
				else
					breakdownPoint.StartDefault();
			}
		}
		private void StartEventsOnStart()
		{
			simulationEvents
				.Where(simEvent => simEvent.Behaviour == SimulationEvent.StartBehaviour.OnTaskStart)
				.ForEach(simEvent => simEvent.StartEvent());
		}

		public override void UpdateProcess()
		{
			if (CheckFailConditions()) ForceCompleteProcess(false);

			UpdateAddOns();
			UpdateBreakdownPoints();
		}
		private bool CheckFailConditions()
		{
			bool shouldComplete = false;
			foreach (FailCondition condition in failConditions)
			{
				if (condition.CheckCondition())
				{
					if (condition.CompleteTaskOnFail) shouldComplete = true;

					//	Add message to raport
				}
			}

			return shouldComplete;
		}
		private void UpdateAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.UpdateAddOn(progress);
			}
		}
		private void UpdateBreakdownPoints()
		{
			if (breakdownPoints == null) return;
			foreach (IBreakdownPoint breakdownPoint in breakdownPoints)
			{
				if (breakdownPoint.IsBroken)
					breakdownPoint.UpdateBroken(progress);
				else
					breakdownPoint.UpdateDefault(progress);
			}
		}

		protected void SetProgress(float value, bool ignoreRequirements = false)
		{
			if (CanBeTriggered())
			{
				progress = Mathf.Clamp(value, 0, TriggerConstants.MaxTriggerProgressValue);
				progressUpdateSubject.OnNext(progress);
			}
		}
		protected void IncreaseProgress(float amount)
		{
			SetProgress(Mathf.Clamp(progress + amount, 0, TriggerConstants.MaxTriggerProgressValue));
		}
		protected void FullfillProgress()
		{
			SetProgress(TriggerConstants.MaxTriggerProgressValue);
		}
		protected void DecreaseProgress()
		{
			if (!holdProgress)
			{
				SetProgress(Mathf.Clamp(progress - progressDropSpeed * Time.deltaTime, 0, TriggerConstants.MaxTriggerProgressValue), true);
			}
		}

		protected bool CanBeTriggered()
		{
			return requirements.All(requirement => requirement.IsFullfilled());
		}
		protected virtual void OnTrigger()
		{
			StopAddOns();
			StopBreakdownPoints();
			OnComplete(true);
		}

		public override void LateUpdateProcess()
		{
			if (progress == TriggerConstants.MaxTriggerProgressValue)
			{
				OnTrigger();
			}
		}

		public override void ForceCompleteProcess(bool completedCorrectly)
		{
			ForceStopAddOns();
			StopBreakdownPoints();
			OnComplete(completedCorrectly);
		}
		protected virtual void OnComplete(bool completedCorrectly)
		{
			completed = true;
			this.completedCorrectly = completedCorrectly;
			StartEventsOnComplete();
			processCompletedSubject.OnNext(PrepareRaport(completedCorrectly));
			processCompletedSubject.OnCompleted();
			Debug.Log("Task " + Message + " completed");
		}
		private void StopAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.StopAddOn();
			}
		}
		private void StopBreakdownPoints()
		{
			if (breakdownPoints == null) return;
			foreach (IBreakdownPoint breakdownPoint in breakdownPoints)
			{
				if (breakdownPoint.IsBroken)
					breakdownPoint.StopBroken();
				else
					breakdownPoint.StopDefault();
			}
		}
		private void StartEventsOnComplete()
		{
			simulationEvents
				.Where(simEvent => simEvent.Behaviour == SimulationEvent.StartBehaviour.OnTaskComplete)
				.ForEach(simEvent => simEvent.StartEvent());
		}
		private void ForceStopAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.ForceStopAddOn();
			}
		}
		private IRaport PrepareRaport(bool completedCorrectly)
		{
			return new TaskRaport(completedCorrectly);
		}

		public override IEnumerable<Task> CollectTasks()
		{
			return new Task[] { this };
		}
		public override IEnumerable<SimulationBlock> GetSubBlocks()
		{
			return new SimulationBlock[] { this };
		}
		public override bool TryFindBlock(SimulationBlock block, ref List<SimulationBlock> blockChain)
		{
			if (block == this) { blockChain.Insert(0, this); return true; }
			else { return false; }
		}
	}
}