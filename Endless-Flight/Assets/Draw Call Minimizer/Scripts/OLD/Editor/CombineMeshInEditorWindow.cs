using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;

namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    public class CombineMeshInEditorWindow : EditorWindow {
        private GameObject m_parentObject;
        private bool m_saveCombinedMeshAssets;
        private static CombineMeshInEditorWindow m_window;
        private TextureAtlasInfo m_textureAtlasInfo;
        private int m_texPropArraySize;
        private bool m_showShaderProperties;
        private bool[] m_shaderFoldoutBools;
        private bool m_createPrefab;
        static private MeshImportSettings m_modelImportSettings;
        static private TextureImportSettings m_textureImportSettings;
        private bool m_showModelSettings = false;
        private bool m_showTextureSettings = false;
        private Vector2 m_scrollPosition;
        static private string m_pathToAssets;
        private bool exportAssets = true;
        private string m_folderPath = "";
    
        [MenuItem("Window/Draw Call Minimizer/Obsolete/Combine In Editor Window")]
        static void Init() {
            m_window = EditorWindow.GetWindow<CombineMeshInEditorWindow>("Draw Call Min");  
            m_modelImportSettings = new MeshImportSettings();
            m_textureImportSettings = new TextureImportSettings();
            m_pathToAssets = Application.dataPath + "/";
        }
    
        void OnGUI() {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Select the parent object that is to be combined.");
        
            EditorGUI.BeginChangeCheck();
            m_parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object Of Meshes To Be Combined", m_parentObject, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck()) {
                m_textureAtlasInfo = new TextureAtlasInfo();
                m_texPropArraySize = m_textureAtlasInfo.shaderPropertiesToLookFor.Length;
                m_shaderFoldoutBools = new bool[m_texPropArraySize];
            }
        
            if (m_parentObject != null) {           
                m_textureAtlasInfo.compressTexturesInMemory = false;//EditorGUILayout.Toggle("Compress Texture", m_textureAtlasInfo.compressTexturesInMemory);
            
                EditorGUI.BeginChangeCheck();
                m_texPropArraySize = EditorGUILayout.IntSlider("# Of Shader Properties", m_texPropArraySize, 0, 20);
                if (EditorGUI.EndChangeCheck()) {
                    if (m_texPropArraySize > m_textureAtlasInfo.shaderPropertiesToLookFor.Length) {
                        ShaderProperties[] temp = new ShaderProperties[m_texPropArraySize];
                    
                        for (int i = 0; i < m_texPropArraySize; i++) {
                            if (i < m_textureAtlasInfo.shaderPropertiesToLookFor.Length) {
                                temp [i] = m_textureAtlasInfo.shaderPropertiesToLookFor [i];
                            } else {
                                temp [i] = new ShaderProperties(false, "");
                            }
                        }
                    
                        m_textureAtlasInfo.shaderPropertiesToLookFor = temp;                    
                    } else if (m_texPropArraySize < m_textureAtlasInfo.shaderPropertiesToLookFor.Length) {
                        ShaderProperties[] temp = new ShaderProperties[m_texPropArraySize];
                    
                        for (int i = 0; i < m_texPropArraySize; i++) {
                            temp [i] = m_textureAtlasInfo.shaderPropertiesToLookFor [i];
                        }
                
                        m_textureAtlasInfo.shaderPropertiesToLookFor = temp;
                    }
                
                    m_shaderFoldoutBools = new bool[m_texPropArraySize];
                }
            
                m_showShaderProperties = EditorGUILayout.Foldout(m_showShaderProperties, "Shader Properties To Watch For");
                if (m_showShaderProperties) {
                    for (int i = 0; i < m_texPropArraySize; i++) {
                        m_shaderFoldoutBools [i] = EditorGUILayout.Foldout(m_shaderFoldoutBools [i], "Shader Properties Element " + i);
                    
                        if (m_shaderFoldoutBools [i]) {
                            m_textureAtlasInfo.shaderPropertiesToLookFor [i].markAsNormal = EditorGUILayout.Toggle("Mark As Normal Map", m_textureAtlasInfo.shaderPropertiesToLookFor [i].markAsNormal);
                            m_textureAtlasInfo.shaderPropertiesToLookFor [i].propertyName = EditorGUILayout.TextField("Shader Property Name", m_textureAtlasInfo.shaderPropertiesToLookFor [i].propertyName);
                        }
                    }
                }
            
                GUILayout.Space(m_window.position.height * 0.05f);
            
                EditorGUI.BeginChangeCheck();
                m_folderPath = EditorGUILayout.TextField("Combined Asset Path", m_folderPath);
                if (EditorGUI.EndChangeCheck()) {
                    m_pathToAssets = Application.dataPath + "/" + m_folderPath + "/";
                }
            
                GUILayout.Space(m_window.position.height * 0.05f);
            
                m_showModelSettings = EditorGUILayout.Foldout(m_showModelSettings, "Model Settings");
                if (m_showModelSettings) {
                    GUILayout.Label("Meshes", "BoldLabel");
                    EditorGUILayout.LabelField("Meshes");
                    m_modelImportSettings.globalScale = EditorGUILayout.FloatField("Global Scale", m_modelImportSettings.globalScale);
                    m_modelImportSettings.meshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", m_modelImportSettings.meshCompression);
                    m_modelImportSettings.optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", m_modelImportSettings.optimizeMesh);
                    m_modelImportSettings.addCollider = EditorGUILayout.Toggle("Generate Colliders", m_modelImportSettings.addCollider);
                    m_modelImportSettings.swapUVChannels = EditorGUILayout.Toggle("Swap UVs", m_modelImportSettings.swapUVChannels);
                    m_modelImportSettings.generateSecondaryUV = EditorGUILayout.Toggle("Generate Lightmap UVs", m_modelImportSettings.generateSecondaryUV);
                
                    GUILayout.Label("Normals & Tangents", "BoldLabel");

                    m_modelImportSettings.normalImportMode = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup("Normals", m_modelImportSettings.normalImportMode);
                    m_modelImportSettings.tangentImportMode = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup("Tangents", m_modelImportSettings.tangentImportMode);
                
                    if ((m_modelImportSettings.normalImportMode == ModelImporterTangentSpaceMode.Calculate) && m_modelImportSettings.tangentImportMode != ModelImporterTangentSpaceMode.None) {
                        m_modelImportSettings.tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
                    }
                
                    EditorGUI.BeginDisabledGroup(m_modelImportSettings.normalImportMode != ModelImporterTangentSpaceMode.Calculate);
                    m_modelImportSettings.normalSmoothingAngle = EditorGUILayout.IntSlider("Normal Smoothing Angle", (int)m_modelImportSettings.normalSmoothingAngle, 0, 180);
                    EditorGUI.EndDisabledGroup();
                
                    EditorGUI.BeginDisabledGroup(m_modelImportSettings.tangentImportMode != ModelImporterTangentSpaceMode.Calculate);
                    m_modelImportSettings.splitTangentsAcrossSeams = EditorGUILayout.Toggle("Split Tangents", m_modelImportSettings.splitTangentsAcrossSeams);
                    EditorGUI.EndDisabledGroup();               
                }
            
                m_showTextureSettings = EditorGUILayout.Foldout(m_showTextureSettings, "Texture Settings");
                if (m_showTextureSettings) {
                    m_textureImportSettings.textureType = (TextureImporterType)EditorGUILayout.EnumPopup("", m_textureImportSettings.textureType);
            
                    switch (m_textureImportSettings.textureType) {
                        case TextureImporterType.NormalMap:                  
                    //m_textureImportSettings.convertToNormalmap = EditorGUILayout.Toggle("", m_textureImportSettings.convertToNormalmap);
                            m_textureImportSettings.heightmapScale = EditorGUILayout.Slider(m_textureImportSettings.heightmapScale, 0.0f, 0.3f);
                            m_textureImportSettings.normalmapFilter = (TextureImporterNormalFilter)EditorGUILayout.EnumPopup("Normal Map Filter", m_textureImportSettings.normalmapFilter);
                    
                            m_textureImportSettings.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Texture Wrap Mode", m_textureImportSettings.wrapMode);
                            m_textureImportSettings.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Texture Filter Mode", m_textureImportSettings.filterMode);
                            m_textureImportSettings.anisoLevel = EditorGUILayout.IntSlider("Aniso Level", m_textureImportSettings.anisoLevel, 0, 10);
                            break;
                        case TextureImporterType.Lightmap:
                            m_textureImportSettings.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Texture Filter Mode", m_textureImportSettings.filterMode);
                            m_textureImportSettings.anisoLevel = EditorGUILayout.IntSlider("Aniso Level", m_textureImportSettings.anisoLevel, 0, 10);                   
                            break;
                        case TextureImporterType.Reflection:
                            m_textureImportSettings.grayscaleToAlpha = EditorGUILayout.Toggle("Alpha From Grayscale", m_textureImportSettings.grayscaleToAlpha);
                            m_textureImportSettings.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Texture Filter Mode", m_textureImportSettings.filterMode);
                            m_textureImportSettings.anisoLevel = EditorGUILayout.IntSlider("Aniso Level", m_textureImportSettings.anisoLevel, 0, 10);
                            break;
                        default:
                            m_textureImportSettings.textureType = TextureImporterType.Default;
                            m_textureImportSettings.grayscaleToAlpha = EditorGUILayout.Toggle("Alpha From Grayscale", m_textureImportSettings.grayscaleToAlpha);
                            m_textureImportSettings.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("Texture Wrap Mode", m_textureImportSettings.wrapMode);
                            m_textureImportSettings.filterMode = (FilterMode)EditorGUILayout.EnumPopup("Texture Filter Mode", m_textureImportSettings.filterMode);
                            m_textureImportSettings.anisoLevel = EditorGUILayout.IntSlider("Aniso Level", m_textureImportSettings.anisoLevel, 0, 10);
                            break;
                    }
                
                    m_textureImportSettings.maxTextureSize = (int)(object)EditorGUILayout.EnumPopup((TextureSize)m_textureImportSettings.maxTextureSize);
                    m_textureImportSettings.textureFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup(m_textureImportSettings.textureFormat);
                
                
                    /*mip map stuff*/
                }
                EditorGUILayout.Toggle("Export Assets ", exportAssets); 
                GUILayout.Space(m_window.position.height * 0.05f);
            
            
                EditorGUILayout.EndVertical();  
            
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Combine Mesh", GUILayout.Height(m_window.position.height * 0.1f))) {
                    combineMesh(exportAssets);
                }   
            
            
            
                EditorGUILayout.EndHorizontal();
            }
        
            EditorGUILayout.EndScrollView();
        }
    
        void displayTextureImportProperties() {
        }
    
        void displayMeshImportProperties() {
        }
    
        void checkAndCreateFolder() {
            if (!File.Exists(m_pathToAssets)) {
                Directory.CreateDirectory(m_pathToAssets);
                AssetDatabase.Refresh();
            }
        }
    
        GameObject combineMesh(bool export) {
            GameObject returnObject = new GameObject(m_parentObject.name);
            returnObject.transform.position = m_parentObject.transform.position;
            returnObject.transform.rotation = m_parentObject.transform.rotation;

        
            MeshFilter[] filters = m_parentObject.GetComponentsInChildren<MeshFilter>();
            Matrix4x4 myTransform = m_parentObject.transform.worldToLocalMatrix;
        
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
                Material combined = TextureCombineUtility.combine(allMaterialTextures, out textureUVPositions, m_textureAtlasInfo);
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
            
                GameObject parent = new GameObject("Combined " + m_parentObject.name + " " + firstPass.Key + " Mesh Parent");
                parent.transform.position = m_parentObject.transform.position;
                parent.transform.rotation = m_parentObject.transform.rotation;
                parent.transform.parent = returnObject.transform;
    
                for (int i = 0; i < combinedMeshes.Length; i++) {
                    GameObject go = new GameObject("Combined " + m_parentObject.name + " Mesh");
                    go.transform.parent = parent.transform;
                    go.tag = m_parentObject.tag;
                    go.layer = m_parentObject.layer;
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localPosition = Vector3.zero;
                    MeshFilter filter = go.AddComponent<MeshFilter>();
                    go.AddComponent<MeshRenderer>();
                    go.GetComponent<Renderer>().sharedMaterial = mat;

                    filter.mesh = combinedMeshes [i];
                
                    if (export) {
                        checkAndCreateFolder();
                    
                        //(go.GetComponent<MeshRenderer>().material.mainTexture as Texture2D).format = TextureFormat.RGBA32;
                        Debug.Log((go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D).format);
                        if ((go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D).format != TextureFormat.ARGB32 && 
                            (go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D).format != TextureFormat.RGB24 &&
                            (go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D).format != TextureFormat.RGBA32) {
                            Debug.LogError("Textures assigned to objects must be either RGBA32 or RGB 24 to be exported");
                            return null;
                        }
            
                        byte[] texture = (go.GetComponent<MeshRenderer>().material.mainTexture as Texture2D).EncodeToPNG();
                        File.WriteAllBytes(m_pathToAssets + mat.shader.ToString().Remove(mat.shader.ToString().IndexOf("(", System.StringComparison.Ordinal)) + i + ".png", texture);
                        exportMesh(combinedMeshes [i], mat.shader.ToString().Remove(mat.shader.ToString().IndexOf("(", System.StringComparison.Ordinal)) + i);
                    }
                }
            }
        
            //if(developmentBake == true)
            //{
            foreach (Renderer r in m_parentObject.GetComponentsInChildren<Renderer>()) {
                r.enabled = false;
            }
            //}
        
            return returnObject;
        }
    
        void exportMesh(Mesh currentMesh, string assetName) {       
        
            StringBuilder sb = new StringBuilder();
    
            sb.Append("g ").Append(currentMesh.name).Append("\n");
            
            foreach (Vector3 v in currentMesh.vertices) {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in currentMesh.normals) {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector2 v in currentMesh.uv) {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            sb.Append("\n");
            foreach (Vector2 v in currentMesh.uv2) {
                sb.Append(string.Format("vt1 {0} {1}\n", v.x, v.y));
            }
            sb.Append("\n");
            foreach (Vector2 v in currentMesh.uv2) {
                sb.Append(string.Format("vt2 {0} {1}\n", v.x, v.y));
            }
            sb.Append("\n");
            foreach (Color c in currentMesh.colors) {
                sb.Append(string.Format("vc {0} {1} {2} {3}\n", c.r, c.g, c.b, c.a));
            }
        
            for (int j = 0; j < currentMesh.triangles.Length; j += 3) {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", 
                    currentMesh.triangles [j] + 1, currentMesh.triangles [j + 1] + 1, currentMesh.triangles [j + 2] + 1));
            }
        
            try {
                Debug.Log("Exporting mesh to " + m_pathToAssets + assetName + ".obj");
            
                checkAndCreateFolder();
                using (StreamWriter sw = new StreamWriter(m_pathToAssets + assetName + ".obj")) {
                    sw.WriteLine(sb.ToString());
                }
            } catch (System.Exception) {
            }
        

            AssetDatabase.Refresh();
        }
    
    
    }

    public class MeshImportSettings {
        public float globalScale;
        public bool addCollider;
        public float normalSmoothingAngle;
        public bool splitTangentsAcrossSeams;
        public bool swapUVChannels;
        public bool generateSecondaryUV;
        public bool optimizeMesh;
        public ModelImporterTangentSpaceMode normalImportMode;
        public ModelImporterTangentSpaceMode tangentImportMode;
        public ModelImporterMeshCompression meshCompression;
    
        public MeshImportSettings() {
            globalScale = 1.0f;
            addCollider = false;
            normalSmoothingAngle = 60.0f;
            splitTangentsAcrossSeams = true;
            swapUVChannels = false;
            generateSecondaryUV = false;
            optimizeMesh = false;
            normalImportMode = ModelImporterTangentSpaceMode.Import;
            tangentImportMode = ModelImporterTangentSpaceMode.Calculate;
            meshCompression = ModelImporterMeshCompression.Off;
        }
    }

    public class TextureImportSettings {
        public TextureImporterFormat textureFormat;
        public int maxTextureSize;
        public bool grayscaleToAlpha;
        public TextureImporterGenerateCubemap generateCubemap;
        public bool isReadable;
        public bool mipmapEnabled;
        public bool borderMipmap;
        public TextureImporterMipFilter mipmapFilter;
        public bool fadeout;
        public int mipmapFadeDistanceStart;
        public int mipmapFadeDistanceEnd;
        public bool generateMipsInLinearSpace;
        public TextureImporterNormalFilter normalmapFilter;
        public float heightmapScale;
        public int anisoLevel;
        public FilterMode filterMode;
        public TextureWrapMode wrapMode;
        public TextureImporterType textureType;
    
        public TextureImportSettings() {
            textureFormat = TextureImporterFormat.AutomaticTruecolor;
            maxTextureSize = (int)TextureSize.Unlimited;
            grayscaleToAlpha = false;
            generateCubemap = TextureImporterGenerateCubemap.None;
            isReadable = false;
            mipmapEnabled = true;
            borderMipmap = false;
            mipmapFilter = TextureImporterMipFilter.BoxFilter;
            fadeout = false;
            mipmapFadeDistanceStart = 1;
            mipmapFadeDistanceEnd = 1;
            generateMipsInLinearSpace = false;
            normalmapFilter = TextureImporterNormalFilter.Standard;
            heightmapScale = 0.25f;
            anisoLevel = 1;
            filterMode = FilterMode.Bilinear;
            wrapMode = TextureWrapMode.Repeat;
            textureType = TextureImporterType.Default;
        }
    }

    public enum TextureSize {
        _32 = 1 << 5,
        _64 = 1 << 6,
        _128 = 1 << 7,
        _256 = 1 << 8,
        _512 = 1 << 9,
        _1024 = 1 << 10,
        _2048 = 1 << 11,
        _4096 = 1 << 12,
        Unlimited
    }
}