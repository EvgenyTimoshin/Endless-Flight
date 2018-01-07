using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The draw call minimizer namespace. Everything that DrawCallMinimizer uses is within this namespace, and if for some reason you need to use the old stuff in your scripts, you can use the DCM.Old namespace to reference those classes
/// </summary>
namespace DCM {
    /// <summary>
    /// This script replaces the OptimizedCombineMesh script that was in the old version of Draw Call Minimizer.
    /// This script gives you some options on how you want to combine your mesh, and comes with a custom inspector to make it easier to use and understand
    /// </summary>
    public class DrawCallMinimizer : MonoBehaviour {
        [SerializeField]
        private DrawCallMinimizerInfo
            _textureAtlasProperties = new DrawCallMinimizerInfo();

        public DrawCallMinimizerInfo textureAtlasProperties {
            get {
                return _textureAtlasProperties;
            }
            set {
                _textureAtlasProperties = value;
            }
        }

        [SerializeField]
        private bool
            destroyOriginalGameObject = false;
        [SerializeField]
        private bool
            skipTextureAtlasing = false;
        public const string VERSION = "2.0";
        /// This option has a far longer preprocessing time at startup but leads to better runtime performance.
        void Start() {

            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
            Matrix4x4 myTransform = transform.worldToLocalMatrix;

            Dictionary<string, Dictionary<Material, List<MeshInstance>>> allMeshesAndMaterials = new Dictionary<string, Dictionary<Material, List<MeshInstance>>>();
            OrganizeObjects(filters, myTransform, allMeshesAndMaterials, skipTextureAtlasing);   

            if(skipTextureAtlasing)
            {
                CreateBatchedObjects(allMeshesAndMaterials); 

            }
            else
            {
                CreateBatchedAtlasedObjects(allMeshesAndMaterials); 

            }


            if (destroyOriginalGameObject) {
                Destroy(gameObject);
            } else {
                foreach (MeshFilter filter in filters) {
                    Destroy(filter);
                }
                
                foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
                    r.enabled = false;
                }

                Destroy(this);
            }
        }

        /// <summary>
        /// Organizes all of the objects in the data tree so that DrawCall Minimizer can do its work
        /// </summary>
        /// <param name="filters">An array of MeshFilters that are going to be combined</param>
        /// <param name="myTransform">A Matrix representing the parent object's transform</param>
        /// <param name="allMeshesAndMaterials">The dictionary that all of this information is going to be stored in</param>
        static void OrganizeObjects(MeshFilter[] filters, Matrix4x4 myTransform, IDictionary<string, Dictionary<Material, List<MeshInstance>>> allMeshesAndMaterials, bool skipAtlasing) {

            for (int i = 0; i < filters.Length; i++) {
                Renderer curRenderer = filters [i].GetComponent<Renderer>();
                MeshInstance instance = new MeshInstance();
                instance.mesh = filters [i].mesh;
                if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
                    instance.transform = myTransform * filters [i].transform.localToWorldMatrix;
                    Material[] materials = curRenderer.sharedMaterials;
                    for (int m = 0; m < materials.Length; m++) {
                        instance.subMeshIndex = Mathf.Min(m, instance.mesh.subMeshCount - 1);
                        if(skipAtlasing)
                        {
                            Dictionary<Material, List<MeshInstance>> mainList;

                            if(!allMeshesAndMaterials.TryGetValue("", out mainList))
                            {

                                mainList = new Dictionary<Material, List<MeshInstance>>();
                                allMeshesAndMaterials.Add("", mainList);
                            }
                            List<MeshInstance> groupedMeshes;
                            if(!mainList.TryGetValue(materials[m], out groupedMeshes))
                            {
                                groupedMeshes = new List<MeshInstance>();
                                mainList.Add(materials[m], groupedMeshes);
                            }
                            groupedMeshes.Add(instance);
                        }
                        else
                        {
                            ConfirmDictionaryKeys(allMeshesAndMaterials, materials [m]);
                            allMeshesAndMaterials [materials [m].shader.ToString()] [materials [m]].Add(instance);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// This just makes sure that a key is actually there in the dictionary, and if its not it adds it
        /// </summary>
        /// <param name="allMeshesAndMaterials">The dictionary that stores all of the information</param>
        /// <param name="mat">The material that is used as a key</param>
        static void ConfirmDictionaryKeys(IDictionary<string, Dictionary<Material, List<MeshInstance>>> allMeshesAndMaterials, Material mat) {
            if (!allMeshesAndMaterials.ContainsKey(mat.shader.ToString())) {
                allMeshesAndMaterials.Add(mat.shader.ToString(), new Dictionary<Material, List<MeshInstance>>());
            }
            if (!allMeshesAndMaterials [mat.shader.ToString()].ContainsKey(mat)) {
                allMeshesAndMaterials [mat.shader.ToString()].Add(mat, new List<MeshInstance>());
            }
        }

        /// <summary>
        /// The main combining method. This calls combining methods and organizes game objects in the scene
        /// </summary>
        /// <param name="allMeshesAndMaterials">The dictionary containing all of the data</param>
        void CreateBatchedAtlasedObjects(IDictionary<string, Dictionary<Material, List<MeshInstance>>> allMeshesAndMaterials) {
            foreach (KeyValuePair<string, Dictionary<Material, List<MeshInstance>>> firstPass in allMeshesAndMaterials) {
                List<Material> allMaterialTextures = new List<Material>(firstPass.Value.Keys);

                TextureCombineOutput textureCombineOutput = TextureCombineUtility.Combine(allMaterialTextures, _textureAtlasProperties);

                if (textureCombineOutput != null && textureCombineOutput.texturePositions != null) {
                    List<MeshInstance> meshIntermediates = new List<MeshInstance>();
                    foreach (KeyValuePair<Material, List<MeshInstance>> kv in firstPass.Value) {
                        TexturePosition refTexture = GetReferenceTexturePosition(textureCombineOutput.texturePositions, kv.Key.mainTexture.name);

                        for (int i = 0; i < kv.Value.Count; i++) {
                            OffsetUvs(kv.Value [i].mesh, refTexture.position);
                            meshIntermediates.Add(kv.Value [i]);
                        }
                    }

                    IList<Mesh> combinedMeshes = MeshCombineUtility.Combine(meshIntermediates);
                    GameObject parent = new GameObject("Combined " + gameObject.name + " " + firstPass.Key + " Mesh Parent");
                    parent.transform.position = transform.position;
                    parent.transform.rotation = transform.rotation;
                    for (int i = 0; i < combinedMeshes.Count; i++) {
                        GameObject go = new GameObject("Combined " + gameObject.name + " Mesh");
                        go.transform.parent = parent.transform;
                        go.tag = gameObject.tag;
                        go.layer = gameObject.layer;
                        go.transform.localScale = Vector3.one;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localPosition = Vector3.zero;
                        MeshFilter filter = go.AddComponent<MeshFilter>();
                        go.AddComponent<MeshRenderer>().sharedMaterial = textureCombineOutput.combinedMaterial;
                        filter.mesh = combinedMeshes [i];
                    }
                }
            }
        }

        void CreateBatchedObjects(IDictionary<string, Dictionary<Material, List<MeshInstance>>> allMeshesAndMaterials) {

            Dictionary<Material, List<MeshInstance>> mainGroup = allMeshesAndMaterials[""];

            foreach(KeyValuePair<Material, List<MeshInstance>> unatlasedGroup in mainGroup)
            {                
                IList<Mesh> combinedMeshes = MeshCombineUtility.Combine(unatlasedGroup.Value);
                GameObject parent = new GameObject("Combined " + gameObject.name + " " + unatlasedGroup.Key + " Mesh Parent");
                parent.transform.position = transform.position;
                parent.transform.rotation = transform.rotation;
                for (int i = 0; i < combinedMeshes.Count; i++) {
                    GameObject go = new GameObject("Combined " + gameObject.name + " Mesh");
                    go.transform.parent = parent.transform;
                    go.tag = gameObject.tag;
                    go.layer = gameObject.layer;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = Vector3.zero;
                    MeshFilter filter = go.AddComponent<MeshFilter>();
                    go.AddComponent<MeshRenderer>().sharedMaterial = unatlasedGroup.Key;
                    filter.mesh = combinedMeshes [i];
                }
            }
        }

        /// <summary>
        /// This gets a texture position from a name passed as a parameter. Used as a quick and easy way to grab an appropriate texture position from a texture name
        /// </summary>
        /// <returns>The reference texture position.</returns>
        /// <param name="textureUVPositions">Texture UV positions.</param>
        /// <param name="textureName">Texture name.</param>
        static TexturePosition GetReferenceTexturePosition(IList<TexturePosition> textureUVPositions, string textureName) {
            TexturePosition texturePosition = textureUVPositions [0];
            for (int i = 0; i < textureUVPositions.Count; i++) {
                if (textureName.Length == textureUVPositions [i].textures [0].name.Length) {
                    if (textureName == textureUVPositions [i].textures [0].name) {
                        texturePosition = textureUVPositions [i];
                        break;
                    }
                }
            }
            return texturePosition;
        }

        /// <summary>
        /// Offsets the uvs of a mesh to the new atlased position so that it properly aligns with the atlas
        /// </summary>
        /// <param name="modifiedMesh">Modified mesh.</param>
        /// <param name="referencedTexturePosition">Referenced texture position.</param>
        static void OffsetUvs(Mesh modifiedMesh, Rect referencedTexturePosition) {
            Vector2[] uvCopy = modifiedMesh.uv;
            for (int j = 0; j < uvCopy.Length; j++) {
                uvCopy [j].x = referencedTexturePosition.x + uvCopy [j].x * referencedTexturePosition.width;
                uvCopy [j].y = referencedTexturePosition.y + uvCopy [j].y * referencedTexturePosition.height;
            }
            modifiedMesh.uv = uvCopy;
            uvCopy = modifiedMesh.uv2;
            for (int j = 0; j < uvCopy.Length; j++) {
                uvCopy [j].x = referencedTexturePosition.x + uvCopy [j].x * referencedTexturePosition.width;
                uvCopy [j].y = referencedTexturePosition.y + uvCopy [j].y * referencedTexturePosition.height;
            }
            modifiedMesh.uv2 = uvCopy;
        }
    }
}
