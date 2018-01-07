using UnityEngine;
using System.Collections.Generic;

namespace DCM {
    /// <summary>
    /// This is the Texture combining class. It takes materials, combines the textures together, and returns the textures, as well as the UVs that the meshes will have to be modified to use
    /// </summary>
    public static class TextureCombineUtility {
        private static Texture2D _defaultTexture = null;

        public static Texture2D defaultTexture {
            get {
                if (_defaultTexture == null) {
                    _defaultTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);

                    Color[] colours = new Color[1024];
                    Color white = Color.white;
                    for (int i = 0; i < colours.Length; i++) {
                        colours [i] = white;

                    }
                   
                    _defaultTexture.SetPixels(colours);
                    _defaultTexture.Apply();
                }
                return _defaultTexture;
            }
        }
        /// <summary>
        /// Combines the material's textures together using the atlas options
        /// </summary>
        /// <param name="combines">The materials that are being combined.</param>
        /// <param name="atlasInfo">Atlas options information.</param>
        public static TextureCombineOutput Combine(IList<Material>combines, DrawCallMinimizerInfo atlasInfo) {   
            if (atlasInfo == null || atlasInfo.shaderPropertiesToLookFor == null || atlasInfo.shaderPropertiesToLookFor.Count <= 0) {
                Debug.LogError("You need to enter some shader properties to look for into Atlas Info. Cannot combine with 0 properties");
                return null;
            }
            TextureCombineOutput output = new TextureCombineOutput();

            IList<ShaderProperties> properties = GetShaderProperties(combines, atlasInfo);

            for (int i = 0; i < combines.Count; i++) {
                FillInNullMainTexture(combines [i]);
                for (int j = 0; j < properties.Count; j++) {
                    FillInNulls(combines [i], properties [j]);
                }
            }

            output.texturePositions = SetTexturePositions(combines, properties); 
            Texture2D textureAtlas = new Texture2D(atlasInfo.maxTextureSize, atlasInfo.maxTextureSize, (atlasInfo.ignoreTransparency && !properties [0].markAsNormal) ? TextureFormat.RGB24 : TextureFormat.ARGB32, true);
            Rect[] UVs = PackOriginalTexture(output.texturePositions, textureAtlas, atlasInfo);
            
            if (UVs != null) {
                Material newMaterial = new Material(combines [0]);
                newMaterial.SetTexture(properties [0].propertyName, textureAtlas);
                for (int i = 1; i < properties.Count; i++) {                      
                    Texture2D additionalAtlas = PackAdditionalTexture(GetTexturesAsArray(output.texturePositions, i), textureAtlas.width, textureAtlas.height, atlasInfo, UVs, properties [i].markAsNormal);

                    newMaterial.SetTexture(properties [i].propertyName, additionalAtlas);
                }
                output.combinedMaterial = newMaterial;
                return output;
            } 
            
            Debug.LogError("There was some sort of issue while trying to pack the textures...");
            return null;           
        }

        private static void FillInNulls(Material mat, ShaderProperties property) {
            if (mat.HasProperty(property.propertyName)) {
                if (mat.GetTexture(property.propertyName) == null) {
                    mat.SetTexture(property.propertyName, defaultTexture);
                }
            }
        }

        private static void FillInNullMainTexture(Material mat) {
            if (mat.mainTexture == null) {
                mat.mainTexture = defaultTexture;
            }
        }

        /// <summary>
        /// Combine the specified materials, but this overrides the readable option so that textures wil remain readable/non-readable for certain situations within the editor combining
        /// </summary>
        /// <param name="combines">Combines.</param>
        /// <param name="atlasInfo">Atlas info.</param>
        /// <param name="readableOveride">If set to <c>true</c> readable overide.</param>
        public static TextureCombineOutput Combine(IList<Material>combines, DrawCallMinimizerInfo atlasInfo, bool readableOveride) {   
            atlasInfo.readableTexture = readableOveride;
            return Combine(combines, atlasInfo);
        }

        /// <summary>
        /// Gets all of the shader properties for a given material and returns a list of them
        /// </summary>
        /// <returns>The shader properties.</returns>
        /// <param name="combines">The materials being combined.</param>
        /// <param name="atlasInfo">Atlas information containing the properties to look for</param>
        static IList<ShaderProperties> GetShaderProperties(IList<Material> combines, DrawCallMinimizerInfo atlasInfo) {
            return GetShaderProperties(combines [0], atlasInfo);
        }

        /// <summary>
        /// Gets all of the shader properties for a given material and returns a list of them
        /// </summary>
        /// <returns>The shader properties.</returns>
        /// <param name="combines">The materials being combined.</param>
        /// <param name="atlasInfo">Atlas information containing the properties to look for</param>
        public static IList<ShaderProperties> GetShaderProperties(Material material, DrawCallMinimizerInfo atlasInfo) {
            List<ShaderProperties> properties = new List<ShaderProperties>();
            
            for (int i = 0; i < atlasInfo.shaderPropertiesToLookFor.Count; i++) {
                if (material.HasProperty(atlasInfo.shaderPropertiesToLookFor [i].propertyName)) {
                    properties.Add(atlasInfo.shaderPropertiesToLookFor [i]);
                }
            }
            return properties.AsReadOnly();
        }

        /// <summary>
        /// Gets the textures contained within a list of texturePositions at a specific index
        /// </summary>
        /// <returns>The textures as array.</returns>
        /// <param name="texturePositions">Texture positions.</param>
        /// <param name="index">Index.</param>
        static Texture2D[] GetTexturesAsArray(IList<TexturePosition> texturePositions, int index) {
            if (texturePositions == null || texturePositions.Count == 0) {
                Debug.LogError("No TexturePositions exist in list passed to GetTexturesAsArray. Exiting out of batching operation... Check to see that objects being batched are not missing textures");
                return null;
            }

            if (texturePositions [0].textures == null || texturePositions [0].textures.Count == 0) {
                Debug.LogError("Could not find textures to batch during GetTexturesAsArray. Exiting out of batching operation... Check to see that objects being batched are not missing textures");
                return null;
            }

            if (index < 0 || index >= texturePositions [0].textures.Count) {
                Debug.LogError("Index passed into GetTexturesAsArray is out of bounds...Not quite sure how that could have happened, double check that your textures are set properly on your objects, and if they are, email johnjrpurdy@gmail.com with the Debug.Log output and what you are trying to do");
                return null;
            }

            Texture2D[] textures = new Texture2D[texturePositions.Count];

            for (int i = 0; i < textures.Length; i++) {
                textures [i] = texturePositions [i].textures [index];
            }

            return textures;
        }

        /// <summary>
        /// Sets the texture positions for each shader property etc
        /// </summary>
        /// <returns>The texture positions.</returns>
        /// <param name="combines">Combines.</param>
        /// <param name="properties">Properties.</param>
        static TexturePosition[] SetTexturePositions(IList<Material> combines, IList<ShaderProperties> properties) {
            TexturePosition[] texturePositions = new TexturePosition[combines.Count];
            for (int i = 0; i < combines.Count; i++) {
                TexturePosition tempTexturePosition = new TexturePosition();
                tempTexturePosition.textures = new List<Texture2D>(properties.Count);
                for (int j = 0; j < properties.Count; j++) {
                    if (combines [i].GetTexture(properties [j].propertyName) == null) {
                        Debug.LogError("Cannot combine textures when using Unity's default material texture");

                        return null;
                    }
                    tempTexturePosition.textures.Add(combines [i].GetTexture(properties [j].propertyName) as Texture2D);
                }
                texturePositions [i] = tempTexturePosition;
            }

            return texturePositions;
        }

        /// <summary>
        /// Packs the original texture using Unity's texture packing method
        /// </summary>
        /// <returns>The original texture.</returns>
        /// <param name="texturePositions">Texture positions.</param>
        /// <param name="textureAtlas">Texture atlas.</param>
        /// <param name="atlasInfo">Atlas info.</param>
        static Rect[] PackOriginalTexture(TexturePosition[] texturePositions, Texture2D textureAtlas, DrawCallMinimizerInfo atlasInfo) {
            textureAtlas.anisoLevel = atlasInfo.anisoLevel;
            textureAtlas.filterMode = atlasInfo.filterMode;
            textureAtlas.wrapMode = atlasInfo.wrapMode;
            Rect[] UVpositions = textureAtlas.PackTextures(GetTexturesAsArray(texturePositions, 0), atlasInfo.padding, atlasInfo.maxTextureSize, !atlasInfo.readableTexture);
           
            for (int i = 0; i < texturePositions.Length; i++) {
                texturePositions [i].position = UVpositions [i];
            }

            return UVpositions;
        }

        /// <summary>
        /// Packs the additional textures using the Rects recieved from the original packing method
        /// </summary>
        /// <returns>The additional texture.</returns>
        /// <param name="textures">Textures.</param>
        /// <param name="textureWidth">Texture width.</param>
        /// <param name="textureHeight">Texture height.</param>
        /// <param name="atlasInfo">Atlas info.</param>
        /// <param name="UVs">U vs.</param>
        /// <param name="markAsNormal">If set to <c>true</c> mark as normal.</param>
        static Texture2D PackAdditionalTexture(IList<Texture2D> textures, int textureWidth, int textureHeight, DrawCallMinimizerInfo atlasInfo, IList<Rect> UVs, bool markAsNormal) {
            Texture2D textureAtlas = new Texture2D(textureWidth, textureHeight, (atlasInfo.ignoreTransparency && !markAsNormal) ? TextureFormat.RGB24 : TextureFormat.ARGB32, true);
            textureAtlas.anisoLevel = atlasInfo.anisoLevel;
            textureAtlas.filterMode = atlasInfo.filterMode;
            textureAtlas.wrapMode = atlasInfo.wrapMode;

            for (int i=0; i<textures.Count; i++) {
                if (!Mathf.Approximately(textures [i].width, textureWidth * UVs [i].width) || !Mathf.Approximately(textures [i].height, textureHeight * UVs [i].height)) {
                    textures [i] = ScaleTexture(textures [i], (int)(UVs [i].width * textureWidth), (int)(UVs [i].height * textureHeight));
                }  

                try {
                    textureAtlas.SetPixels((int)(UVs [i].x * textureWidth), (int)(textureHeight * UVs [i].y), (int)(UVs [i].width * textureWidth), (int)(textureHeight * UVs [i].height), textures [i].GetPixels());                
                } catch (System.Exception exception) {
                    Debug.LogException(exception);
                    return null;
                }
            }

            textureAtlas.Apply(true, !atlasInfo.readableTexture);

            return textureAtlas;
        }

        /// <summary>
        /// Scales a texture so it can fit into a different sized position if needed
        /// </summary>
        /// <returns>The texture.</returns>
        /// <param name="oldTexture">Old texture.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        static Texture2D ScaleTexture(Texture2D oldTexture, int width, int height) {
            Color[] oldTextureColors = oldTexture.GetPixels();
            Color[] newTextureColors = new Color[width * height];
            
            float ratioX = 1.0f / ((float)width / (oldTexture.width - 1));
            float ratioY = 1.0f / ((float)height / (oldTexture.height - 1));
            
            int oldWidth = oldTexture.width;
            int newWidth = width;
            
            for (int y = 0; y < height; y++) {
                int yFloor = Mathf.FloorToInt(y * ratioY);
                int y1 = yFloor * oldWidth;
                int y2 = (yFloor + 1) * oldWidth;
                int yWidth = y * newWidth;
                
                for (int x = 0; x < newWidth; x++) {
                    int xFloor = Mathf.FloorToInt(x * ratioX);
                    float xLerp = x * ratioX - xFloor;
                    
                    newTextureColors [yWidth + x] = ColorLerpUnclamped(ColorLerpUnclamped(oldTextureColors [y1 + xFloor], oldTextureColors [y1 + xFloor + 1], xLerp),
                                                                      ColorLerpUnclamped(oldTextureColors [y2 + xFloor], oldTextureColors [y2 + xFloor + 1], xLerp),
                                                                      y * ratioY - yFloor);
                }
            }
            
            Texture2D newTexture = new Texture2D(width, height, oldTexture.format, false);
            newTexture.SetPixels(newTextureColors);
            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// Lerps two colours together using unclamped values
        /// </summary>
        /// <returns>The lerp unclamped.</returns>
        /// <param name="color1">Color1.</param>
        /// <param name="color2">Color2.</param>
        /// <param name="v">V.</param>
        static Color ColorLerpUnclamped(Color color1, Color color2, float v) {
            return new Color(color1.r + (color2.r - color1.r) * v,
                             color1.g + (color2.g - color1.g) * v,
                             color1.b + (color2.b - color1.b) * v,
                             color1.a + (color2.a - color1.a) * v);
        }
    }
}