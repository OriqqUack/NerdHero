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
	private Camera cam;
	private float initialZoomSize;

	private void Start()
	{
		// Setting up the reference.
		_player = WaveManager.Instance.PlayerTransform;
		cam = GetComponent<Camera>();
		if (cam == null)
			cam = Camera.main;

		// 초기 줌 크기 설정
		initialZoomSize = cam.orthographicSize;
		targetZoomSize = initialZoomSize;
	}
	
	private void Update()
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
		ZoomTo(cam.orthographicSize - amount);
	}

	public void ZoomOut(float amount)
	{
		ZoomTo(cam.orthographicSize + amount);
	}

	public void ZoomInit()
	{
		ZoomTo(initialZoomSize);
	}

	private void HandleZoom()
	{
		if (cam != null)
		{
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoomSize, Time.unscaledDeltaTime * zoomSmooth);
		}
	}
	
	private bool CheckXMargin()
	{
		return Mathf.Abs(transform.position.x - _player.position.x) > xMargin;
	}

	private void TrackPlayer()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;

		// If the player has moved beyond the x margin...
		if (CheckXMargin())
		{
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
			targetX = Mathf.Lerp(transform.position.x, _player.position.x, xSmooth * Time.deltaTime);
		}

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
		targetX = Mathf.Clamp(targetX, minXAndY.x, maxXAndY.x);
		//targetY = Mathf.Clamp(targetY, minXAndY.y, maxXAndY.y);

		// Set the camera's position to the target position with the same z component.
		transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
	}
}
