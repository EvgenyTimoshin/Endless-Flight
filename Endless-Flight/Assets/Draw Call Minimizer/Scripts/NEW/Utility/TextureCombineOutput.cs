using UnityEngine;

namespace DCM {
    /// <summary>
    /// This class is used by the TextureCombineUtility. It just contains some information that is needed by DrawCallMinimizer, such as the combined material and the positions of the textures within the atlas
    /// </summary>
    public class TextureCombineOutput {
        [SerializeField]
        private Material
            _combinedMaterial;
        
        public Material combinedMaterial {
            get {
                return _combinedMaterial;
            }
            set {
                _combinedMaterial = value;
            }
        }

        [SerializeField]
        private TexturePosition[]
            _texturePositions;
        
        public TexturePosition[] texturePositions {
            get {
                return _texturePositions;
            }
            set {
                _texturePositions = value;
            }
        }
    }
}
