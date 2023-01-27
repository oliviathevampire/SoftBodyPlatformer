using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoftBody {
    public class BlobResetter : MonoBehaviour {
        public void ResetBlob() {
            // Cant make the player reset correctly within the timeframe.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }
}