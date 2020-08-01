using UnityEngine;

public class SuspensionPoint
{
	public Rigidbody AttachedBody { get; private set; }
	public Transform Transform { get; private set; }
	public Transform Model { get; private set; }

	public float CompressionRatio { get; private set; }
	public Vector3 AttachPoint => Transform.position;
	public Vector3 ImpactPoint { get; private set; }
	public Vector3 ImpactNormal { get; private set; }
	public Vector3 Velocity => AttachedBody.velocity;
	public Vector3 GetForwardVelocity => Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(Velocity, Transform.right), Transform.up);
	public Vector3 GetSideVelocity => Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(Velocity, Transform.forward), Transform.up);
	public Vector3 ForwardOnPlane => Vector3.ProjectOnPlane(Transform.forward, ImpactNormal).normalized;
	public Vector3 PositionDelta { get; private set; }
	public float DistanceTravelledForward { get; private set; }
	public int ForwardDirection { get; private set; }
	public bool Grounded { get; private set; }

	private Vector3 lastPosition;

	public SuspensionPoint(Transform transform, Rigidbody attachedBody, Transform model)
	{
		Transform = transform;
		AttachedBody = attachedBody;
		Model = model;
	}
	public void UpdateCollisionData(bool grounded, float compressionRatio, Vector3 impactPoint, Vector3 impactNormal)
	{
		CompressionRatio = compressionRatio;
		ImpactPoint = impactPoint;
		ImpactNormal = ImpactNormal;
		Grounded = grounded;

	}
	public void Update()
	{
		PositionDelta = ImpactPoint - lastPosition;
		Vector3 travelledForward = Vector3.ProjectOnPlane(
			Vector3.ProjectOnPlane(
				PositionDelta, Transform.up)
				, Transform.right);

		DistanceTravelledForward = travelledForward.magnitude;
		ForwardDirection = Vector3.Dot(travelledForward, ForwardOnPlane) > 0 ? 1 : -1;

		lastPosition = ImpactPoint;
	}
}