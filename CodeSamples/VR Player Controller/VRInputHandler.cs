using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

public class VRInputHandler : IVRInputProvider
{
	public IObservable<Unit> AnyTriggerDownStream => Observable.Merge(LeftHandNode.TriggerDownStream, RightHandNode.TriggerDownStream);
	public IObservable<Unit> AnyTriggerUpStream => Observable.Merge(LeftHandNode.TriggerUpStream, RightHandNode.TriggerUpStream);
	public IObservable<Unit> AnyTriggerHeldStream => Observable.Merge(LeftHandNode.TriggerHeldStream, RightHandNode.TriggerHeldStream);

	public IObservable<Unit> AnyGripDownStream => Observable.Merge(LeftHandNode.GripDownStream, RightHandNode.GripDownStream);
	public IObservable<Unit> AnyGripUpStream => Observable.Merge(LeftHandNode.GripUpStream, RightHandNode.GripUpStream);
	public IObservable<Unit> AnyGripHeldStream => Observable.Merge(LeftHandNode.GripHeldStream, RightHandNode.GripHeldStream);

	public IObservable<Unit> AnyPrimaryButtonDownStream => Observable.Merge(LeftHandNode.PrimaryButtonDownStream, RightHandNode.PrimaryButtonDownStream);
	public IObservable<Unit> AnyPrimaryButtonHeldStream => Observable.Merge(LeftHandNode.PrimaryButtonHeldStream, RightHandNode.PrimaryButtonHeldStream);
	public IObservable<Unit> AnyPrimaryButtonUpStream => Observable.Merge(LeftHandNode.PrimaryButtonUpStream, RightHandNode.PrimaryButtonUpStream);

	public IObservable<Unit> AnySecondaryButtonDownStream => Observable.Merge(LeftHandNode.SecondaryButtonDownStream, RightHandNode.SecondaryButtonDownStream);
	public IObservable<Unit> AnySecondaryButtonHeldStream => Observable.Merge(LeftHandNode.SecondaryButtonHeldStream, RightHandNode.SecondaryButtonHeldStream);
	public IObservable<Unit> AnySecondaryButtonUpStream => Observable.Merge(LeftHandNode.SecondaryButtonUpStream, RightHandNode.SecondaryButtonUpStream);

	public Headset HeadsetNode { get; private set; }
	public Handheld LeftHandNode { get; private set; }
	public Handheld RightHandNode { get; private set; }

	IVRInputDevice IVRInputProvider.HeadsetNode => HeadsetNode;
	IHandheld IVRInputProvider.LeftHandNode => LeftHandNode;
	IHandheld IVRInputProvider.RightHandNode => RightHandNode;

	private List<InputDevice> devices = new List<InputDevice>();
	private List<InputDevice> _devices = new List<InputDevice>();

	public VRInputHandler()
	{
		HeadsetNode = new Headset();
		LeftHandNode = new Handheld();
		RightHandNode = new Handheld();

		UpdateDeviceList();
	}
	public void Update()
	{
		UpdateDeviceList();
	}

	private void UpdateDeviceList()
	{
		_devices.Clear();
		_devices.AddRange(devices);
		InputDevices.GetDevices(devices);

		devices.Except(_devices).ToList().ForEach(HandleNewDevice);
		devices.Union(_devices).ToList().ForEach(HandleDeviceUpdate);
		_devices.Except(devices).ToList().ForEach(HandleDeviceRemoved);
	}

	private void HandleNewDevice(InputDevice device)
	{
		switch (device.role)
		{
			case InputDeviceRole.Generic: HeadsetNode.OnConnect(device); break;
			case InputDeviceRole.LeftHanded: LeftHandNode.OnConnect(device); break;
			case InputDeviceRole.RightHanded: RightHandNode.OnConnect(device); break;
			default: break;
		}
	}
	private void HandleDeviceUpdate(InputDevice device)
	{
		switch (device.role)
		{
			case InputDeviceRole.Generic: HeadsetNode.UpdateConnected(); break;
			case InputDeviceRole.LeftHanded: LeftHandNode.UpdateConnected(); break;
			case InputDeviceRole.RightHanded: RightHandNode.UpdateConnected(); break;
			default: break;
		}
	}
	private void HandleDeviceRemoved(InputDevice device)
	{
		switch (device.role)
		{
			case InputDeviceRole.Generic: HeadsetNode.OnDisconnect(); break;
			case InputDeviceRole.LeftHanded: LeftHandNode.OnDisconnect(); break;
			case InputDeviceRole.RightHanded: RightHandNode.OnDisconnect(); break;
			default: break;
		}
	}
}
