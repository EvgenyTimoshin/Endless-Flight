using UnityEngine;
using System.Collections.Generic;

namespace DCM.Old {
    [System.Obsolete("This Class is obsolete")]
    public class MeshCombineUtility {   
        public struct MeshInstance {
            public Mesh      mesh;
            public int       subMeshIndex;
            public Matrix4x4 transform;
        }
    
        public static Mesh[] Combine(MeshInstance[] combines) {
            int vertexCount = 0;
            int triangleCount = 0;

            for (int i = 0; i < combines.Length; i++) {
                if (combines [i].mesh != null) {
                    vertexCount += combines [i].mesh.vertexCount;
                    triangleCount += combines [i].mesh.GetTriangles(combines [i].subMeshIndex).Length;
                }
            }   
        
            int indexOffset = 0;
            int numOfSplits = Mathf.CeilToInt(vertexCount / 60000.0f);
            int vertsPerSplit = vertexCount / numOfSplits;
            List<Mesh> meshSplits = new List<Mesh>();       
        
            for (int i = 1; i <= numOfSplits; i++) {
                List<Vector3> vertices = new List<Vector3>(vertsPerSplit);
                List<Vector3> normals = new List<Vector3>(vertsPerSplit);
                List<Vector4> tangents = new List<Vector4>(vertsPerSplit);
                List<Vector2> uv = new List<Vector2>(vertsPerSplit);
                List<Vector2> uv1 = new List<Vector2>(vertsPerSplit);
                List<Vector2> uv2 = new List<Vector2>(vertsPerSplit);
                List<Color> colors = new List<Color>(vertsPerSplit);
        
                List<int> triangles = new List<int>(triangleCount / numOfSplits);
        
                int vertexOffset = 0;
            
                while (indexOffset < combines.Length && vertexOffset <= vertsPerSplit) {
                    if (combines [indexOffset].mesh != null) {
                        Copy(combines [indexOffset].mesh.vertices, vertices, combines [indexOffset].transform);
                    
                        Matrix4x4 invTranspose = combines [indexOffset].transform;
                        invTranspose = invTranspose.inverse.transpose;
                        CopyNormal(combines [indexOffset].mesh.normals, normals, invTranspose);
                    
                        CopyTangents(combines [indexOffset].mesh.tangents, tangents, invTranspose);
                    
                        Copy(combines [indexOffset].mesh.uv, uv);
                    
                        Copy(combines [indexOffset].mesh.uv2, uv1);
                    
                        Copy(combines [indexOffset].mesh.uv2, uv2);
                        if (combines [indexOffset].mesh.colors.Length == combines [indexOffset].mesh.vertexCount) {
                            CopyColors(combines [indexOffset].mesh.colors, colors);
                        } else {
                            Color[] newColors = new Color[combines [indexOffset].mesh.vertexCount];
                            for (int x = 0; x < newColors .Length; x++) {
                                newColors [x] = Color.white;
                            }
                            CopyColors(newColors, colors);
                        }
                    
                        CopyTriangles(combines [indexOffset].mesh.GetTriangles(combines [indexOffset].subMeshIndex), triangles, vertexOffset);
                                    
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
                mesh.uv2 = uv2.ToArray();
                mesh.tangents = tangents.ToArray();
        
                mesh.triangles = triangles.ToArray();
            
                meshSplits.Add(mesh);
            }   
        
            return meshSplits.ToArray();
        }
    
        static void Copy(Vector3[] src, List<Vector3> dst, Matrix4x4 transform) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(transform.MultiplyPoint(src [i]));
            }
        }

        static void CopyNormal(Vector3[] src, List<Vector3> dst, Matrix4x4 transform) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(transform.MultiplyVector(src [i]).normalized);
            }
        }

        static void Copy(Vector2[] src, List<Vector2> dst) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(src [i]);
            }
        }

        static void CopyColors(Color[] src, List<Color> dst) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(src [i]);
            }
        }
    
        static void CopyTangents(Vector4[] src, List<Vector4> dst, Matrix4x4 transform) {
            for (int i = 0; i < src.Length; i++) {
                Vector4 p4 = src [i];
                Vector3 p = new Vector3(p4.x, p4.y, p4.z);
                p = transform.MultiplyVector(p).normalized;
                dst.Add(new Vector4(p.x, p.y, p.z, p4.w));
            }
        }
    
        static void CopyTriangles(int[] src, List<int> dst, int vertexOffset) {
            for (int i = 0; i < src.Length; i++) {
                dst.Add(src [i] + vertexOffset);
            }       
        }
    }
}