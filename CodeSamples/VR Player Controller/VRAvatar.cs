using System;
using UniRx;
using UnityEngine;

public class VRAvatar : MonoBehaviour
{
	[SerializeField] protected VRAvatarHead head;
	[SerializeField] protected VRAvatarHand leftHand;
	[SerializeField] protected VRAvatarHand rightHand;
	[SerializeField] protected VRAvatarHandModel defaultHandModel;
	[SerializeField] private float heightAdjustment;

	public IObservable<Unit> PlayerPositionResetStream => playerPositionResetSubject.AsObservable();

	private Subject<Unit> playerPositionResetSubject = new Subject<Unit>();
	protected IVRInputProvider inputProvider;

	public virtual void Initialize(IVRInputProvider inputProvider)
	{
		this.inputProvider = inputProvider;

		leftHand.Initialize(inputProvider.LeftHandNode);
		rightHand.Initialize(inputProvider.RightHandNode);
		head.Initialize(inputProvider.HeadsetNode);
	}
	public virtual void ResetPosition()
	{
		Vector3 offset = GetPlayerOffset();
		offset.y = heightAdjustment;
		head.SetPositionOffset(offset);
		leftHand.SetPositionOffset(offset);
		rightHand.SetPositionOffset(offset);

		playerPositionResetSubject.OnNext(Unit.Default);
	}
	protected virtual Vector3 GetPlayerOffset()
	{
		return -inputProvider.HeadsetNode.Position;
	}

	public void SetDefaultHandModels()
	{
		SetHandModels(defaultHandModel);
	}
	public void SetHandModels(VRAvatarHandModel handModelPrefab)
	{
		leftHand.SetHandModel(handModelPrefab);
		rightHand.SetHandModel(handModelPrefab);
	}
	public VRAvatarHandModel GetDefaultHandModel()
	{
		return defaultHandModel;
	}
}
