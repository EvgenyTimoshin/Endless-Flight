using UnityEngine;
using System.IO;
using System.Text;

namespace DCM.Old
{
		[System.Obsolete("This Class is obsolete")]
		public class AssetExporter : MonoBehaviour
		{

				public void exportTexture(Texture export, string path)
				{
						/*static void ExportNormalmap () {
	    var tex = Selection.activeObject as Texture2D;
	    if (tex == null) {
	        EditorUtility.DisplayDialog("No texture selected", "Please select a texture.", "Cancel");
	        return;
	    }
	    
	    // Force the texture to be readable so that we can access its pixels
	    var texPath = AssetDatabase.GetAssetPath(tex);
	    var texImport : TextureImporter = AssetImporter.GetAtPath(texPath);
	    if (!texImport.isReadable) {
	        texImport.isReadable = true;
	        AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
	    }
	    
	    var bytes = tex.EncodeToPNG();
	    var path = EditorUtility.SaveFilePanel("Save Texture", "", tex.name+"_normal.png", "png");
	    if (path != "") {
	        System.IO.File.WriteAllBytes(path, bytes);
	        AssetDatabase.RefreshDelayed(); // In case it was saved to the Assets folder
	    }
	}*/
				}
	
				public void exportTexture(Texture[] export, string path)
				{
				}
	
				public void exportMesh(Mesh export, string path)
				{
						Mesh m = export;
        
						StringBuilder sb = new StringBuilder();
        
						sb.Append("g ").Append(export.name).Append("\n");
						foreach(Vector3 v in m.vertices)
						{
								sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
						}
						sb.Append("\n");
						foreach(Vector3 v in m.normals)
						{
								sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
						}
						sb.Append("\n");
						foreach(Vector2 v in m.uv)
						{
								sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
						}
						sb.Append("\n");
						foreach(Vector2 v in m.uv2)
						{
								sb.Append(string.Format("vt1 {0} {1}\n", v.x, v.y));
						}
						sb.Append("\n");
						foreach(Vector2 v in m.uv2)
						{
								sb.Append(string.Format("vt2 {0} {1}\n", v.x, v.y));
						}
						sb.Append("\n");
						foreach(Color c in m.colors)
						{
								sb.Append(string.Format("vc {0} {1} {2} {3}\n", c.r, c.g, c.b, c.a));
						}
		
						for(int i=0; i<m.triangles.Length; i+=3)
						{
								sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", 
                    m.triangles[i] + 1, m.triangles[i + 1] + 1, m.triangles[i + 2] + 1));
            
						}
		
						try
						{
								using(StreamWriter sw = new StreamWriter(Application.dataPath + path))
								{
										sw.WriteLine(sb.ToString());
								}
						} catch(System.Exception)
						{
						}
				}
    
				public static void MeshToFile(MeshFilter mf, string filename, bool append)
				{
        
    
				}
	
				public void createObjectPrefab(GameObject prefab, string path)
				{
				}
	
				public void exportMaterial(Material export, string path)
				{
				}
	
				public void exportMaterial(Material[] export, string path)
				{
				}
		}
}
