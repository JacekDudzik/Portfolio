using UnityEngine;

public class TapParticleSystem : MonoBehaviour
{

    public GameObject tapParticle;
    public Canvas canvas;
    [Space]
    public bool randomiseRotation;
    public bool randomiseSize;
    public bool randomisePosition;

    private void Start()
    {
        TouchManager.OnFingerTap += OnFingerTap;
    }

    void OnFingerTap(Finger finger)
    {
        SpawnParticle(finger.Position);
    }

    void SpawnParticle(Vector3 position)
    {
        Quaternion rotation = (randomiseRotation) ? Quaternion.Euler(0, 0, Random.Range(-45, 45)) : Quaternion.identity;
        Vector3 scale = (randomiseSize) ? Vector3.one * Random.Range(.8f, 1.2f) : Vector3.one;
        Vector3 offset = (randomisePosition) ? Random.insideUnitCircle.normalized * 30 : Vector2.zero;

        GameObject particle = Instantiate(tapParticle, position, rotation, canvas.transform);
        particle.transform.localScale = scale;
        particle.transform.position += offset;
    }

    private void OnDestroy()
    {
        TouchManager.OnFingerTap -= OnFingerTap;
    }
}
