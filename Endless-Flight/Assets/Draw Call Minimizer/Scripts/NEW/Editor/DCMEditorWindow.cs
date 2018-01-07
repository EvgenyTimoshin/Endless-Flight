using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DCM.ReorderableList;
using System.Linq;
using System.IO;

namespace DCM
{
	public class DCMEditorWindow : EditorWindow
	{
		private Dictionary<Material, Dictionary<Texture, List<MeshInstance>>> _dataTreeNoAtlasing = new Dictionary<Material, Dictionary<Texture, List<MeshInstance>>> ();
		private Dictionary<string, Dictionary<Material, List<MeshInstance>>> _dataTreeWithAtlasing = new Dictionary<string, Dictionary<Material, List<MeshInstance>>> ();
		private List<bool> _infoFoldOuts = new List<bool> ();
		private GameObject _hiddenCombinedObject;
		private Vector2 _scrollPosition;
		private int _maximumVertices = 65000;
		private int _numOfMeshes = 0;
		private int _numOfExportedMeshes = 0;
		private string _exportPath = string.Empty;
		private bool _isGeneratingLightmapUVs = false;
		private bool _isShowingOptions = true;
		private bool _isShowingObjectsList = true;
		private bool _isAtlasingTextures = true;
		private List<GameObject> _combinedObjects;
		private Transform _rootObject;
		Object _exportFolder = null;
		private DrawCallMinimizerInfo
			_textureAtlasProperties = new DrawCallMinimizerInfo ();
		private GUIContent[] sizeOptions = new GUIContent[] {
            new GUIContent ("32"),
            new GUIContent ("64"),
            new GUIContent ("128"),
            new GUIContent ("256"),
            new GUIContent ("512"),
            new GUIContent ("1024"),
            new GUIContent ("2048"),
            new GUIContent ("4096")
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

		[MenuItem("Window/Draw Call Minimizer/Editor Batcher")]
		static void Init ()
		{
			EditorWindow.GetWindow<DCMEditorWindow> (); 

		}
       
		[MenuItem("Window/Draw Call Minimizer/View Wiki")]
		static void OpenWiki ()
		{
			Application.OpenURL ("https://bitbucket.org/purdyjo/draw-call-minimizer/wiki/Home");
		}

		[MenuItem("Window/Draw Call Minimizer/Report Issue")]
		static void OpenIssues ()
		{
			Application.OpenURL ("https://bitbucket.org/purdyjo/draw-call-minimizer/issues");
		}

		[MenuItem("Window/Draw Call Minimizer/More By Purdyjo")]
		static void MoreByMe ()
		{
			Application.OpenURL ("http://purdyjotut.blogspot.ca/p/tools-and-projects.html");
		}

		void OnEnable ()
		{
			title = "Editor Batcher";
			_combinedObjects = new List<GameObject> ();
			InstantiateHiddenParentObject();
			_infoFoldOuts.Clear ();
		}

		void OnDestroy ()
		{
			if (PrefabUtility.GetPrefabObject (_hiddenCombinedObject) != null) {
				foreach (GameObject go in _combinedObjects) {
					go.SetActive (false);
				}

				_hiddenCombinedObject.hideFlags = HideFlags.None;
			} else {
				DestroyImmediate (_hiddenCombinedObject);
			}                        
		}

		private void OnGUI ()
		{
			ShowOptions ();

			ShowObjectsList ();

			if (CanExportMesh ()) {
				if (GUILayout.Button (new GUIContent ("Export"))) {
					_exportPath = AssetDatabase.GetAssetPath (_exportFolder) + "/";
					string realAssetPath = Application.dataPath.Remove (Application.dataPath.LastIndexOf ("Assets", System.StringComparison.Ordinal)) + _exportPath;

					ExportMeshes (realAssetPath);
					if (_isAtlasingTextures) {
						ExportTexturesAndMaterials (realAssetPath);
					}

					EditorWindow.GetWindow<DCMEditorWindow> ().Close (); 
				}   
			}

			_scrollPosition = EditorGUILayout.BeginScrollView (_scrollPosition);

			if (_infoFoldOuts.Count > 0) {
				EditorGUILayout.BeginVertical ("box");
				if (_isAtlasingTextures) {
					DrawAtlasedInformation ();
				} else {
					DrawUnAtlasedInformation ();
				}
				EditorGUILayout.EndVertical ();
			}
           
			EditorGUILayout.EndScrollView ();
		}

		void DrawAtlasProperties ()
		{
			EditorGUI.indentLevel++;
			_textureAtlasProperties.anisoLevel = EditorGUILayout.IntSlider (new GUIContent ("Aniso Level"), _textureAtlasProperties.anisoLevel, 0, 10);
			_textureAtlasProperties.readableTexture = EditorGUILayout.Toggle (new GUIContent ("Readable Atlas", "Set this to true if you want to read pixels from the atlas once it is formed, if you do not need this functionality, set it to false to free up resources"), _textureAtlasProperties.readableTexture);
			_textureAtlasProperties.filterMode = (FilterMode)EditorGUILayout.EnumPopup (new GUIContent ("Filter Mode"), _textureAtlasProperties.filterMode);
			_textureAtlasProperties.ignoreTransparency = EditorGUILayout.Toggle (new GUIContent ("No Transparency", "If set to true it will ignore all transparency when creating the texture atlas. This is for cases where the texture artist put a transparent background behind the texture when it wasn't needed etc."), _textureAtlasProperties.ignoreTransparency);
			_textureAtlasProperties.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup (new GUIContent ("Wrap Mode"), _textureAtlasProperties.wrapMode);
			_textureAtlasProperties.maxTextureSize = EditorGUILayout.IntPopup (new GUIContent ("Texture Size"), _textureAtlasProperties.maxTextureSize, sizeOptions, sizeOptionsValues);
			_textureAtlasProperties.padding = EditorGUILayout.IntField (new GUIContent ("Texture Padding", "This is the amount of pixels between each object in the texture. Increasing this can increase the size of your texture but it can help with mip maps"), _textureAtlasProperties.padding);
			EditorGUI.indentLevel--;
			ReorderableListGUI.Title (new GUIContent ("Batched Shader Properties", "These are properties within the shader that DrawCallMinimizer will look for to determine which textures go together, and so you can batch more complex shaders together"));
			ReorderableListGUI.ListField<ShaderProperties> (_textureAtlasProperties.shaderPropertiesToLookFor, PropertyItemDrawer, ReorderableListGUI.DefaultItemHeight * 2, ReorderableListFlags.DisableReordering | ReorderableListFlags.DisableDuplicateCommand | ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons);
		}

        
		/// <summary>
		/// A special method that gives the reorderable list a way to draw the GUI for the objects contained within it
		/// </summary>
		/// <returns>The item drawer.</returns>
		/// <param name="position">Position.</param>
		/// <param name="itemValue">Item value.</param>
		static private ShaderProperties PropertyItemDrawer (Rect position, ShaderProperties itemValue)
		{
			if (itemValue != null) {
				float originalHeight = position.height;
				float originalYPosition = position.y;
				position.height *= 0.5f;
                
				itemValue.markAsNormal = EditorGUI.Toggle (position, new GUIContent ("Is Normal", "One thing to keep in mind is that textures cannot be set as a Unity normal map at run time currently, so if you are going to set this to true, make sure that you have a shader that can handle external normal maps. All this does is make sure that the texture is in the right format for these non native shaders"), itemValue.markAsNormal);
				position.y += position.height;
				itemValue.propertyName = EditorGUI.TextField (position, new GUIContent ("Name", "This is the name of the property in the shader that you are references, as an example, the default property for diffuse shaders is _MainTex"), itemValue.propertyName);
				position.height = originalHeight;
				position.y = originalYPosition;
				position.width -= 50;
                
                
				position.x = position.xMax + 5;
				position.width = 45;
			}
			return itemValue;
		}

		/// <summary>
		/// Shows information on the screen related to the number of meshes that there will be once combined, and which meshes are being combined
		/// </summary>
		void DrawUnAtlasedInformation ()
		{
			List<Material> treeTopLevel = new List<Material> (_dataTreeNoAtlasing.Keys);

			for (int i = 0; i < _infoFoldOuts.Count; i++) {
				float numberOfVertices = 0.0f;
				foreach (KeyValuePair<Texture, List<MeshInstance>> kv in _dataTreeNoAtlasing[treeTopLevel[i]]) {
					foreach (MeshInstance mf in kv.Value) {
						numberOfVertices += mf.mesh.vertexCount;
					}
				}
				string itemName = treeTopLevel [i].ToString ();
				int removalIndex = treeTopLevel [i].ToString ().LastIndexOf (" (UnityEngine.Material)", System.StringComparison.Ordinal);
				if (removalIndex >= 0) {
					itemName = itemName.Remove (removalIndex);
				}

				if (GUILayout.Button (new GUIContent (itemName + " Will be combined into approx. " + (Mathf.CeilToInt (numberOfVertices / _maximumVertices)) + " meshes with a total of " + numberOfVertices + " vertices" + (_infoFoldOuts [i] ? string.Empty : " (Hidden)")), ReorderableListGUI.defaultTitleStyle, GUILayout.Height (20f))) {
					_infoFoldOuts [i] = !_infoFoldOuts [i];
				}

				if (_infoFoldOuts [i]) {
					GUILayout.Space (-6);
					EditorGUILayout.BeginVertical (ReorderableListGUI.defaultContainerStyle);

					foreach (KeyValuePair<Texture, List<MeshInstance>> kv in _dataTreeNoAtlasing[treeTopLevel[i]]) {
						EditorGUI.indentLevel++;
						foreach (MeshInstance mf in kv.Value) {
							EditorGUILayout.LabelField (mf.mesh.name);
						}
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.EndVertical ();
				}
			}
		}

		/// <summary>
		/// Shows information on which groups of materials are being combined, and information on the meshes using them
		/// </summary>
		void DrawAtlasedInformation ()
		{
			List<string> treeTopLevel = new List<string> (_dataTreeWithAtlasing.Keys);
            
			for (int i = 0; i < _infoFoldOuts.Count; i++) {
				float numberOfVertices = 0.0f;
				foreach (KeyValuePair<Material, List<MeshInstance>> kv in _dataTreeWithAtlasing[treeTopLevel[i]]) {
					foreach (MeshInstance mf in kv.Value) {
						numberOfVertices += mf.mesh.vertexCount;
					}
				}
				string itemName = treeTopLevel [i];
				int removalIndex = treeTopLevel [i].LastIndexOf (" (UnityEngine.Shader)", System.StringComparison.Ordinal);
				if (removalIndex >= 0) {
					itemName = itemName.Remove (removalIndex);
				}
                
				if (GUILayout.Button (new GUIContent (itemName + " objects will be combined into approx. " + (Mathf.CeilToInt (numberOfVertices / _maximumVertices)) + " meshes with a total of " + numberOfVertices + " vertices" + (_infoFoldOuts [i] ? string.Empty : " (Hidden)")), ReorderableListGUI.defaultTitleStyle, GUILayout.Height (25f))) {
					_infoFoldOuts [i] = !_infoFoldOuts [i];
				}
                
				if (_infoFoldOuts [i]) {
					GUILayout.Space (-6);
					EditorGUILayout.BeginVertical ();
                    
					foreach (KeyValuePair<Material, List<MeshInstance>> kv in _dataTreeWithAtlasing[treeTopLevel[i]]) {
						EditorGUILayout.LabelField (new GUIContent (kv.Key.name + " (" + kv.Value.Count + " Meshes)"), ReorderableListGUI.defaultTitleStyle);
						GUILayout.Space (-6);
						EditorGUILayout.BeginVertical ();
						EditorGUI.indentLevel++;
						foreach (MeshInstance mf in kv.Value) {
							EditorGUILayout.LabelField (mf.mesh.name);
						}
						EditorGUI.indentLevel--;
						EditorGUILayout.EndVertical ();
					}
					EditorGUILayout.EndVertical ();
				}
			}
		}

		/// <summary>
		/// Updates the foldouts count to match the currently viewed dictionary size. Also keeps some data for consistency
		/// </summary>
		private void UpdateFoldoutsCount ()
		{
			if (_isAtlasingTextures) {
				if (_infoFoldOuts.Count != _dataTreeWithAtlasing.Keys.Count) {
					if (_infoFoldOuts.Count > _dataTreeWithAtlasing.Keys.Count) {
						while (_infoFoldOuts.Count > _dataTreeWithAtlasing.Keys.Count) {
							_infoFoldOuts.RemoveAt (0);
						}
					} else if (_infoFoldOuts.Count < _dataTreeWithAtlasing.Keys.Count) {                                               
						while (_infoFoldOuts.Count < _dataTreeWithAtlasing.Keys.Count) {
							_infoFoldOuts.Add (false);
						}
					}
				}

			} else {
				if (_infoFoldOuts.Count != _dataTreeNoAtlasing.Keys.Count) {
					if (_infoFoldOuts.Count > _dataTreeNoAtlasing.Keys.Count) {
						while (_infoFoldOuts.Count > _dataTreeWithAtlasing.Keys.Count) {
							_infoFoldOuts.RemoveAt (0);
						}
					} else if (_infoFoldOuts.Count < _dataTreeNoAtlasing.Keys.Count) {                                               
						while (_infoFoldOuts.Count < _dataTreeNoAtlasing.Keys.Count) {
							_infoFoldOuts.Add (false);
						}
					}
				}		
			}
		}

		/// <summary>
		/// Shows the options for the texture atlas and the mesh combining options like maximum vertices etc.
		/// </summary>
		void ShowOptions ()
		{
			if (GUILayout.Button (new GUIContent ("Options" + (_isShowingOptions ? string.Empty : " (Hidden)")), ReorderableListGUI.defaultTitleStyle, GUILayout.Height (20f))) {
				_isShowingOptions = !_isShowingOptions;
			}
			if (_isShowingOptions) {
				GUILayout.Space (-6);

				EditorGUILayout.BeginVertical (ReorderableListGUI.defaultContainerStyle);
				EditorGUI.BeginChangeCheck ();
				_exportFolder = EditorGUILayout.ObjectField (new GUIContent ("Export Path", "Drag and drop the folder that you want the combined meshes exported to"), _exportFolder, typeof(Object), false);
				if (EditorGUI.EndChangeCheck ()) {
					if (_exportFolder.GetType () != typeof(Object)) {
						string assetPath = AssetDatabase.GetAssetPath (_exportFolder);
                        
						_exportFolder = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Object));
					}
				}

				if (_exportFolder != null) {
					EditorGUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					EditorGUILayout.LabelField (AssetDatabase.GetAssetPath (_exportFolder) + "/", ReorderableListGUI.defaultTitleStyle);    
					EditorGUILayout.EndHorizontal ();
				}

				EditorGUI.BeginChangeCheck ();
				_maximumVertices = EditorGUILayout.IntSlider (new GUIContent ("Maximum Vertices"), _maximumVertices, 1000, 65000);
				if (EditorGUI.EndChangeCheck ()) {
					UpdateFoldoutsCount ();
				}
				_isGeneratingLightmapUVs = EditorGUILayout.Toggle (new GUIContent ("Generate Lightmap UVs", "This will tell the importer to regenerate the lightmap UVs for the combined mesh"), _isGeneratingLightmapUVs);

               

				EditorGUI.BeginChangeCheck ();
				_isAtlasingTextures = EditorGUILayout.Toggle (new GUIContent ("Atlas Textures", "This will atlas textures together for similar materials"), _isAtlasingTextures);
                
				if (EditorGUI.EndChangeCheck ()) {
					CombineSelectedObjects ();
					UpdateFoldoutsCount ();
				}

				if (_isAtlasingTextures) {
					EditorGUI.indentLevel++;
					DrawAtlasProperties ();
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.EndVertical ();
			}
		}

		/// <summary>
		/// Shows the list of objects that you have chosen to combine
		/// </summary>
		private void ShowObjectsList ()
		{
			if (GUILayout.Button (new GUIContent ("Combined Objects" + (_isShowingObjectsList ? string.Empty : " (Hidden)")), ReorderableListGUI.defaultTitleStyle, GUILayout.Height (20f))) {
				_isShowingObjectsList = !_isShowingObjectsList;
			}

			if (_isShowingObjectsList) {
				GUILayout.Space (-6);
				EditorGUI.BeginChangeCheck ();
				ReorderableListGUI.ListField<GameObject> (_combinedObjects, PropertyItemDrawer);
				if (EditorGUI.EndChangeCheck ()) {
                           
					//get all shader properties
					IEnumerable<GameObject> validGameObjects = _combinedObjects.Select (currentObject => currentObject).
																				Where (currentObject => currentObject != null);
                    
					IEnumerable<Material> uniqueMaterials = validGameObjects.SelectMany (currentObject => currentObject.GetComponentsInChildren<Renderer> ().Select (currentRenderer => currentRenderer.sharedMaterial)).Distinct ();

					IEnumerable<string> propertiesList = ShaderPropertyUtility.GetUniqueShaderPropertyNames (uniqueMaterials);
                    
					//here we clear out properties that are no longer referenced
					List<ShaderProperties> activeShaderProperties = _textureAtlasProperties.shaderPropertiesToLookFor.Where (x => propertiesList.Contains (x.propertyName)).ToList ();
                    
					//then we figure out which properties still need to be added to the list
					propertiesList = propertiesList.Where (x => !activeShaderProperties.Select (property => property.propertyName).Contains (x));
                    
					foreach (string s in propertiesList) {
						activeShaderProperties.Add (new ShaderProperties (false, s));
					}
                    
					_textureAtlasProperties.shaderPropertiesToLookFor = activeShaderProperties;

					if (!_combinedObjects.Contains (null)) {
						Undo.RegisterCompleteObjectUndo (_combinedObjects.ToArray (), "UNDOOOOOO");
						CombineSelectedObjects ();
					}      
					UpdateFoldoutsCount ();

					IEnumerable<Mesh> allMeshes = validGameObjects.SelectMany (currentObject => currentObject.GetComponentsInChildren<MeshFilter> ().Select (currentMeshFilter => currentMeshFilter.sharedMesh)).Distinct ();

					ReimportNonReadableMeshes (allMeshes);
				}
			}
		}

		/// <summary>
		/// This is a special method that shows the reorderable list how to draw the GUI for the objects within the list
		/// </summary>
		/// <returns>The item drawer.</returns>
		/// <param name="position">Position.</param>
		/// <param name="itemValue">Item value.</param>
		static private GameObject PropertyItemDrawer (Rect position, GameObject itemValue)
		{
			position.width -= 50;   

			itemValue = (GameObject)EditorGUI.ObjectField (position, itemValue, typeof(GameObject), true);
			position.x = position.xMax + 5;
			position.width = 45;

			return itemValue;
		}

		/// <summary>
		/// Combines the selected objects together
		/// </summary>
		private void CombineSelectedObjects ()
		{
			InstantiateHiddenParentObject();
			ResetData ();

			if (_isAtlasingTextures) {
				BuildAtlasedMeshes ();
			} else {
				BuildUnatlasedMeshes ();
			}
		}

		/// <summary>
		/// Clears all data and starts things over so everything is funky fresh
		/// </summary>
		private void ResetData ()
		{
			_dataTreeNoAtlasing.Clear ();
			_dataTreeWithAtlasing.Clear ();

			_numOfMeshes = 0;
			Matrix4x4 myTransform = _hiddenCombinedObject.transform.worldToLocalMatrix;
			foreach (GameObject go in _combinedObjects) {
				if (go != null) {
					foreach (MeshFilter mf in go.GetComponentsInChildren<MeshFilter>()) {
						_numOfMeshes++;
						Material[] materials = mf.GetComponent<Renderer> ().sharedMaterials;
						for (int i = 0; i < materials.Length; i++) {
							MeshInstance currentInstance = new MeshInstance ();
							currentInstance.mesh = CloneMesh (mf.sharedMesh, i);
							currentInstance.transform = myTransform * mf.transform.localToWorldMatrix;
							currentInstance.subMeshIndex = Mathf.Min (i, currentInstance.mesh.subMeshCount - 1);
							Material currentMaterial = materials [i];

							FillInUnatlasedDictionary (currentInstance, currentMaterial);
							FillInAtlasedDictionary (currentInstance, currentMaterial);
						}
					}
				}
			}
		}

		/// <summary>
		/// Fills the in unatlased dictionary with information gathered from the currently selected meshes
		/// </summary>
		/// <param name="currentInstance">Current MeshInstance to add to the dictionary.</param>
		/// <param name="currentMaterial">Current material to use as a key and possibly add to the dictionary.</param>
		void FillInUnatlasedDictionary (MeshInstance currentInstance, Material currentMaterial)
		{
			Dictionary<Texture, List<MeshInstance>> meshesByTexture;
			if (currentMaterial.mainTexture == null) {
				currentMaterial.mainTexture = TextureCombineUtility.defaultTexture;
			}
			if (_dataTreeNoAtlasing.TryGetValue (currentMaterial, out meshesByTexture)) {
				List<MeshInstance> meshes;

				if (meshesByTexture.TryGetValue (currentMaterial.mainTexture, out meshes)) {
					meshes.Add (currentInstance);
				} else {
					meshesByTexture.Add (currentMaterial.mainTexture, new List<MeshInstance> {currentInstance});
				}
			} else {
				_dataTreeNoAtlasing.Add (currentMaterial, new Dictionary<Texture, List<MeshInstance>>{                  

                    {currentMaterial.mainTexture, new List<MeshInstance> {currentInstance}}
                });                                        
			}
		}

		/// <summary>
		/// Fills the in atlased dictionary with information gathered from the currently selected meshes
		/// </summary>
		/// <param name="currentInstance">Current MeshInstance to add to the dictionary.</param>
		/// <param name="currentMaterial">Current material to use as a key and possibly add to the dictionary.</param>
		void FillInAtlasedDictionary (MeshInstance currentInstance, Material currentMaterial)
		{
			Dictionary<Material, List<MeshInstance>> meshesByMaterial;
			if (currentMaterial.mainTexture == null) {
				currentMaterial.mainTexture = TextureCombineUtility.defaultTexture;
			}
			if (_dataTreeWithAtlasing.TryGetValue (currentMaterial.shader.ToString (), out meshesByMaterial)) {
				List<MeshInstance> meshes;
				if (meshesByMaterial.TryGetValue (currentMaterial, out meshes)) {
					meshes.Add (currentInstance);
				} else {
					meshesByMaterial.Add (currentMaterial, new List<MeshInstance>{currentInstance});
				}
			} else {
				_dataTreeWithAtlasing.Add (currentMaterial.shader.ToString (), new Dictionary<Material,  List<MeshInstance>>
                                          {{currentMaterial,new List<MeshInstance>{currentInstance}}});
			}
		}

		/// <summary>
		/// Builds the combined meshes using data from the unatlased dictionary
		/// </summary>
		void BuildUnatlasedMeshes ()
		{
			_numOfExportedMeshes = 0;
			foreach (KeyValuePair<Material, Dictionary<Texture, List<MeshInstance>>> dataByMaterial in _dataTreeNoAtlasing) {
				foreach (KeyValuePair<Texture, List<MeshInstance>> meshesByTexture in dataByMaterial.Value) {
					int index = 0;
					foreach (Mesh m in MeshCombineUtility.Combine(meshesByTexture.Value)) {
						GameObject go = new GameObject ("Combined " + dataByMaterial.Key.name + " Object " + index);
						go.AddComponent<MeshFilter> ().sharedMesh = m;
						go.AddComponent<MeshRenderer> ().sharedMaterial = dataByMaterial.Key;
						go.GetComponent<Renderer> ().sharedMaterial.mainTexture = meshesByTexture.Key;
                        
						go.transform.parent = _hiddenCombinedObject.transform;
						index++;
						_numOfExportedMeshes++;
					}
				}
			}
		}

		/// <summary>
		/// Builds the combined meshes using data from the atlased dictionary
		/// </summary>
		void BuildAtlasedMeshes ()
		{
			_numOfExportedMeshes = 0;
			foreach (KeyValuePair<string, Dictionary<Material, List<MeshInstance>>> firstPass in _dataTreeWithAtlasing) {
				List<Material> allMaterialTextures = new List<Material> (firstPass.Value.Keys);

				ReimportNonReadonlyTextures (allMaterialTextures);

				TextureCombineOutput textureCombineOutput = TextureCombineUtility.Combine (allMaterialTextures, _textureAtlasProperties, true);

				if (textureCombineOutput != null && textureCombineOutput.texturePositions != null) {
					List<MeshInstance> meshIntermediates = new List<MeshInstance> ();
					foreach (KeyValuePair<Material, List<MeshInstance>> kv in firstPass.Value) {
						TexturePosition refTexture = GetReferenceTexturePosition (textureCombineOutput.texturePositions, kv.Key.mainTexture.name);
                        
						for (int i = 0; i < kv.Value.Count; i++) {
							OffsetUvs (kv.Value [i].mesh, refTexture.position);
							meshIntermediates.Add (kv.Value [i]);
						}
					}
                    
					IList<Mesh> combinedMeshes = MeshCombineUtility.Combine (meshIntermediates);
					string objectName = firstPass.Key.Remove (firstPass.Key.LastIndexOf (' '));
					GameObject parent = new GameObject ("Combined " + objectName + " Mesh Parent");
					parent.transform.parent = _hiddenCombinedObject.transform;
					parent.transform.position = _hiddenCombinedObject.transform.position;
					parent.transform.rotation = _hiddenCombinedObject.transform.rotation;
					for (int i = 0; i < combinedMeshes.Count; i++) {
						GameObject go = new GameObject ("Combined " + objectName + " Mesh");
						go.transform.parent = parent.transform;
						go.transform.localScale = Vector3.one;
						go.transform.localRotation = Quaternion.identity;
						go.transform.localPosition = Vector3.zero;
						MeshFilter filter = go.AddComponent<MeshFilter> ();
						go.AddComponent<MeshRenderer> ().sharedMaterial = textureCombineOutput.combinedMaterial;
						filter.mesh = combinedMeshes [i];
						_numOfExportedMeshes++;
					}
				}
			}
		}

		void ReimportNonReadonlyTextures (IEnumerable<Material> allMaterials)
		{
			IEnumerable<Texture> allTextures = allMaterials.SelectMany (X => ShaderPropertyUtility.GetAllTextures (X)).Distinct ();
            
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

		void ReimportNonReadableMeshes (IEnumerable<Mesh> allMeshes)
		{
			allMeshes = allMeshes.Distinct ();
			
			AssetPostprocessor postProcessor = new AssetPostprocessor ();
			
			float currentCount = 0.0f;
			int targetCount = allMeshes.Count ();
			foreach (Mesh m in allMeshes) {
				EditorUtility.DisplayProgressBar ("Turning Off Readonly", "Analyzing mesh " + ((int)currentCount + 1).ToString () + " of " + targetCount.ToString (), currentCount / targetCount);
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


		/// <summary>
		/// Clears the combined meshes from the scene
		/// </summary>
		private void ClearCombinedMeshes ()
		{
			foreach (Transform t in _hiddenCombinedObject.transform) {
				DestroyImmediate (t.gameObject);
			}
		}

		/// <summary>
		/// Exports the combined meshes to the folder selected in the Editor Window
		/// </summary>
		/// <param name="realAssetPath">The real asset path to be used by System.IO</param>
		private void ExportMeshes (string realAssetPath)
		{
			string materialsPath = Path.Combine (_exportPath, "Materials");
			char[] invalidChars = Path.GetInvalidFileNameChars ();

			foreach (MeshFilter mf in _hiddenCombinedObject.GetComponentsInChildren<MeshFilter>()) {

				string filename = mf.name;

				foreach (char c in invalidChars) {
					filename.Replace (c, '_');
				}

				filename += ".obj";
				if (MeshExporter.MeshToFile (mf.sharedMesh, realAssetPath + filename)) {
					AssetPostprocessor proc = new AssetPostprocessor ();
					AssetDatabase.Refresh ();

					//even though it cant get here, should double check that export folder is null or not
					proc.assetPath = Path.Combine (AssetDatabase.GetAssetPath (_exportFolder), filename);

					ModelImporter importer = proc.assetImporter as ModelImporter;
                
					importer.generateSecondaryUV = _isGeneratingLightmapUVs;
					importer.generateAnimations = ModelImporterGenerateAnimations.None;
					importer.optimizeMesh = true;
					importer.importMaterials = false;

					AssetDatabase.ImportAsset (Path.Combine (_exportPath, filename), ImportAssetOptions.ForceUpdate);
					mf.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath (Path.Combine (_exportPath, filename), typeof(Mesh));

					AssetDatabase.DeleteAsset (Path.Combine (materialsPath, mf.sharedMesh.name));
					if (AssetDatabase.LoadAllAssetsAtPath (materialsPath).Length == 0) {
						FileUtil.DeleteFileOrDirectory (materialsPath);
					} 
				} else {
					Debug.LogError ("There was an issue exporting the mesh to " + realAssetPath + filename + ". Please double check your paths, names, and files. If this occurs frequently, create an issue, or vote for the existing issue if it already exists at (https://bitbucket.org/purdyjo/draw-call-minimizer/issues) and please include all information related to this issue to make the repair as fast and seamless as possible");
				}
			}

			AssetDatabase.Refresh ();
            
			PrefabUtility.CreatePrefab (_exportPath + "CombinedObject.prefab", _hiddenCombinedObject, ReplacePrefabOptions.ConnectToPrefab);

		}

		/// <summary>
		/// Exports the textures and materials of the atlased objects
		/// </summary>
		/// <param name="realAssetPath">Real asset path used by System.IO</param>
		private void ExportTexturesAndMaterials (string realAssetPath)
		{
			foreach (Transform t in _hiddenCombinedObject.transform) {
				Renderer currentRenderer = t.GetComponentInChildren<Renderer> ();
				if (currentRenderer != null) {
					foreach (ShaderProperties property in TextureCombineUtility.GetShaderProperties(currentRenderer.sharedMaterial, _textureAtlasProperties)) {
						currentRenderer.sharedMaterial.SetTexture (property.propertyName, ExportTexture (currentRenderer.sharedMaterial.GetTexture (property.propertyName), currentRenderer.name + property.propertyName, realAssetPath, property.markAsNormal));
					}

					currentRenderer.sharedMaterial = ExportMaterial (currentRenderer.sharedMaterial, currentRenderer.name + "_MAT");
				}
			}
		}

		/// <summary>
		/// Exports the texture to the selected folder
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="exportedTexture">Exported texture.</param>
		/// <param name="textureName">Texture name.</param>
		/// <param name="exportPath">Export path.</param>
		/// <param name="isNormal">If set to <c>true</c> the texture is imported as a normal map.</param>
		private Texture ExportTexture (Texture exportedTexture, string textureName, string realExportPath, bool isNormal)
		{
			using (FileStream f = new FileStream(realExportPath + textureName + ".png", FileMode.OpenOrCreate)) {
				using (BinaryWriter b = new BinaryWriter(f)) {
					b.Write (((Texture2D)exportedTexture).EncodeToPNG ());
				}
			}

			AssetPostprocessor proc = new AssetPostprocessor ();
			AssetDatabase.Refresh ();
			string texturePath = _exportPath + textureName + ".png";
			proc.assetPath = texturePath;

			TextureImporter importer = proc.assetImporter as TextureImporter;
			importer.anisoLevel = _textureAtlasProperties.anisoLevel;
			importer.normalmap = isNormal;
			importer.filterMode = _textureAtlasProperties.filterMode;
			importer.isReadable = _textureAtlasProperties.readableTexture;
			importer.maxTextureSize = _textureAtlasProperties.maxTextureSize;
			importer.wrapMode = _textureAtlasProperties.wrapMode;
			AssetDatabase.ImportAsset (texturePath, ImportAssetOptions.ForceUpdate);
			exportedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath (texturePath, typeof(Texture2D));
			AssetDatabase.Refresh ();
			return exportedTexture;
		}

		/// <summary>
		/// Exports the material used by the atlased combined meshes
		/// </summary>
		/// <returns>The exported material</returns>
		/// <param name="exportedMaterial">Exported material.</param>
		/// <param name="materialName">Material name.</param>
		private Material ExportMaterial (Material exportedMaterial, string materialName)
		{

			if (AssetDatabase.LoadAssetAtPath (_exportPath + materialName + ".mat", typeof(Material)) != null) {
				AssetDatabase.DeleteAsset (_exportPath + materialName + ".mat");
				AssetDatabase.Refresh ();
			}

			Material copy = Instantiate (exportedMaterial);
			AssetDatabase.CreateAsset (copy, _exportPath + materialName + ".mat");
			AssetDatabase.Refresh ();
			exportedMaterial = (Material)AssetDatabase.LoadAssetAtPath (_exportPath + materialName + ".mat", typeof(Material));

			return exportedMaterial;
		}

		/// <summary>
		/// Determines whether we can export the mesh. Checks to see if certain parameters are met, and if not, it gives warnings stating what the user needs to do.
		/// </summary>
		/// <returns><c>true</c> if this instance can export mesh; otherwise, <c>false</c>.</returns>
		private bool CanExportMesh ()
		{
			bool canExport = true;

			if (_combinedObjects == null || _combinedObjects.Count == 0) {
				canExport = false;
				if (_combinedObjects.Count == 0) {
					EditorGUILayout.HelpBox ("There are no objects being combined. Add some objects to the list", MessageType.Warning);
				}
			}

			for (int i = 0; i < _combinedObjects.Count; i++) {
				if (_combinedObjects [i] == null) {
					canExport = false;
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.HelpBox ("One of the objects in the Combined Objects list is null. Either set its value or remove it from the list", MessageType.Warning);
					if (GUILayout.Button (new GUIContent ("Auto Clear Null Objects"))) {
						AutoClearNullObjects ();
					}
					EditorGUILayout.EndHorizontal ();
					break;
				}
			}

			if (_exportFolder == null) {
				canExport = false;
				if (_exportFolder == null) {
					EditorGUILayout.HelpBox ("No export path has been set. Drag a folder into Export Path in the Options", MessageType.Warning);
				}
			}
			return canExport;
		}

		/// <summary>
		/// Goes through the list of objects being combined and clears out any nulls
		/// </summary>
		private void AutoClearNullObjects ()
		{
			for (int i = 0; i < _combinedObjects.Count; i++) {
				if (_combinedObjects [i] == null) {
					_combinedObjects.RemoveAt (i);
					i--;
				}
			}
		}

		/// <summary>
		/// Gets the reference texture position from a name of a texture
		/// </summary>
		/// <returns>The reference texture position.</returns>
		/// <param name="textureUVPositions">Texture UV positions.</param>
		/// <param name="textureName">Texture name.</param>
		static TexturePosition GetReferenceTexturePosition (IList<TexturePosition> textureUVPositions, string textureName)
		{
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
		/// Offsets the uvs so that the mesh will align with the new atlas
		/// </summary>
		/// <param name="modifiedMesh">Modified mesh.</param>
		/// <param name="referencedTexturePosition">Referenced texture position.</param>
		static void OffsetUvs (Mesh modifiedMesh, Rect referencedTexturePosition)
		{
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

		/// <summary>
		/// Clones a mesh so that all of it's properties are the same
		/// </summary>
		/// <returns>The cloned mesh.</returns>
		/// <param name="original">The original mesh</param>
		private Mesh CloneMesh (Mesh original, int subMesh)
		{
			Mesh newMesh = new Mesh ();

			newMesh.vertices = original.vertices;
			//newMesh.triangles = original.triangles;
			newMesh.triangles = original.GetTriangles (subMesh);
			newMesh.normals = original.normals;
			newMesh.colors32 = original.colors32;
			newMesh.uv = original.uv;
			newMesh.uv2 = original.uv2;
			newMesh.bounds = original.bounds;
			newMesh.subMeshCount = original.subMeshCount;
			newMesh.tangents = original.tangents;
			newMesh.hideFlags = HideFlags.DontSave;
			newMesh.name = original.name;
			return newMesh;
		}
		private void InstantiateHiddenParentObject()
		{
			if (_hiddenCombinedObject != null) {
				DestroyImmediate (_hiddenCombinedObject);
			}
			_hiddenCombinedObject = new GameObject ("CombinedObject");
			//_hiddenCombinedObject.hideFlags = HideFlags.HideAndDontSave;
        }
    }
    

}