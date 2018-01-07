using UnityEngine;
using System.Collections.Generic;

namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    public class TextureCombineUtility {
        //For this class we use a struce that holds the texture and a rect for the new UV positions in the texture atlas
        public struct TexturePosition {
            public Texture2D[] textures;
            public Rect position;
        }   
        
        /**
     *  This function returns two variables:
        -> The tecture atlas
        -> An array of the old textures and their new positions. (Used for UV modification)
    */
        public static Material combine(Material[] combines, out TexturePosition[] texturePositions, TextureAtlasInfo atlasInfo) {   
            if (atlasInfo == null) {
                Debug.LogError("atlasInfo is null. Try removing and reattaching combine children component");
                texturePositions = null;
                return null;
            }
            if (atlasInfo.shaderPropertiesToLookFor.Length <= 0) {
                Debug.LogError("You need to enter some shader properties to look for into Atlas Info. Cannot combine with 0 properties");
                texturePositions = null;
                return null;
            }
            List<ShaderProperties> properties = new List<ShaderProperties>();
        
            for (int i = 0; i < atlasInfo.shaderPropertiesToLookFor.Length; i++) {
                if (combines [0].HasProperty(atlasInfo.shaderPropertiesToLookFor [i].propertyName)) {
                    properties.Add(atlasInfo.shaderPropertiesToLookFor [i]);
                }
            }       
        
            texturePositions = new TexturePosition[combines.Length];
        
            for (int i = 0; i < combines.Length; i++) {
                TexturePosition tempTexturePosition = new TexturePosition();
                tempTexturePosition.textures = new Texture2D[properties.Count];
            
                for (int j = 0; j < properties.Count; j++) {                
                    //Debug.Log((combines[i].GetTexture(properties[j].propertyName) == null) + ", " +  properties[j].propertyName + ", " + combines[i].name);
                    if (combines [i].GetTexture(properties [j].propertyName) == null) {
                        Debug.LogError("Cannot combine textures when using Unity's default material texture");
                        texturePositions = null;
                        return null;
                    }
                    tempTexturePosition.textures [j] = Object.Instantiate(combines [i].GetTexture(properties [j].propertyName)) as Texture2D;
                    tempTexturePosition.textures [j].name = tempTexturePosition.textures [j].name.Remove(tempTexturePosition.textures [j].name.IndexOf("(Clone)", System.StringComparison.Ordinal));
                
                }
            
                texturePositions [i] = tempTexturePosition;
            }

            textureQuickSort(texturePositions, 0, texturePositions.Length - 1);
                
            for (int i = 0; i < texturePositions.Length; i++) {
                for (int j = 1; j < texturePositions[i].textures.Length; j++) {
                    texturePositions [i].textures [j] = scaleTexture(texturePositions [i].textures [j], texturePositions [i].textures [0].width, texturePositions [i].textures [0].height);
                }
            }

            texturePositions [0].position.x = texturePositions [0].position.y = 0;
            texturePositions [0].position.width = texturePositions [0].textures [0].width;
            texturePositions [0].position.height = texturePositions [0].textures [0].height;
        
            int height = texturePositions [0].textures [0].height;
            int width = texturePositions [0].textures [0].width;
        
            int widthIndex = width;
            int heightIndex = 0;
        
            bool useHeightAsReference = true;
            for (int i = 1; i < texturePositions.Length; i++) {
                texturePositions [i].position.x = widthIndex;
                texturePositions [i].position.y = heightIndex;
                texturePositions [i].position.width = texturePositions [i].textures [0].width;
                texturePositions [i].position.height = texturePositions [i].textures [0].height;
            
                if (useHeightAsReference) { 
                    if (widthIndex + texturePositions [i].textures [0].width > width) {
                        width = widthIndex + texturePositions [i].textures [0].width;
                    }
                
                    heightIndex += texturePositions [i].textures [0].height;
                
                    if (heightIndex >= height) {
                        useHeightAsReference = false;
                        height = heightIndex;
                        heightIndex = height;
                    
                        widthIndex = 0;
                    }
                } else {
                    if (heightIndex + texturePositions [i].textures [0].height > height) {
                        height = heightIndex + texturePositions [i].textures [0].height;
                    }
                
                    widthIndex += texturePositions [i].textures [0].width;
                
                    if (widthIndex >= width) {
                        useHeightAsReference = true;
                        width = widthIndex;
                        widthIndex = width;
                    
                        heightIndex = 0;
                    }               
                }
            }
        
            if (height > width) {
                width = height;
            } else {
                height = width;
            }
            float textureSizeFactor = 1.0f / height;
            
            Material newMaterial = new Material(combines [0]);
        
            for (int i = 0; i < properties.Count; i++) {        
                Texture2D combinesTextures = new Texture2D(width, height, (atlasInfo.ignoreAlpha && !properties [i].markAsNormal) ? TextureFormat.RGB24 : TextureFormat.ARGB32, true);
                combinesTextures.anisoLevel = atlasInfo.anisoLevel;
                combinesTextures.filterMode = atlasInfo.filterMode;
                combinesTextures.wrapMode = atlasInfo.wrapMode;
                        
                for (int j = 0; j < texturePositions.Length; j++) {
                    combinesTextures.SetPixels((int)texturePositions [j].position.x, (int)texturePositions [j].position.y, texturePositions [j].textures [i].width, texturePositions [j].textures [i].height, texturePositions [j].textures [i].GetPixels());
                }
            
                combinesTextures.Apply();
            
            
                if (atlasInfo.compressTexturesInMemory) {
                    combinesTextures.Compress(true);
                }
            
                newMaterial.SetTexture(properties [i].propertyName, combinesTextures);
            }
                
            for (int i = 0; i < texturePositions.Length; i++) {
                texturePositions [i].position.x = texturePositions [i].position.x * textureSizeFactor;
                texturePositions [i].position.y = texturePositions [i].position.y * textureSizeFactor;
                texturePositions [i].position.width = texturePositions [i].position.width * textureSizeFactor;
                texturePositions [i].position.height = texturePositions [i].position.height * textureSizeFactor;
            }

            return newMaterial;
        }
        
        static void textureQuickSort(TexturePosition[] textures, int low, int high) {
            if (low < high) {
                int pivot = partition(textures, low, high);
                textureQuickSort(textures, low, pivot - 1);
                textureQuickSort(textures, pivot + 1, high);
            }
        }
        
        static int partition(TexturePosition[] texturePositions, int low, int high) {
            TexturePosition pivot_item;
            int pivotPosition = low;
            pivot_item = texturePositions [pivotPosition];

            for (int i = low + 1; i <= high; i++) {
                if (texturePositions [i].textures [0].height > pivot_item.textures [0].height) {
                    pivotPosition++;
                    swap(texturePositions, pivotPosition, i);
                }
            }
        
            swap(texturePositions, low, pivotPosition);

            return pivotPosition;   
        }
    
        static void swap(TexturePosition[] textures, int i, int j) {
            TexturePosition temp = textures [i];
            textures [i] = textures [j];
            textures [j] = temp;
        }
    
        static Texture2D scaleTexture(Texture2D oldTexture, int width, int height) {
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
                
                    newTextureColors [yWidth + x] = colorLerpUnclamped(colorLerpUnclamped(oldTextureColors [y1 + xFloor], oldTextureColors [y1 + xFloor + 1], xLerp),
                                                                  colorLerpUnclamped(oldTextureColors [y2 + xFloor], oldTextureColors [y2 + xFloor + 1], xLerp),
                                                                  y * ratioY - yFloor);
                }
            }
        
            Texture2D newTexture = new Texture2D(width, height, oldTexture.format, false);
            newTexture.SetPixels(newTextureColors);
            newTexture.Apply();
            return newTexture;
        }
    
        static Color colorLerpUnclamped(Color color1, Color color2, float v) {
            return new Color(color1.r + (color2.r - color1.r) * v,
                         color1.g + (color2.g - color1.g) * v,
                         color1.b + (color2.b - color1.b) * v,
                         color1.a + (color2.a - color1.a) * v);
        }   
    }
}