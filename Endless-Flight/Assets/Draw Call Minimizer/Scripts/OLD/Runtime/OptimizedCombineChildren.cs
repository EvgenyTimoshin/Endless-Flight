using UnityEngine;
using System.Collections.Generic;

/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.
 
 This is a modifed version of the CombineChildren script that unity supplies. This creates one mesh out of all objects childed underneath the game object
 and runs on one atlased texture. The old script the unity supplies really doesnt give you a performance boost whatsoever.
 
 The way that would make the most sense for you to use it is to child all of the objects in one room at a time instead of everything in the level
 that way the GPU isn't drawing everything
*/
namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    [AddComponentMenu("Mesh/Optimized Combine Children")]
    public class OptimizedCombineChildren : MonoBehaviour {
    
        /// Usually rendering with triangle strips is faster.
        /// However when combining objects with very low triangle counts, it can be faster to use triangles.
        /// Best is to try out which value is faster in practice.
        public TextureAtlasInfo textureAtlasProperties;// = new TextureCombineUtility.TextureAtlasInfo();
    
        /// This option has a far longer preprocessing time at startup but leads to better runtime performance.
        void Start() {
            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
            Matrix4x4 myTransform = transform.worldToLocalMatrix;
        
            Dictionary<string, Dictionary<Material, List<MeshCombineUtility.MeshInstance>>> allMeshesAndMaterials = new Dictionary<string, Dictionary<Material, List<MeshCombineUtility.MeshInstance>>>();
            for (int i = 0; i < filters.Length; i++) {
                Renderer curRenderer = filters [i].GetComponent<Renderer>();
                MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
            
                instance.mesh = filters [i].mesh;
            
                if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
                    instance.transform = myTransform * filters [i].transform.localToWorldMatrix;
                
                    Material[] materials = curRenderer.sharedMaterials;
                    for (int m = 0; m < materials.Length; m++) {
                        instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
                    
                        if (!allMeshesAndMaterials.ContainsKey(materials [m].shader.ToString())) {
                            allMeshesAndMaterials.Add(materials [m].shader.ToString(), new Dictionary<Material, List<MeshCombineUtility.MeshInstance>>());
                        }

                        if (!allMeshesAndMaterials [materials [m].shader.ToString()].ContainsKey(materials [m])) {
                            allMeshesAndMaterials [materials [m].shader.ToString()].Add(materials [m], new List<MeshCombineUtility.MeshInstance>());
                        }
                    
                        allMeshesAndMaterials [materials [m].shader.ToString()] [materials [m]].Add(instance);
                    }
                }
            }
        
            foreach (KeyValuePair<string, Dictionary<Material, List<MeshCombineUtility.MeshInstance>>>  firstPass in allMeshesAndMaterials) {
                Material[] allMaterialTextures = new Material[firstPass.Value.Keys.Count];
                int index = 0;
                                
                foreach (KeyValuePair<Material, List<MeshCombineUtility.MeshInstance>> kv in firstPass.Value) {
                    allMaterialTextures [index] = kv.Key;
                    index++;
                }
            
                TextureCombineUtility.TexturePosition[] textureUVPositions;
                Material combined = TextureCombineUtility.combine(allMaterialTextures, out textureUVPositions, textureAtlasProperties);
            
                if (textureUVPositions != null) {
            
                    List<MeshCombineUtility.MeshInstance> meshIntermediates = new List<MeshCombineUtility.MeshInstance>();
                    foreach (KeyValuePair<Material, List<MeshCombineUtility.MeshInstance>> kv in firstPass.Value) {
                        TextureCombineUtility.TexturePosition refTexture = textureUVPositions [0];
                    
                        for (int i = 0; i < textureUVPositions.Length; i++) {
                            if (kv.Key.mainTexture.name == textureUVPositions [i].textures [0].name) {
                                refTexture = textureUVPositions [i];                                        
                                break;
                            }
                        }   
                
                        for (int i = 0; i < kv.Value.Count; i++) {              
                            Vector2[] uvCopy = kv.Value [i].mesh.uv;
                    
                            for (int j = 0; j < uvCopy.Length; j++) {
                                uvCopy [j].x = refTexture.position.x + uvCopy [j].x * refTexture.position.width;
                                uvCopy [j].y = refTexture.position.y + uvCopy [j].y * refTexture.position.height;
                            }
                    
                            kv.Value [i].mesh.uv = uvCopy;              
                    

                            uvCopy = kv.Value [i].mesh.uv2;
                            for (int j = 0; j < uvCopy.Length; j++) {
                                uvCopy [j].x = refTexture.position.x + uvCopy [j].x * refTexture.position.width;
                                uvCopy [j].y = refTexture.position.y + uvCopy [j].y * refTexture.position.height;
                            }                   
                    
                            kv.Value [i].mesh.uv2 = uvCopy;
                    
                    
    
                            uvCopy = kv.Value [i].mesh.uv2;
                            for (int j = 0; j < uvCopy.Length; j++) {
                                uvCopy [j].x = refTexture.position.x + uvCopy [j].x * refTexture.position.width;
                                uvCopy [j].y = refTexture.position.y + uvCopy [j].y * refTexture.position.height;
                            }                   
                        
                            kv.Value [i].mesh.uv2 = uvCopy;
                    
                    
                            meshIntermediates.Add(kv.Value [i]);
                        }                   
                    }
            
                    Material mat = combined;
                
                    Mesh[] combinedMeshes = MeshCombineUtility.Combine(meshIntermediates.ToArray());
            
                    GameObject parent = new GameObject("Combined " + gameObject.name + " " + firstPass.Key + " Mesh Parent");
                    parent.transform.position = transform.position;
                    parent.transform.rotation = transform.rotation;
    
                    for (int i = 0; i < combinedMeshes.Length; i++) {
                        GameObject go = new GameObject("Combined " + gameObject.name + " Mesh");
                        go.transform.parent = parent.transform;
                        go.tag = gameObject.tag;
                        go.layer = gameObject.layer;
                        go.transform.localScale = Vector3.one;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localPosition = Vector3.zero;
                        MeshFilter filter = go.AddComponent<MeshFilter>();
                        go.AddComponent<MeshRenderer>();
                        go.GetComponent<Renderer>().sharedMaterial = mat;

                        filter.mesh = combinedMeshes [i];
                    }
                }
            } 

            //Destroy(gameObject);
            foreach (MeshFilter filter in filters) {
                Destroy(filter);
            }

            foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
                r.enabled = false;
            }
        }
    }
}