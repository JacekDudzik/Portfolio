using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DrivingSystem : MonoBehaviour
{
	[SerializeField] private Transform leftFrontWheel = default;
	[SerializeField] private Transform rightFrontWheel = default;
	[SerializeField] private Transform leftBackWheel = default;
	[SerializeField] private Transform rightBackWheel = default;
	[SerializeField] private float wheelRadius = default;
	[SerializeField] private float suspensionHeight = default;
	[SerializeField] private float suspensionStiffness = 100;
	[SerializeField] private float forwardBackwardForce = 100;
	[SerializeField] private float turningForce = 100;
	[SerializeField] private LayerMask groundMask = 0xFF;
	[SerializeField] private Vector3 centerOfMassOffset = default;

	[Space, SerializeField] private float testForce;

	private float MaxDistanceToGround => wheelRadius + suspensionHeight;

	private SuspensionPoint[] suspensionPoints;
	private new Rigidbody rigidbody;
	private bool grounded;

	private void Awake()
	{
		if (this.TryGetComponent(out rigidbody))
			rigidbody.centerOfMass = centerOfMassOffset;
		suspensionPoints = new SuspensionPoint[]
		{
			new SuspensionPoint(leftFrontWheel),
			new SuspensionPoint(rightFrontWheel),
			new SuspensionPoint(leftBackWheel),
			new SuspensionPoint(rightBackWheel)
		};
	}
	private void Update()
	{
		if (Input.GetKey(KeyCode.W)) ApplyForwardBackwardForce(forwardBackwardForce);
		if (Input.GetKey(KeyCode.S)) ApplyForwardBackwardForce(-forwardBackwardForce);
	}

	private void FixedUpdate()
	{
		UpdateSuspenionPoints();
		ApplySuspensionForces();
		ApplyRotationForce();
	}
	private void UpdateSuspenionPoints()
	{
		suspensionPoints.ForEach(suspensionPoint =>
		{
			Ray ray = new Ray(suspensionPoint.Transform.position, -suspensionPoint.Transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, MaxDistanceToGround, groundMask))
				suspensionPoint.Update(true, 1 - hitInfo.distance / MaxDistanceToGround, hitInfo.point, hitInfo.normal);
			else
				suspensionPoint.Update(false, 0, suspensionPoint.Transform.position, suspensionPoint.Transform.up);
		});

		grounded = suspensionPoints.Any(suspensionPoint => suspensionPoint.Grounded);
		rigidbody.drag = grounded ? 3 : 0;
	}

	private void ApplySuspensionForces()
	{
		suspensionPoints.ForEach(ApplySuspensionForce);
	}
	private void ApplySuspensionForce(SuspensionPoint suspensionPoint)
	{
		float force = suspensionPoint.CompressionRatio * suspensionStiffness;
		rigidbody.AddForceAtPosition(suspensionPoint.Transform.up * force, suspensionPoint.Transform.position, ForceMode.Force);
	}

	private void ApplyForwardBackwardForce(float force)
	{
		suspensionPoints
			.Where(suspensionPoint => suspensionPoint.Grounded)
			.ForEach(suspensionPoint => rigidbody.AddForceAtPosition(Vector3.ProjectOnPlane(suspensionPoint.Transform.forward, Vector3.up) * force, suspensionPoint.Transform.position));
	}
	private void ApplyRotationForce()
	{
		float turn = Input.GetAxis("Horizontal");
		rigidbody.AddTorque(transform.up * turningForce * turn * rigidbody.velocity.magnitude);
	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawRay(leftFrontWheel.position, -leftFrontWheel.up * MaxDistanceToGround);
		Gizmos.DrawRay(rightFrontWheel.position, -leftFrontWheel.up * MaxDistanceToGround);
		Gizmos.DrawRay(leftBackWheel.position, -leftBackWheel.up * MaxDistanceToGround);
		Gizmos.DrawRay(rightBackWheel.position, -rightBackWheel.up * MaxDistanceToGround);

		Gizmos.DrawWireSphere(transform.position + centerOfMassOffset, .3f);
	}

	private class SuspensionPoint
	{
		public Transform Transform { get; private set; }
		public float CompressionRatio { get; private set; }
		public Vector3 ImpactPoint { get; private set; }
		public Vector3 ImpactNormal { get; private set; }
		public bool Grounded { get; private set; }

		public SuspensionPoint(Transform transform)
		{
			Transform = transform;
		}
		public void Update(bool grounded, float compressionRatio, Vector3 impactPoint, Vector3 impactNormal)
		{
			CompressionRatio = compressionRatio;
			ImpactPoint = impactPoint;
			ImpactNormal = ImpactNormal;
			Grounded = grounded;
		}
	}
}
