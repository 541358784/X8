using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

 
    public class ShaderReplacer
    {
        private Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();


        public void Replace(Material mat)
        {
            try
            {
                if (mat)
                {
                    for (int i = 0; i < ShaderReplaceConfig.replaceShaderPair.Length; i++)
                    {
                        if (mat.shader.name == ShaderReplaceConfig.replaceShaderPair[i][0])
                        {
                            var shader = GetShader(ShaderReplaceConfig.replaceShaderPair[i][1]);
                            if (shader != null)
                            {
                                mat.shader = shader;
                                Debug.Log($"{GetType()}: shader replaced, mat name = {mat.name}, {ShaderReplaceConfig.replaceShaderPair[i][0]} -> {mat.shader.name}");
                                EditorUtility.SetDirty(mat);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
        }
        
        public Shader GetShader(string name)
        {
            Shader shader;
            if (shaders.TryGetValue(name, out shader))
            {
                return shader;
            }
            else
            {
                shader = Shader.Find(name);
                if (shader != null)
                {
                    shaders.Add(name, shader);
                }
                return shader;
            }
        }


        private static ShaderReplacer _instance;

        public static ShaderReplacer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShaderReplacer();
                }

                return _instance;
            }
        }
        
    }