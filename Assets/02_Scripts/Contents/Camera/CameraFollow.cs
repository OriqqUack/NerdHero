using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	[SerializeField] private float xMargin = 1f;
	[SerializeField] private float xSmooth = 8f;
	[SerializeField] private Vector2 maxXAndY; 
	[SerializeField] private Vector2 minXAndY; 

	private Transform _player;
	
	[Header("Zoom Settings")]
	[SerializeField] private float zoomSmooth = 5f;           // 줌 속도
	[SerializeField] private float targetZoomSize = 5f;       // 목표 orthographic size
	private Camera _cam;
	private float _initialZoomSize;

	private void Start()
	{
		// Setting up the reference.
		_player = WaveManager.Instance.PlayerTransform;
		_cam = GetComponent<Camera>();
		if (_cam == null)
			_cam = Camera.main;

		// 초기 줌 크기 설정
		_initialZoomSize = _cam.orthographicSize;
		targetZoomSize = _initialZoomSize;
	}
	
	private void FixedUpdate()
	{
		TrackPlayer();
		HandleZoom();
	}
	
	public void ZoomTo(float newSize)
	{
		targetZoomSize = Mathf.Clamp(newSize, 1f, 20f); // 적당한 범위 설정
	}

	public void ZoomIn(float amount)
	{
		ZoomTo(_cam.orthographicSize - amount);
	}

	public void ZoomOut(float amount)
	{
		ZoomTo(_cam.orthographicSize + amount);
	}

	public void ZoomInit()
	{
		ZoomTo(_initialZoomSize);
	}

	private void HandleZoom()
	{
		if (_cam != null)
		{
			_cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, targetZoomSize, Time.unscaledDeltaTime * zoomSmooth);
		}
	}
	
	private bool CheckXMargin()
	{
		return Mathf.Abs(transform.position.x - _player.position.x) > xMargin;
	}

	private Vector3 velocity = Vector3.zero;

	private void TrackPlayer()
	{
		if (_player == null)
			return;

		Vector3 targetPos = new Vector3(_player.position.x, transform.position.y, transform.position.z);

		// 마진 체크 (움직일 때만 따라가고 싶으면)
		if (!CheckXMargin())
			targetPos.x = transform.position.x; // xMargin 안 넘었으면 이동 안 함

		// Clamp
		targetPos.x = Mathf.Clamp(targetPos.x, minXAndY.x, maxXAndY.x);

		// SmoothDamp로 부드럽게 이동
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / xSmooth);
	}

}
