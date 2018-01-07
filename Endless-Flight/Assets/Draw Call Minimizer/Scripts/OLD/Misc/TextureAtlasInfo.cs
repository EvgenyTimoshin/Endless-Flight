using UnityEngine;

namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    [System.Serializable]
    public class TextureAtlasInfo {
        public int anisoLevel;
        public bool compressTexturesInMemory;
        public FilterMode filterMode;
        public bool ignoreAlpha;
        public TextureWrapMode wrapMode;
        public ShaderProperties[] shaderPropertiesToLookFor;
    
        public TextureAtlasInfo() {
            anisoLevel = 1;
            compressTexturesInMemory = true;
            filterMode = FilterMode.Trilinear;
            ignoreAlpha = true;
            wrapMode = TextureWrapMode.Clamp;   
    
            shaderPropertiesToLookFor = new ShaderProperties[]
        {
            new ShaderProperties(false, "_MainTex"), 
            new ShaderProperties(true, "_BumpMap"), 
            new ShaderProperties(false, "_Cube"), 
            new ShaderProperties(false, "_DecalTex"), 
            new ShaderProperties(false, "_Detail"), 
            new ShaderProperties(false, "_ParallaxMap")
        };
        }
    }
}
