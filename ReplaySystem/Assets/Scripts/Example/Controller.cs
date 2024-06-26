using UnityEngine;

internal class Controller : MonoBehaviour
{
    [SerializeField] private float _force = 5;
    [SerializeField] private float _maxVelocity = 5;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _rigidbody.AddForce(_force * Time.deltaTime * Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _rigidbody.AddForce(_force * Time.deltaTime * -Vector3.forward);
        }

        if (Input.GetKey(KeyCode.D))
        {
            _rigidbody.AddForce(_force * Time.deltaTime * Vector3.right);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _rigidbody.AddForce(_force * Time.deltaTime * -Vector3.right);
        }

        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxVelocity);
    }
}
