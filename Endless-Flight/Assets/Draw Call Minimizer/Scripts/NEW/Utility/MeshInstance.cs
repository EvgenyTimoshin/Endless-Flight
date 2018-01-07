using UnityEngine;

namespace DCM {
    /// <summary>
    /// This struct is used throughout DrawCallMinimizer and stores information that is used to determine how the mesh is combined together with everything else.
    /// </summary>
    public struct MeshInstance {
        [SerializeField]
        private Mesh _mesh;
    
        public Mesh mesh {
            get {
                return _mesh;
            }
            set {
                _mesh = value;
            }
        }
        [SerializeField]
        private int _subMeshIndex;
    
        public int subMeshIndex {
            get {
                return _subMeshIndex;
            }
            set {
                _subMeshIndex = value;
            }
        }
        [SerializeField]
        private Matrix4x4 _transform;
    
        public Matrix4x4 transform {
            get {
                return _transform;
            }
            set {
                _transform = value;
            }
        }
    }
}
