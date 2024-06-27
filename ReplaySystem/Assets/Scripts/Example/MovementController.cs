using UnityEngine;

internal class MovementController : MonoBehaviour
{
    [SerializeField] private float _force = 1000;
    [SerializeField] private float _maxVelocity = 8;

    private Rigidbody _rigidbody;

    private Vector3 _inputVector;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _inputVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) _inputVector += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) _inputVector += -Vector3.forward;

        if (Input.GetKey(KeyCode.D)) _inputVector += Vector3.right;
        if (Input.GetKey(KeyCode.A)) _inputVector += -Vector3.right;

        if (_inputVector != Vector3.zero) _rigidbody.AddForce(_force * Time.deltaTime * _inputVector);
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxVelocity);
    }
}
