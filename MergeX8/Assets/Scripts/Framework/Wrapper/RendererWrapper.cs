using DragonU3DSDK;
using UnityEngine;

namespace Framework.Wrapper
{
    public class RendererWrapper
    {
        private readonly Renderer[] _renderers;
        private Material[][] _originalMaterials;
        private Shader[] _originalShaders;


        public RendererWrapper(Renderer[] renderers)
        {
            if (renderers == null)
            {
                DragonU3DSDK.DebugUtil.LogError("RendererWrapper: renderer is null");
            }

            _renderers = renderers;

            if (_renderers.Length > 0)
            {
                _originalMaterials = new Material[_renderers.Length][];
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _originalMaterials[i] = _renderers[i].materials;
                }
            }
        }

        public void SetVisible(bool visible)
        {
            if (_renderers != null)
            {
                foreach (var renderer in _renderers)
                {
                    if (renderer != null)
                    {
                        renderer.enabled = visible;
                    }
                }
            }
        }

        public void SetMaterialProperty(string name, Color value)
        {
            if (_renderers != null)
            {
                foreach (var renderer in _renderers)
                {
                    if (renderer != null)
                    {
                        var mpb = MaterialPropertyBlockVisitor.Instance;
                        mpb.SetColor(name, value);
                        renderer.SetPropertyBlock(mpb);
                    }
                }
            }
        }

        public void SetMaterial(Material material)
        {
            if (_renderers != null)
            {
                foreach (var renderer in _renderers)
                {
                    if (renderer != null)
                    {
                        if (renderer.materials != null && renderer.materials.Length > 0)
                        {
                            var mats = new Material[renderer.materials.Length];
                            for (int i = 0; i < mats.Length; i++)
                            {
                                mats[i] = material;
                            }

                            renderer.materials = mats;
                        }
                    }
                }
            }
        }

        public void ResetMaterial()
        {
            if (_originalMaterials != null && _renderers != null && _originalMaterials.Length != _renderers.Length)
            {
                DebugUtil.LogError(
                    $"{GetType()}.ResetMaterial, _originalMaterials.Length {_originalMaterials.Length} != _renderers.Length {_renderers.Length}");
                return;
            }

            for (int i = 0; i < _originalMaterials.Length; i++)
            {
                _renderers[i].materials = _originalMaterials[i];
            }
        }
    }
}