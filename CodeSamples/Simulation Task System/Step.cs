using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Scenarios
{
	public class Step : SimulationBlock
	{
		[SerializeField] private ProcessFlowType processFlowType = default;
		public ProcessFlowType ProcessFlowType { get { return processFlowType; } }

		public override IObservable<Unit> ProcessStartedStream => processFlowController.ProcessStartedStream;
		public override IObservable<float> ProgressUpdateStream => processFlowController.ProgressUpdateStream;
		public override IObservable<IRaport> ProcessCompletedStream => processFlowController.ProcessCompletedStream.Select(CompleteProcess);
		public override IObservable<IEnumerable<SimulationBlock>> BlockChainStream => subBlocks.Select(subBlock => subBlock.BlockChainStream).Merge().Select(AppendToChain);
		public override bool Started => started;
		public override bool Completed => completed;
		public override bool CompletedCorrectly => completedCorrectly;
		public override float Progress => progress;

		private IFlowController processFlowController;
		private List<SimulationBlock> subBlocks = new List<SimulationBlock>();
		private bool started;
		private bool completed;
		private bool completedCorrectly;
		private float progress;

		private IAddOn[] addons;

		public override void Build()
		{
			GetAddOns();

			subBlocks = new List<SimulationBlock>();
			foreach (Transform child in transform)
			{
				SimulationBlock block;
				if (child.gameObject.TryGetComponent(out block))
				{
					subBlocks.Add(block);
					block.Build();
				}
			}
			processFlowController = FlowControllerFactory.GetFlowController(ProcessFlowType, subBlocks.ToArray());

			ProcessStartedStream.Subscribe(_ => started = true);
			ProgressUpdateStream.Subscribe(x => progress = x);
			ProcessCompletedStream.Subscribe(raport => { completed = true; completedCorrectly = raport.CompletedCorrectly; });
		}
		private void GetAddOns()
		{
			addons = GetComponents<AddOn>();
		}

		public override void SetupProcess()
		{
			SetupAddOns();
			processFlowController.SetupProcess();
		}
		private void SetupAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.SetupAddOn();
			}
		}

		public override void StartProcess()
		{
			StartAddOns();
			processFlowController.StartProcess();
		}
		private void StartAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.StartAddOn();
			}
		}

		public override void LateUpdateProcess()
		{
			processFlowController.LateUpdateProcess();
		}
		public override void UpdateProcess()
		{
			UpdateAddOns();
			processFlowController.UpdateProcess();
		}
		private void UpdateAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.UpdateAddOn(progress);
			}
		}

		public override void ForceCompleteProcess(bool completedCorrectly)
		{
			ForceStopAddOns();
			processFlowController.ForceCompleteProcess(completedCorrectly);
		}
		private void ForceStopAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.ForceStopAddOn();
			}
		}

		public override IEnumerable<SimulationBlock> GetSubBlocks()
		{
			return subBlocks;
		}
		public override IEnumerable<Task> CollectTasks()
		{
			return subBlocks.SelectMany(subBlock => subBlock.CollectTasks());
		}
		public override bool TryFindBlock(SimulationBlock block, ref List<SimulationBlock> blockChain)
		{
			for (int i = 0; i < subBlocks.Count; i++)
			{
				if (block == this)
				{
					blockChain.Insert(0, this);
					return true;
				}
				if (subBlocks[i].TryFindBlock(block, ref blockChain))
				{
					blockChain.Insert(0, this);
					return true;
				}
			}
			return false;
		}

		private IRaport CompleteProcess(IRaport[] subRaports)
		{
			StopAddOns();
			return PrepareRaport(subRaports);
		}
		private void StopAddOns()
		{
			foreach (IAddOn addOn in addons)
			{
				addOn.StopAddOn();
			}
		}
		private IRaport PrepareRaport(IRaport[] subRaports)
		{
			return new StepRaport(subRaports);
		}

		private IEnumerable<SimulationBlock> AppendToChain(IEnumerable<SimulationBlock> blockChain)
		{
			if (processFlowType == ProcessFlowType.Parallel) return new SimulationBlock[] { this };
			else return blockChain.Prepend(this);
		}
	}
}

