using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace SoftBody {
    public class SoftBodyBlobGenerator : MonoBehaviour {
        [SerializeField] private GameObject blobNodePrefab;
        [SerializeField] private float2 spacing;
        [SerializeField] private float2 blobArea;
        [SerializeField] private int maxNodes;
        [SerializeField] private Rigidbody2D masterNodeRigidbody2D;
        [Header("Tuning")] 
        [SerializeField] private bool tuningMode = false;
        [SerializeField] private bool letJointsAutoConfigureDistance = false;
        [SerializeField] private float springDistanceMultiplier = 1;
        [SerializeField] private float dampeningRatio;
        [SerializeField] private float frequency;
        
        private bool _lastLetJointsAutoConfigureDistance = false;
        private float _lastSpringDistanceMultiplier = 1;
        private float _lastDampeningRatio;
        private float _lastFrequency;
        
        private List<SpringJoint2D> _springJoint2Ds = new List<SpringJoint2D>();
        private Dictionary<SpringJoint2D, float> _startingDistances = new Dictionary<SpringJoint2D, float>();
        private Dictionary<GameObject, Vector3> _startingLocalPosition = new Dictionary<GameObject, Vector3>();

        private void Start() {
            GenerateSoftBody();
            //Time.timeScale = 0;
        }

        private void FixedUpdate() {
            if (!tuningMode) return;
            if (_lastLetJointsAutoConfigureDistance == letJointsAutoConfigureDistance &&
                Math.Abs(_lastSpringDistanceMultiplier - springDistanceMultiplier) < 0.001f && 
                Math.Abs(_lastDampeningRatio - dampeningRatio) < 0.001f && 
                Math.Abs(_lastFrequency - frequency) < 0.001f)
                return;

            _lastLetJointsAutoConfigureDistance = letJointsAutoConfigureDistance;
            _lastSpringDistanceMultiplier = springDistanceMultiplier;
            _lastDampeningRatio = dampeningRatio;
            _lastFrequency = frequency;
            
            foreach (SpringJoint2D springJoint2D in _springJoint2Ds) {
                TuneSpringJoint(springJoint2D);
            }
        }

        private void GenerateSoftBody() {
            if (float.IsNegative(spacing.x)) throw new ArgumentException($"{nameof(spacing)}.x cannot be negative");
            if (float.IsNegative(spacing.y)) throw new ArgumentException($"{nameof(spacing)}.y cannot be negative");
            if (spacing.x == 0) throw new ArgumentException($"{nameof(spacing)}.x cannot be zero");
            if (spacing.y == 0) throw new ArgumentException($"{nameof(spacing)}.y cannot be zero");

            int numberOfRows = Mathf.RoundToInt(blobArea.x / spacing.x);
            int numberOfCols = Mathf.RoundToInt(blobArea.y / spacing.y);
            int numberOfNodesToSpawn = Mathf.Min(numberOfRows * numberOfCols, maxNodes);
            List<GameObject> nodes = new List<GameObject>();
            Dictionary<GameObject, Rigidbody2D> nodesRigidBodyDictionary = new Dictionary<GameObject, Rigidbody2D>();
            // Add master
            nodesRigidBodyDictionary.Add(masterNodeRigidbody2D.gameObject, masterNodeRigidbody2D);
            
            Transform thisTransform = transform;
            float3 spawningOffset = new float3(blobArea.x / 2, blobArea.y / 2, 0);
            for (int y = 0; y < numberOfCols; y++) {
                for (int x = 0; x < numberOfRows; x++) {
                    if (nodes.Count >= numberOfNodesToSpawn)
                        break;

                    GameObject activeNode = Instantiate(blobNodePrefab, thisTransform.position, Quaternion.identity, thisTransform);
                    
                    _startingLocalPosition.Add(activeNode, activeNode.transform.localPosition);

                    // Add spacing
                    activeNode.transform.localPosition = new float3(x * spacing.x, y * spacing.y, 0) - spawningOffset;
                    nodes.Add(activeNode);
                    
                    // Add rb
                    nodesRigidBodyDictionary.Add(activeNode, activeNode.AddComponent<Rigidbody2D>());
                }
                if (nodes.Count >= numberOfNodesToSpawn)
                    break;
            }

            Vector2 spacingInVec2 = spacing;
            foreach (GameObject activeNode in nodes) {
                IEnumerable<GameObject> closestNodes = Physics2D.OverlapCircleAll(activeNode.transform.position,
                    spacingInVec2.magnitude)
                    .Select(x => x.gameObject)
                    .Where(x => nodesRigidBodyDictionary.ContainsKey(x));

                foreach (GameObject otherNode in closestNodes) {
                    if (otherNode == activeNode) continue;
                    
                    SpringJoint2D springJoint2D = activeNode.AddComponent<SpringJoint2D>();
                    springJoint2D.connectedBody = nodesRigidBodyDictionary[otherNode];
                    
                    // Set distance
                    float distance = Vector2.Distance(activeNode.transform.localPosition, 
                        otherNode.transform.localPosition);
                    springJoint2D.autoConfigureDistance = false;
                    _startingDistances.Add(springJoint2D, distance);
                    springJoint2D.distance = distance;
                    
                    TuneSpringJoint(springJoint2D);
                    _springJoint2Ds.Add(springJoint2D);
                }
            }
        }

        private void TuneSpringJoint(SpringJoint2D springJoint2D, bool resetPosition = false) {
            springJoint2D.autoConfigureDistance = letJointsAutoConfigureDistance;
            if (!letJointsAutoConfigureDistance) {
                springJoint2D.distance = _startingDistances[springJoint2D] * springDistanceMultiplier;
            }

            springJoint2D.dampingRatio = dampeningRatio;
            springJoint2D.frequency = frequency;
            springJoint2D.attachedRigidbody.WakeUp();

            if (resetPosition) {
                springJoint2D.transform.localPosition = _startingLocalPosition[springJoint2D.gameObject];
            }
        }
    }
}
