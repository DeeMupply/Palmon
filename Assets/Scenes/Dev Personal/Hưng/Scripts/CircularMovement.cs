using UnityEngine;

public class CircularMovement3D : MonoBehaviour
{
    public Transform centerPoint;
    public float radius = 3f;
    public float speed = 2f;

    private float angle;
    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        angle += speed * Time.deltaTime;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        Vector3 newPosition = centerPoint.position + new Vector3(x, 0, z);
        transform.position = newPosition;

        Vector3 direction = newPosition - previousPosition;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        previousPosition = newPosition;
    }
}