using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 100f;

    private void Update()
    {
        float move = Input.GetAxis("Vertical");   // W/S o ↑/↓
        float turn = Input.GetAxis("Horizontal"); // A/D o ←/→

        // Movimiento hacia adelante o atrás
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);

        // Rotación sobre el eje Y
        transform.Rotate(Vector3.up * turn * turnSpeed * Time.deltaTime);
    }
}
