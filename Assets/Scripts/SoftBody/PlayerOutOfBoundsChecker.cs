using UnityEngine;

namespace SoftBody.Subjects {
    [RequireComponent(typeof(BlobResetter))]
    public class PlayerOutOfBoundsChecker : MonoBehaviour {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float levelFloor;

        private BlobResetter _blobResetter;

        private void Start() {
            _blobResetter = GetComponent<BlobResetter>();
        }

        private void Update() {
            Vector2 playerPosition = playerTransform.position;
            if (playerPosition.y < levelFloor || playerPosition.y > 100) _blobResetter.ResetBlob();
        }
    }
}