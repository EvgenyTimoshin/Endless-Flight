using UnityEngine;
using System.Collections.Generic;

namespace DCM {
    /// <summary>
    /// This is information that is used by draw call minimizer to help it determine the best options for batching that applies to your project
    /// </summary>
    [System.Serializable]
    public class DrawCallMinimizerInfo {
        [SerializeField]
        private int
            _anisoLevel = 1;

        public int anisoLevel {
            get {
                return _anisoLevel;
            }
            set {
                _anisoLevel = value;
            }
        }

        [SerializeField]
        private bool
            _readableTexture = false;

        public bool readableTexture {
            get {
                return _readableTexture;
            }
            set {
                _readableTexture = value;
            }
        }

        [SerializeField]
        private FilterMode
            _filterMode = FilterMode.Bilinear;

        public FilterMode filterMode {
            get {
                return _filterMode;
            }
            set {
                _filterMode = value;
            }
        }

        [SerializeField]
        private bool
            _ignoreTransparency = false;

        public bool ignoreTransparency {
            get {
                return _ignoreTransparency;
            }
            set {
                _ignoreTransparency = value;
            }
        }

        [SerializeField]
        private TextureWrapMode
            _wrapMode = TextureWrapMode.Clamp;

        public TextureWrapMode wrapMode {
            get {
                return _wrapMode;
            }
            set {
                _wrapMode = value;
            }
        }

        [SerializeField]
        private List<ShaderProperties>
            _shaderPropertiesToLookFor = new List<ShaderProperties>();
       /* {
            new ShaderProperties(false, "_MainTex"), 
            new ShaderProperties(true, "_BumpMap"), 
            new ShaderProperties(false, "_Cube"), 
            new ShaderProperties(false, "_DecalTex"), 
            new ShaderProperties(false, "_Detail"), 
            new ShaderProperties(false, "_ParallaxMap")
        };*/

        public IList<ShaderProperties> shaderPropertiesToLookFor {
            get {
                return _shaderPropertiesToLookFor;
            }
            set {
                _shaderPropertiesToLookFor = (List<ShaderProperties>)value;
            }
        }

        [SerializeField]
        private int
            _maxTextureSize = 2048;

        public int maxTextureSize {
            get {
                return _maxTextureSize;
            }
            set {
                _maxTextureSize = value;
            }
        }

        [SerializeField]
        private int
            _padding = 0;

        public int padding {
            get {
                return _padding;
            }
            set {
                _padding = value;
            }
        }
    }
}