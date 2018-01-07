using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
public static class ShaderPropertyUtility {

    /// <summary>
    /// Uses shader util to get all of the textures referenced by this material
    /// </summary>
    /// <returns>The all textures.</returns>
    /// <param name="mat">Mat.</param>
    public static IEnumerable<Texture> GetAllTextures(Material mat) {
        List<Texture> allTextures = new List<Texture>();
        
        int count = ShaderUtil.GetPropertyCount(mat.shader);
        
        for (int i = 0; i < count; i++) {
            if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
                string propertyName = ShaderUtil.GetPropertyName(mat.shader, i);
                allTextures.Add(mat.GetTexture(propertyName));
            }
        }

        return allTextures;
    }

    public static IEnumerable<string> GetUniqueShaderPropertyNames(IEnumerable<Material> materials)
    {
        return GetUniqueShaderPropertyNames(materials.Select(x => x.shader));
    }

    public static IEnumerable<string> GetUniqueShaderPropertyNames(IEnumerable<Shader> shaders)
    {
        shaders = shaders.Distinct();

        List<string> shaderProperties = new List<string>();
        foreach (Shader currentShader in shaders)
        {
            int count = ShaderUtil.GetPropertyCount(currentShader);

            for(int i = 0; i < count; i++)
            {
                if (ShaderUtil.GetPropertyType(currentShader, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
                    shaderProperties.Add(ShaderUtil.GetPropertyName(currentShader, i));
                }
            }
        }

        return shaderProperties.Distinct();
    }
}
