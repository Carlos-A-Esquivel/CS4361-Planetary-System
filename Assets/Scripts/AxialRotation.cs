using UnityEngine;

public class AxialRotation : MonoBehaviour
{
    [SerializeField] private float rotationPeriodHours = 24f;
    [SerializeField] private float axialTiltDegrees = 23.4f;

    void Start()
    {
        // Tilt the body's axis, then let it spin around that tilted axis
        transform.rotation = Quaternion.Euler(0f, 0f, axialTiltDegrees);
    }

    void Update()
    {
        // 360 degrees per rotation period; SimulationTime scales how fast a "day" passes
        float degreesPerSecond = 360f / rotationPeriodHours;
        transform.Rotate(Vector3.up * degreesPerSecond * SimulationTime.DeltaTime, Space.Self);
    }
}