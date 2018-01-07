using UnityEngine;

namespace DCM {
    /// <summary>
    /// A few extension methods that are used throughout DrawCallMinimizer
    /// </summary>
    public static class MatrixExtension {

        public static Vector3 GetScale(this Matrix4x4 mat) {
            return new Vector3(Vector3.Magnitude(new Vector3(mat.m00, mat.m01, mat.m02)),
            Vector3.Magnitude(new Vector3(mat.m10, mat.m11, mat.m12)),
            Vector3.Magnitude(new Vector3(mat.m20, mat.m21, mat.m22)));
        }
    
        public static float GetDeterminant(this Matrix4x4 mat) {
            return (mat.m00 * mat.m11 * mat.m22) + 
                (mat.m01 * mat.m12 * mat.m20) + 
                (mat.m02 * mat.m10 * mat.m21) - 
                (mat.m02 * mat.m11 * mat.m20) - 
                (mat.m01 * mat.m10 * mat.m22) - 
                (mat.m00 * mat.m12 * mat.m21);
        }
    }
}