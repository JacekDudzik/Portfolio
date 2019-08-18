using System;
using UniRx;
using UnityEngine.XR;

public class Handheld : VRInputDevice, IHandheld
{
	public IObservable<float> TriggerStream => triggerSubject.AsObservable();
	public IObservable<float> TriggerPressedTimedStream => Observable.EveryUpdate().SkipUntil(TriggerDownStream).TakeUntil(TriggerUpStream).Select(_ => UnityEngine.Time.deltaTime).Scan((x, y) => x + y).Repeat();
	public IObservable<bool> TriggerPressedStream => TriggerStream.Select(value => value >= triggerPressThreshold);
	public IObservable<Unit> TriggerDownStream => TriggerPressedStream.Pairwise((prev, next) => !prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> TriggerHeldStream => TriggerPressedStream.Pairwise((prev, next) => prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> TriggerUpStream => TriggerPressedStream.Pairwise((prev, next) => prev && !next).Where(x => x).AsUnitObservable();

	public IObservable<float> GripValueSetStream => gripSubject.AsObservable();
	public IObservable<float> GripPressedTimedStream => Observable.EveryUpdate().SkipUntil(GripDownStream).TakeUntil(GripUpStream).Select(_ => UnityEngine.Time.deltaTime).Scan((x, y) => x + y).Repeat();
	public IObservable<bool> GripPressedStream => GripValueSetStream.Select(value => value >= gripPressThreshold);
	public IObservable<Unit> GripDownStream => GripPressedStream.Pairwise((prev, next) => !prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> GripHeldStream => GripPressedStream.Pairwise((prev, next) => prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> GripUpStream => GripPressedStream.Pairwise((prev, next) => prev && !next).Where(x => x).AsUnitObservable();

	public IObservable<bool> PrimaryButtonValueSetStream => primaryButtonSubject.AsObservable();
	public IObservable<float> PrimaryButtonPressedTimedStream => Observable.EveryUpdate().SkipUntil(PrimaryButtonDownStream).TakeUntil(PrimaryButtonUpStream).Select(_ => UnityEngine.Time.deltaTime).Scan((x, y) => x + y).Repeat();
	public IObservable<Unit> PrimaryButtonDownStream => PrimaryButtonValueSetStream.Pairwise((prev, next) => !prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> PrimaryButtonHeldStream => PrimaryButtonValueSetStream.Pairwise((prev, next) => prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> PrimaryButtonUpStream => PrimaryButtonValueSetStream.Pairwise((prev, next) => prev && !next).Where(x => x).AsUnitObservable();

	public IObservable<bool> SecondaryButtonValueSetStream => primaryButtonSubject.AsObservable();
	public IObservable<float> SecondaryButtonPressedTimedStream => Observable.EveryUpdate().SkipUntil(SecondaryButtonDownStream).TakeUntil(SecondaryButtonUpStream).Select(_ => UnityEngine.Time.deltaTime).Scan((x, y) => x + y).Repeat();
	public IObservable<Unit> SecondaryButtonDownStream => SecondaryButtonValueSetStream.Pairwise((prev, next) => !prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> SecondaryButtonHeldStream => SecondaryButtonValueSetStream.Pairwise((prev, next) => prev && next).Where(x => x).AsUnitObservable();
	public IObservable<Unit> SecondaryButtonUpStream => SecondaryButtonValueSetStream.Pairwise((prev, next) => prev && !next).Where(x => x).AsUnitObservable();

	private BehaviorSubject<float> triggerSubject = new BehaviorSubject<float>(0f);
	private BehaviorSubject<float> gripSubject = new BehaviorSubject<float>(0f);
	private Subject<bool> primaryButtonSubject = new Subject<bool>();
	private Subject<bool> secondaryButtonSubject = new Subject<bool>();

	private const float triggerPressThreshold = .5f;
	private const float gripPressThreshold = .5f;

	public Handheld() : base() { }
	public override void UpdateConnected()
	{
		base.UpdateConnected();
		UpdateFeaturesValues();
	}

	private void UpdateFeaturesValues()
	{
		if (device.TryGetFeatureValue(CommonUsages.trigger, out float trigger)) triggerSubject.OnNext(trigger);
		if (device.TryGetFeatureValue(CommonUsages.grip, out float grip)) gripSubject.OnNext(grip);
		if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton)) primaryButtonSubject.OnNext(primaryButton);
		if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton)) secondaryButtonSubject.OnNext(secondaryButton);
	}
}