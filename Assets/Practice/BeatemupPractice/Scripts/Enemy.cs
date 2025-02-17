using UnityEngine;

public class EnemyMovement : Movement
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minHeight, maxHeight;
    [SerializeField] private float damageTime = 0.5f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackRate = 1f;
    
    private Rigidbody _rb;
    private Animator _anim;
    private Transform _groundCheck;
    private Transform _target;
    private bool _isGrounded;
    private bool _isFacingRight;
    private bool _isDead = false;
    private bool _damaged = false;
    private float _zForce;
    private float _walkTimer;
    private float _damageTimer;
    private float _currentSpeed;
    private float _currentHealth;
    private float _nextAttack;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _groundCheck = transform.Find("GroundCheck");
        _target = FindObjectOfType<PlayerMovement>().transform;

        _currentSpeed = maxSpeed;
        _currentHealth = maxHealth;
        ResetSpeed();
    }

    private void Update()
    {
        _isGrounded = Physics.Linecast(transform.position, _groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        _anim.SetBool("Grounded", _isGrounded);
        _anim.SetBool("Dead", _isDead);
        
        _isFacingRight = (!(_target.position.x < transform.position.x));
        if (_isFacingRight)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (_damaged && !_isDead)
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= damageTime)
            {
                _damaged = false;
                _damageTimer = 0;
            }
        }
        
        _walkTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        
        Vector3 targetDistance = _target.position - transform.position;
        float hForce = targetDistance.x / Mathf.Abs(targetDistance.x);

        if (_walkTimer >= Random.Range(1f, 2f))
        {
            _zForce = Random.Range(-1, 2);
            _walkTimer = 0;
        }

        if (Mathf.Abs(targetDistance.x) < 1.5f)
        {
            hForce = 0;
        }

        _rb.linearVelocity = new Vector3(hForce * _currentSpeed, 0, _zForce * _currentSpeed);
        
        _anim.SetFloat("Speed", Mathf.Abs(_currentSpeed));

        if (Mathf.Abs(targetDistance.x) < 1.5f && Mathf.Abs(targetDistance.z) < 1.5f && Time.time > _nextAttack)
        {
            _anim.SetTrigger("Attack");
            _currentSpeed = 0;
            _nextAttack = Time.time + attackRate;
        }
        
        _rb.position = new Vector3(_rb.position.x, _rb.position.y, Mathf.Clamp(_rb.position.z, minHeight + 1, maxHeight - 1));
    }

    public void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    private void ResetSpeed()
    {
        _currentSpeed = maxSpeed;
    }
}
