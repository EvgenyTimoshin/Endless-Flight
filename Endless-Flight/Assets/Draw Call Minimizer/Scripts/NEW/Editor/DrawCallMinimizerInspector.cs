using UnityEngine;
using UnityEditor;
using DCM.ReorderableList;
using System.Linq;
using System.Collections.Generic;

namespace DCM {
    [CustomEditor(typeof(DrawCallMinimizer))]
    public class DrawCallMinimizerInspector : Editor {

        private DrawCallMinimizer drawCallTarget;
        private SerializedProperty destroyOriginalGameObject;
        private SerializedProperty skipAtlasingTextures;
        private SerializedProperty textureAtlasProperties;
        private DrawCallMinimizerInfo propertiesToLookFor;
        private bool _showAtlasProperties = true;
        private GUIContent[] sizeOptions = new GUIContent[] {
                new GUIContent("32"),
                new GUIContent("64"),
                new GUIContent("128"),
                new GUIContent("256"),
                new GUIContent("512"),
                new GUIContent("1024"),
                new GUIContent("2048"),
                new GUIContent("4096")
        };
        private int[] sizeOptionsValues = new int[] {
                32,
                64,
                128,
                256,
                512,
                1024,
                2048,
                4096
        };

        void OnEnable() {
            drawCallTarget = target as DrawCallMinimizer;
            destroyOriginalGameObject = serializedObject.FindProperty("destroyOriginalGameObject");
            textureAtlasProperties = serializedObject.FindProperty("_textureAtlasProperties");
            skipAtlasingTextures = serializedObject.FindProperty("skipTextureAtlasing");
            propertiesToLookFor = drawCallTarget.textureAtlasProperties;

            if (propertiesToLookFor == null) {
                propertiesToLookFor = new DrawCallMinimizerInfo();

            }
            IEnumerable<string> propertiesList = ShaderPropertyUtility.GetUniqueShaderPropertyNames(drawCallTarget.GetComponentsInChildren<Renderer>().Select(X => X.sharedMaterial).Distinct());

            //here we clear out properties that are no longer referenced
            IList<ShaderProperties> activeProperties = propertiesToLookFor.shaderPropertiesToLookFor.Where(x => propertiesList.Contains(x.propertyName)).ToList();

            //then we figure out which properties still need to be added to the list
            propertiesList = propertiesList.Where(x => !activeProperties.Select(property => property.propertyName).Contains(x));

            foreach (string s in propertiesList) {
                activeProperties.Add(new ShaderProperties(false, s));
            }

            propertiesToLookFor.shaderPropertiesToLookFor = activeProperties;
            serializedObject.ApplyModifiedProperties();
            ReimportNonReadonly();
            //Reimport anything that is needed
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(destroyOriginalGameObject, new GUIContent("Destroy Original Object", "Setting this to true will destroy the original game object that stored the original meshes. This is useful to clear up memory and other resources from being used. If you have scripts attached to the original object, or colliders, you will want this turned off"));

            EditorGUILayout.PropertyField(skipAtlasingTextures, new GUIContent("Skip Atlasing Textures", "Setting this to true will only combine objects using the same texture together, instead of trying to atlas common materials together"));

            if (!skipAtlasingTextures.boolValue) {
                _showAtlasProperties = EditorGUILayout.Foldout(_showAtlasProperties, new GUIContent("Atlas Properties"));
                if (_showAtlasProperties) {
                    DrawAtlasProperties();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the texture atlas properties for the user to manipulate
        /// </summary>
        void DrawAtlasProperties() {
            EditorGUI.indentLevel++;
            EditorGUILayout.IntSlider(textureAtlasProperties.FindPropertyRelative("_anisoLevel"), 0, 10, new GUIContent("Aniso Level"));
            EditorGUILayout.PropertyField(textureAtlasProperties.FindPropertyRelative("_readableTexture"), new GUIContent("Readable Atlas", "Set this to true if you want to read pixels from the atlas once it is formed, if you do not need this functionality, set it to false to free up resources"));
            EditorGUILayout.PropertyField(textureAtlasProperties.FindPropertyRelative("_filterMode"), new GUIContent("Filter Mode"));
            EditorGUILayout.PropertyField(textureAtlasProperties.FindPropertyRelative("_ignoreTransparency"), new GUIContent("No Transparency", "If set to true it will ignore all transparency when creating the texture atlas. This is for cases where the texture artist put a transparent background behind the texture when it wasn't needed etc."));
            EditorGUILayout.PropertyField(textureAtlasProperties.FindPropertyRelative("_wrapMode"), new GUIContent("Wrap Mode"));
            EditorGUILayout.IntPopup(textureAtlasProperties.FindPropertyRelative("_maxTextureSize"), sizeOptions, sizeOptionsValues, new GUIContent("Texture Size"));
            EditorGUILayout.PropertyField(textureAtlasProperties.FindPropertyRelative("_padding"), new GUIContent("Texture Padding", "This is the amount of pixels between each object in the texture. Increasing this can increase the size of your texture but it can help with mip maps"));
            EditorGUI.indentLevel--;
            ReorderableListGUI.Title(new GUIContent("Batched Shader Properties", "These are properties within the shader that DrawCallMinimizer will look for to determine which textures go together, and so you can batch more complex shaders together"));
            ReorderableListGUI.ListField<ShaderProperties>(drawCallTarget.textureAtlasProperties.shaderPropertiesToLookFor, PropertyItemDrawer, ReorderableListGUI.DefaultItemHeight * 2, ReorderableListFlags.DisableReordering | ReorderableListFlags.DisableDuplicateCommand | ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.HideAddButton);
        }

        /// <summary>
        /// A special method that gives the reorderable list a way to draw the GUI for the objects contained within it
        /// </summary>
        /// <returns>The item drawer.</returns>
        /// <param name="position">Position.</param>
        /// <param name="itemValue">Item value.</param>
        static private ShaderProperties PropertyItemDrawer(Rect position, ShaderProperties itemValue) {
            if (itemValue != null) {
                float originalHeight = position.height;
                float originalYPosition = position.y;
                position.height *= 0.5f;

                itemValue.markAsNormal = EditorGUI.Toggle(position, new GUIContent("Is Normal", "One thing to keep in mind is that textures cannot be set as a Unity normal map at run time currently, so if you are going to set this to true, make sure that you have a shader that can handle external normal maps. All this does is make sure that the texture is in the right format for these non native shaders"), itemValue.markAsNormal);
                position.y += position.height;
                itemValue.propertyName = EditorGUI.TextField(position, new GUIContent("Name", "This is the name of the property in the shader that you are references, as an example, the default property for diffuse shaders is _MainTex"), itemValue.propertyName);
                position.height = originalHeight;
                position.y = originalYPosition;
                position.width -= 50;


                position.x = position.xMax + 5;
                position.width = 45;
            }
            return itemValue;
        }

        /// <summary>
        /// Reimports all textures that do not have readonly turned off
        /// </summary>
        void ReimportNonReadonly() {
            ReimportNonReadableTextures ();
			ReimportNonReadableMeshes ();


        }

		void ReimportNonReadableTextures ()
		{
			IEnumerable<Texture> allTextures = drawCallTarget.GetComponentsInChildren<Renderer> ().SelectMany (X => ShaderPropertyUtility.GetAllTextures (X.sharedMaterial)).Distinct ();
			AssetPostprocessor postProcessor = new AssetPostprocessor ();
			float currentCount = 0.0f;
			int targetCount = allTextures.Count ();
			foreach (Texture t in allTextures) {
				EditorUtility.DisplayProgressBar ("Turning Off Readonly", "Analyzing texture " + ((int)currentCount + 1).ToString () + " of " + targetCount.ToString (), currentCount / targetCount);
				currentCount += 1;
				string assetPath = AssetDatabase.GetAssetPath (t);
				if (assetPath != null) {
					postProcessor.assetPath = assetPath;
					TextureImporter importer = (TextureImporter)postProcessor.assetImporter;
					if (importer != null && !importer.isReadable) {
						importer.isReadable = true;
						importer.textureFormat = TextureImporterFormat.ARGB32;
						AssetDatabase.ImportAsset (assetPath);
						AssetDatabase.Refresh ();
					}
				}
			}
			EditorUtility.ClearProgressBar ();
		}
		void ReimportNonReadableMeshes ()
		{
			IEnumerable<Mesh> allMeshes = drawCallTarget.GetComponentsInChildren<MeshFilter> ().Select(X => X.sharedMesh).Distinct();
			AssetPostprocessor postProcessor = new AssetPostprocessor ();
			float currentCount = 0.0f;
			int targetCount = allMeshes.Count ();
			foreach (Mesh m in allMeshes) {
				EditorUtility.DisplayProgressBar ("Allowing mesh to be readable", "Analyzing mesh " + ((int)currentCount + 1).ToString () + " of " + targetCount.ToString (), currentCount / targetCount);
				currentCount += 1;
				string assetPath = AssetDatabase.GetAssetPath (m);
				if (assetPath != null) {
					postProcessor.assetPath = assetPath;
					ModelImporter importer = (ModelImporter)postProcessor.assetImporter;
					if (importer != null && !importer.isReadable) {
						importer.isReadable = true;
						AssetDatabase.ImportAsset (assetPath);
						AssetDatabase.Refresh ();
					}
				}
			}
			EditorUtility.ClearProgressBar ();
		}
    }
}