using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private float minHeight, maxHeight;
    private float _currentSpeed;
    private Rigidbody _rb;
    private Animator _anim;
    private Transform _groundCheck;
    private bool _onGround;
    private bool _isDead = false;
    private bool _facingRight = true;
    private bool _jump = false;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _groundCheck = transform.Find("GroundCheck");
        _currentSpeed = maxSpeed;
    }

    void Update()
    {
        _onGround = Physics.Linecast(transform.position, _groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        _anim.SetBool("OnGround", _onGround);
        _anim.SetBool("Dead", _isDead);
        
        if (Input.GetButtonDown("Jump") && _onGround)
        {
            _jump = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            _anim.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        
        float h = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (!_onGround)
            z = 0;
        
        _rb.linearVelocity = new Vector3(h * _currentSpeed, _rb.linearVelocity.y, z * _currentSpeed);
        
        if(_onGround)
            _anim.SetFloat("Speed", Mathf.Abs(_rb.linearVelocity.magnitude));
        
        if(h > 0 && !_facingRight)
            Flip();
        else if(h < 0 && _facingRight)
            Flip();

        if (_jump)
        {
            _jump = false;
            _rb.AddForce(Vector3.up * jumpForce);
        }

        float minWidth = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10)).x;
        float maxWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 10)).x;
        _rb.position = new Vector3(Mathf.Clamp(_rb.position.x, minWidth, maxWidth), _rb.position.y, Mathf.Clamp(_rb.position.z, minHeight + 1, maxHeight - 1));
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void ZeroSpeed()
    {
        _currentSpeed = 0;
    }

    private void ResetSpeed()
    {
        _currentSpeed = maxSpeed;
    }
}
