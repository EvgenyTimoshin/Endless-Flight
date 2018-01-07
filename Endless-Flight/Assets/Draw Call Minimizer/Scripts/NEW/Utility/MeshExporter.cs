using UnityEngine;
using System.IO;
using System.Text;

namespace DCM
{
	/// <summary>
	/// This class is a slightly modified version of the MeshExporter found on the Unity Community Wiki
	/// The difference here is that it reverses the triangles of a mesh and scales it to -1 in the X direction, because for some reason the mesh combining screws up the positions of the vertices, and this fixes it
	/// </summary>
	public class MeshExporter
	{
		public static string MeshToString (Mesh m)
		{                   
			StringBuilder sb = new StringBuilder ();
        
			sb.Append ("g Exported Mesh " + m.name + " \n");
			Matrix4x4 trs = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (-1, 1, 1));
			foreach (Vector3 v in m.vertices) {
				Vector3 vv = trs.MultiplyPoint (v);
				sb.Append (string.Format ("v {0} {1} {2}\n", vv.x, vv.y, vv.z));
			}
			sb.Append ("\n");
			foreach (Vector3 v in m.normals) {
				Vector3 vv = trs.MultiplyVector (v);
				sb.Append (string.Format ("vn {0} {1} {2}\n", vv.x, vv.y, vv.z));
			}
			sb.Append ("\n");
			foreach (Vector2 v in m.uv) {
				sb.Append (string.Format ("vt {0} {1}\n", v.x, v.y));
			}
			sb.Append ("\n");
			foreach (Vector2 v in m.uv2) {
				sb.Append (string.Format ("vt1 {0} {1}\n", v.x, v.y));
			}
			sb.Append ("\n");
			foreach (Vector2 v in m.uv2) {
				sb.Append (string.Format ("vt2 {0} {1}\n", v.x, v.y));
			}
			sb.Append ("\n");
			foreach (Color c in m.colors) {
				sb.Append (string.Format ("vc {0} {1} {2} {3}\n", c.r, c.g, c.b, c.a));
			}
        
			int[] meshTris = m.triangles;
			for (int i=0; i<meshTris.Length; i+=3) {
				sb.Append (string.Format ("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", 
                   // m.triangles [i] + 1, m.triangles [i + 1] + 1, m.triangles [i + 2] + 1));
                                        meshTris [i] + 1, meshTris [i + 2] + 1, meshTris [i + 1] + 1));
			}
			return sb.ToString ();
		}
    
		public static bool MeshToFile (Mesh m, string filename)
		{
			if (!Directory.Exists (Path.GetDirectoryName (filename))) {
				Directory.CreateDirectory (Path.GetDirectoryName (filename));
			}

			Debug.Log ("Exporting mesh to: " + filename);

			try {
				using (StreamWriter sw = new StreamWriter(filename)) {
					sw.WriteLine (MeshToString (m));
				}

				return true;
			} catch (System.Exception e) {
				Debug.LogError (e.Message);
				return false;
			}
		}
	}
}
