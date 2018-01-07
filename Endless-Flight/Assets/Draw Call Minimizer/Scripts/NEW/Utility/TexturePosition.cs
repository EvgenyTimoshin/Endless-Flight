using UnityEngine;
using System.Collections.Generic;

namespace DCM {
    /// <summary>
    /// This class holds data that is used throughout DrawCallMinimizer for UV positions and textures that use these UVs
    /// </summary>
    public struct TexturePosition {
        [SerializeField]
        private List<Texture2D>
            _textures;
        
        public IList<Texture2D> textures {
            get {
                return _textures;
            }
            set {
                _textures = (List<Texture2D>)value;
            }
        }

        [SerializeField]
        private Rect
            _position;
        
        public Rect position {
            get {
                return _position;
            }
            set {
                _position = value;
            }
        }
    }
}
