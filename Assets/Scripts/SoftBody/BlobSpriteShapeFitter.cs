using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SoftBody {
    public class BlobSpriteShapeFitter : MonoBehaviour {
        [SerializeField] private SpriteShapeController spriteShape;
        [SerializeField] private Transform middleNode;
        [SerializeField] private Transform[] softBodyNodes = new Transform[11];

        [Header("Tuning values")] 
        [SerializeField] private float growShrink = 0;
        [SerializeField] private float tangentMagnitudeMultiplier = 1;

        private readonly Dictionary<Transform, CircleCollider2D>
            _circleColliders = new Dictionary<Transform, CircleCollider2D>();

        private void Start() {
            foreach (Transform node in softBodyNodes) {
                _circleColliders.Add(node, node.gameObject.AddComponent<CircleCollider2D>());
            }
        }

        private void Update() {
            UpdateSpriteShape();
        }

        private void UpdateSpriteShape() {
            spriteShape.transform.position = middleNode.position;
            
            for (int i = 0; i < softBodyNodes.Length; i++) {
                Transform softBodyNode = softBodyNodes[i];
                Vector3 nodePos = softBodyNode.localPosition;

                Vector3 directionToCenter = (Vector3.zero - nodePos).normalized;
                
                float radius = _circleColliders[softBodyNode].radius;

                Vector3 point = nodePos - directionToCenter * (radius + growShrink);
                
                try {
                    spriteShape.spline.SetPosition(i, point);
                }
                catch (ArgumentException e) {
                    Debug.LogWarning(e.Message);
                    const float offset = 0.5f;
                    point = nodePos - directionToCenter * (radius + growShrink + offset);
                    spriteShape.spline.SetPosition(i, point);
                }

                Vector2 leftTangent = spriteShape.spline.GetLeftTangent(i);
                Vector2 replacementLeftTangent = Vector2.Perpendicular(directionToCenter) * (leftTangent.magnitude * tangentMagnitudeMultiplier);
                Vector2 replacementRightTangent = Vector2.zero - replacementLeftTangent;
                
                spriteShape.spline.SetRightTangent(i, replacementRightTangent);
                spriteShape.spline.SetLeftTangent(i, replacementLeftTangent);
            }
        }
    }
}