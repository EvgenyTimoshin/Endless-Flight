using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    public class CombineMeshInEditor : ScriptableWizard {
        public bool developmentBake = true;
        public GameObject parentToCombinedObjects = null;
        public TextureAtlasInfo textureAtlasProperties;
    
        [MenuItem("Window/Draw Call Minimizer/Obsolete/Optimize Level")]
        static void CreateWizard() {
            ScriptableWizard.DisplayWizard<CombineMeshInEditor>("Combine Meshes", "Export Combined Objects", "Combine");
        }
    
        void OnWizardUpdate() {
        }
    
        //Export combined mesh
        void OnWizardCreate() {
            //coming soon
        }
    
        //Basic Combine
        void OnWizardOtherButton() {
            MeshFilter[] filters = parentToCombinedObjects.GetComponentsInChildren<MeshFilter>();
            Matrix4x4 myTransform = parentToCombinedObjects.transform.worldToLocalMatrix;
        
            Dictionary<string, Dictionary<Material, List<MeshCombineUtility.MeshInstance>>> allMeshesAndMaterials = new Dictionary<string, Dictionary<Material, List<MeshCombineUtility.MeshInstance>>>();
            for (int i = 0; i < filters.Length; i++) {
                Renderer curRenderer = filters [i].GetComponent<Renderer>();
                MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
            
                instance.mesh = filters [i].sharedMesh;
            
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
                List<MeshCombineUtility.MeshInstance> meshes = new List<MeshCombineUtility.MeshInstance>();
            
                foreach (KeyValuePair<Material, List<MeshCombineUtility.MeshInstance>> kv in firstPass.Value) {
                    List<MeshCombineUtility.MeshInstance> meshIntermediates = new List<MeshCombineUtility.MeshInstance>();
                    Mesh[] firstCombineStep = MeshCombineUtility.Combine(kv.Value.ToArray());
                
                    for (int i = 0; i < firstCombineStep.Length; i++) {
                        MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
                        instance.mesh = firstCombineStep [i];
                        instance.subMeshIndex = 0;
                        instance.transform = Matrix4x4.identity;
                        meshIntermediates.Add(instance);
                    }
                
                    TextureCombineUtility.TexturePosition refTexture = textureUVPositions [0];
                    
                    for (int j = 0; j < textureUVPositions.Length; j++) {
                        if (kv.Key.mainTexture.name == textureUVPositions [j].textures [0].name) {
                            refTexture = textureUVPositions [j];                                        
                            break;
                        }
                    }   
                
                    for (int j = 0; j < meshIntermediates.Count; j++) {         
                        Vector2[] uvCopy = meshIntermediates [j].mesh.uv;
                        for (int k = 0; k < uvCopy.Length; k++) {
                            uvCopy [k].x = refTexture.position.x + uvCopy [k].x * refTexture.position.width;
                            uvCopy [k].y = refTexture.position.y + uvCopy [k].y * refTexture.position.height;
                        }
                    
                        meshIntermediates [j].mesh.uv = uvCopy;             
                    
                        uvCopy = meshIntermediates [j].mesh.uv2;
                        for (int k = 0; k < uvCopy.Length; k++) {
                            uvCopy [k].x = refTexture.position.x + uvCopy [k].x * refTexture.position.width;
                            uvCopy [k].y = refTexture.position.y + uvCopy [k].y * refTexture.position.height;
                        }                   
                
                        meshIntermediates [j].mesh.uv2 = uvCopy;

                        uvCopy = meshIntermediates [j].mesh.uv2;
                        for (int k = 0; k < uvCopy.Length; k++) {
                            uvCopy [k].x = refTexture.position.x + uvCopy [k].x * refTexture.position.width;
                            uvCopy [k].y = refTexture.position.y + uvCopy [k].y * refTexture.position.height;
                        }                   
                    
                        meshIntermediates [j].mesh.uv2 = uvCopy;
                    
                        meshes.Add(meshIntermediates [j]);
                    }                   
                }
            
                Material mat = combined;
                
                Mesh[] combinedMeshes = MeshCombineUtility.Combine(meshes.ToArray());
            
                GameObject parent = new GameObject("Combined " + parentToCombinedObjects.name + " " + firstPass.Key + " Mesh Parent");
                parent.transform.position = parentToCombinedObjects.transform.position;
                parent.transform.rotation = parentToCombinedObjects.transform.rotation;
    
                for (int i = 0; i < combinedMeshes.Length; i++) {
                    GameObject go = new GameObject("Combined " + parentToCombinedObjects.name + " Mesh");
                    go.transform.parent = parent.transform;
                    go.tag = parentToCombinedObjects.tag;
                    go.layer = parentToCombinedObjects.layer;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = Vector3.zero;
                    MeshFilter filter = go.AddComponent<MeshFilter>();
                    go.AddComponent<MeshRenderer>();
                    go.GetComponent<Renderer>().sharedMaterial = mat;

                    filter.mesh = combinedMeshes [i];
                }
            }
        
            if (developmentBake) {
                foreach (Renderer r in parentToCombinedObjects.GetComponentsInChildren<Renderer>()) {
                    r.enabled = false;
                }
            }
        }
    }
}