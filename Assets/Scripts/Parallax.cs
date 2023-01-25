using UnityEngine;

public class Parallax : MonoBehaviour {

	private float _length;
	private float _startPos;
	public float parallaxEffect;
	public Camera cam;
	
	private void Start() {
		_startPos = transform.position.x;
		_length = GetComponent<SpriteRenderer>().bounds.size.x;
	}

	private void FixedUpdate() {
		var temp = cam.transform.position.x * (1 - parallaxEffect);
		var dist = cam.transform.position.x * parallaxEffect;

		transform.position = new Vector3(_startPos + dist, transform.position.y, transform.position.z);

		if (temp > _startPos + _length) _startPos += _length;
		else if (temp < _startPos - _length) _startPos -= _length;
	}
}