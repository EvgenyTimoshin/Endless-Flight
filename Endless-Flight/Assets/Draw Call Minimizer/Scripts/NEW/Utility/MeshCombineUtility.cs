using UnityEngine;
using System.Collections.Generic;

namespace DCM {
    /// <summary>
    /// This is a static class that will combine all of your meshes and split them up based on shaders and the 65k limit on meshes
    /// </summary>
    public static class MeshCombineUtility {
        public static IList<Mesh> Combine(IList<MeshInstance> combines) {
            return Combine(combines, 65000);
        }

        /// <summary>
        /// Combine the specified meshes staying under the maximumVertices parameter
        /// </summary>
        /// <param name="combines">Combines.</param>
        /// <param name="maximumVertices">Maximum vertices.</param>
        public static IList<Mesh> Combine(IList<MeshInstance> combines, int maximumVertices) {
            float vertexCount = 0;
            int triangleCount = 0;
            
            for (int i = 0; i < combines.Count; i++) {
                if (combines [i].mesh != null) {
                    vertexCount += combines [i].mesh.vertexCount;
                    triangleCount += combines [i].mesh.triangles.Length;
                }
            }   
            
            int indexOffset = 0;
            int numOfSplits = Mathf.CeilToInt(vertexCount / maximumVertices);
            //Debug.Log (vertexCount + ", " + m_maximumVertices + ", " + numOfSplits);
            int vertsPerSplit = (int)(vertexCount / numOfSplits);
            List<Mesh> meshSplits = new List<Mesh>();      
            
            for (int i = 1; i <= numOfSplits; i++) {
                List<Vector3> vertices = new List<Vector3>(vertsPerSplit);
                List<Vector3> normals = new List<Vector3>(vertsPerSplit);
                List<Vector4> tangents = new List<Vector4>(vertsPerSplit);
                List<Vector2> uv = new List<Vector2>(vertsPerSplit);
                List<Vector2> uv1 = new List<Vector2>(vertsPerSplit);
                List<Color> colors = new List<Color>(vertsPerSplit);
                
                List<int> triangles = new List<int>(triangleCount / numOfSplits);
                
                int vertexOffset = 0;
                
                while (indexOffset < combines.Count && vertexOffset <= vertsPerSplit) {
                    if (combines [indexOffset].mesh != null) {
                        
                        Matrix4x4 invTranspose = combines [indexOffset].transform;
                        
                        Copy(combines [indexOffset].mesh.vertices, vertices, combines [indexOffset].transform);
                        invTranspose = invTranspose.inverse.transpose;
                       
                        if (combines [indexOffset].mesh.normals.Length != combines [indexOffset].mesh.vertexCount) { 
                            combines [indexOffset].mesh.RecalculateNormals();
                        }

                        CopyNormal(combines [indexOffset].mesh.normals, normals, invTranspose);                       

                        if (combines [indexOffset].mesh.tangents.Length != combines [indexOffset].mesh.vertexCount) { 
                            CalculateTangents(combines [indexOffset].mesh);
                        } 

                        CopyTangents(combines [indexOffset].mesh.tangents, tangents, invTranspose);
                        
                        Copy(combines [indexOffset].mesh.uv, uv);

                        if (combines [indexOffset].mesh.uv2.Length == combines [indexOffset].mesh.vertexCount) {
                            Copy(combines [indexOffset].mesh.uv2, uv1);
                        } else {
                            Vector2 defaultUV = Vector2.zero;
                            for (int j = 0; j < combines [indexOffset].mesh.vertexCount; j++) {
                                uv1.Add(defaultUV);// newUVs[j] = defaultUV;
                            }
                        }

                        if (combines [indexOffset].mesh.colors.Length == combines [indexOffset].mesh.vertexCount) {
                            CopyColors(combines [indexOffset].mesh.colors, colors);
                        } else {
                            Color white = Color.white;
                            for (int j = 0; j < combines [indexOffset].mesh.vertexCount; j++) {
                                colors.Add(white);
                            }
                        }
                        
                        CopyTriangles(combines [indexOffset].mesh.triangles, triangles, vertexOffset, (combines [indexOffset].transform.GetDeterminant() < 0));
                        
                        vertexOffset += combines [indexOffset].mesh.vertexCount;
                    }
                    
                    indexOffset++;
                }

                Mesh mesh = new Mesh();
                mesh.name = "Combined Mesh";
                mesh.vertices = vertices.ToArray();
                mesh.normals = normals.ToArray();
                mesh.colors = colors.ToArray();
                mesh.uv = uv.ToArray();
                mesh.uv2 = uv1.ToArray();
                mesh.tangents = tangents.ToArray();
                
                mesh.triangles = triangles.ToArray();
                
                meshSplits.Add(mesh);
            }   
            
            return meshSplits.ToArray();
        }
        
        static void Copy(Vector3[] src, ICollection<Vector3> dst, Matrix4x4 transform) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(transform.MultiplyPoint(src [i]));
            }
        }
        
        static void CopyNormal(Vector3[] src, ICollection<Vector3> dst, Matrix4x4 transform) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(transform.MultiplyVector(src [i]).normalized);
            }
        }
        
        static void Copy(Vector2[] src, ICollection<Vector2> dst) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(src [i]);
            }
        }
        
        static void CopyColors(Color[] src, ICollection<Color> dst) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(src [i]);
            }
        }
        
        static void CopyTangents(Vector4[] src, ICollection<Vector4> dst, Matrix4x4 transform) {
            for (int i = 0; i < src.Length; i++) {
                Vector4 p4 = src [i];
                Vector3 p = new Vector3(p4.x, p4.y, p4.z);
                p = transform.MultiplyVector(p).normalized;
                dst.Add(new Vector4(p.x, p.y, p.z, p4.w));
            }
        }
        
        static void CopyTriangles(int[] src, ICollection<int> dst, int vertexOffset, bool isNegativeScale) {            
            if (isNegativeScale) {
                for (int i = 0; i < src.Length; i+=3) {
                    dst.Add(src [i] + vertexOffset);
                    dst.Add(src [i + 2] + vertexOffset);
                    dst.Add(src [i + 1] + vertexOffset);
                }   
            } else {
                for (int i = 0; i < src.Length; i++) {
                    dst.Add(src [i] + vertexOffset);
                }       
            }
        }

        /// <summary>
        /// Calculates the tangents.
        /// 
        /// This function is a translation to C# from a very helpful user on the Unity forums known as noontz
        /// The original forum post with the javascript code is here http://forum.unity3d.com/threads/how-to-calculate-mesh-tangents.38984/
        /// </summary>
        /// <param name="theMesh">The mesh.</param>
        static void CalculateTangents(Mesh mesh) {
            int vertexCount = mesh.vertexCount;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Vector2[] texcoords = mesh.uv;
            int[] triangles = mesh.triangles;
            int triangleCount = triangles.Length / 3;
            Vector4[] tangents = new Vector4[vertexCount];
            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            int tri = 0;
            for (int  i = 0; i < (triangleCount); i++) {
                int i1 = triangles [tri];
                int i2 = triangles [tri + 1];
                int i3 = triangles [tri + 2];
                
                Vector3 v1 = vertices [i1];
                Vector3 v2 = vertices [i2];
                Vector3 v3 = vertices [i3];
                
                Vector2 w1 = texcoords [i1];
                Vector2 w2 = texcoords [i2];
                Vector2 w3 = texcoords [i3];
                
                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;
                
                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;
                
                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
                
                tan1 [i1] += sdir;
                tan1 [i2] += sdir;
                tan1 [i3] += sdir;
                
                tan2 [i1] += tdir;
                tan2 [i2] += tdir;
                tan2 [i3] += tdir;
                
                tri += 3;
            }
            
            for (int i = 0; i < (vertexCount); i++) {
                Vector3 n = normals [i];
                Vector3 t = tan1 [i];
                
                // Gram-Schmidt orthogonalize
                Vector3.OrthoNormalize(ref n, ref t);
                
                tangents [i].x = t.x;
                tangents [i].y = t.y;
                tangents [i].z = t.z;
                
                // Calculate handedness
                tangents [i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2 [i]) < 0.0f) ? -1.0f : 1.0f;
            }      
            mesh.tangents = tangents;
        }
    }
}