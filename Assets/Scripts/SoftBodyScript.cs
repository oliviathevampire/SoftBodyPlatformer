using UnityEngine;
using UnityEngine.U2D;

public class SoftBodyScript : MonoBehaviour {
	#region Constants
	private const float SplineOffset = 0.5f;
	#endregion
	
	#region Fields
	public SpriteShapeController spriteShape;
	public Transform[] points;
	#endregion
	
	#region MonoBehaviour Callbacks
	private void Awake() {
		UpdateVertices();
	}

	private void Update() {
		UpdateVertices();
	}
	#endregion
	
	#region privateMethods
	private void UpdateVertices() {
		for (var i = 0; i < points.Length-1; i++) {
			Vector2 vertex = points[i].localPosition;
			var towardsCenter = (Vector2.zero - vertex).normalized;
			var colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;
			try {
				spriteShape.spline.SetPosition(i, vertex - towardsCenter * colliderRadius);
			} catch {
				Debug.Log("Spline points are too close to eachother.. recalculate");
				spriteShape.spline.SetPosition(i, vertex - towardsCenter * (colliderRadius + SplineOffset));
			}

			Vector2 lt = spriteShape.spline.GetLeftTangent(i);

			Vector2 newRt = Vector2.Perpendicular(towardsCenter) * lt.magnitude;
			Vector2 newLt = Vector2.zero - newRt;
			
			spriteShape.spline.SetRightTangent(i, newRt);
			spriteShape.spline.SetLeftTangent(i, newLt);
		}
	}
	#endregion
}