using UnityEngine;

namespace DCM {
    /// <summary>
    /// This class is here to represent a shader property that DrawCallMinimizer should be looking for.
    /// Mark as normal tells DrawCallMinimizer to mark the texture as a normal map and toss it in another atlas specific to the normal maps, and allows for the special import option
    /// </summary>
    [System.Serializable]
    public class ShaderProperties {
        [SerializeField]
        private bool
            _markAsNormal;

        public bool markAsNormal {
            get {
                return _markAsNormal;
            }
            set {
                _markAsNormal = value;
            }
        }

        [SerializeField]
        private string
            _propertyName;

        public string propertyName {
            get {
                return _propertyName;
            }
            set {
                _propertyName = value;
            }
        }

        public ShaderProperties(bool normal, string name) {
            _markAsNormal = normal;
            _propertyName = name;
        }
    }
}
