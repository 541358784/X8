using System;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace Framework
{
    public class RenderTextureFactory : GlobalSystem<RenderTextureFactory>, IInitable
    {
        private const int MaxCountAKey = 2;
        private int antiAliasing = 2;

        public int AntiAliasing
        {
            get { return antiAliasing; }
        }

        private Dictionary<string, Stack<RenderTexture>> _unusedRenderTexture =
            new Dictionary<string, Stack<RenderTexture>>(4);

        private HashSet<RenderTexture> _usingRenderTexture = new HashSet<RenderTexture>();


        public void Init()
        {
        }

        public void Release()
        {
            try
            {
                foreach (var rt in _usingRenderTexture)
                {
                    rt.Release();
                }

                _usingRenderTexture.Clear();

                foreach (var p in _unusedRenderTexture)
                {
                    foreach (var rt in p.Value)
                    {
                        rt.Release();
                    }

                    p.Value.Clear();
                }

                _unusedRenderTexture.Clear();
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }


        public RenderTexture Create(string name, int width, int height, int depth)
        {
            RenderTexture rt = null;
            try
            {
                var key = _GenKey(width, height, depth);
                Stack<RenderTexture> rts;
                if (_unusedRenderTexture.TryGetValue(key, out rts))
                {
                    if (rts.Count > 0)
                    {
                        rt = rts.Pop();
                    }
                }

                if (rt == null)
                {
                    rt = new RenderTexture(width, height, depth);
                    rt.antiAliasing = antiAliasing;
                }

                rt.name = name;

                _usingRenderTexture.Add(rt);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return rt;
        }


        public void Destroy(RenderTexture rt)
        {
            try
            {
                if (rt == null)
                {
                    return;
                }

                if (_usingRenderTexture.Remove(rt))
                {
                    var key = _GenKey(rt.width, rt.height, rt.depth);

                    Stack<RenderTexture> rts;
                    if (_unusedRenderTexture.TryGetValue(key, out rts))
                    {
                    }
                    else
                    {
                        rts = new Stack<RenderTexture>(MaxCountAKey);
                        _unusedRenderTexture.Add(key, rts);
                    }

                    if (rts.Count < MaxCountAKey)
                    {
                        rts.Push(rt);
                    }
                    else
                    {
                        rt.Release();
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        public void ClearUnUsed()
        {
            try
            {
                foreach (var p in _unusedRenderTexture)
                {
                    foreach (var rt in p.Value)
                    {
                        rt.Release();
                    }

                    p.Value.Clear();
                }

                _unusedRenderTexture.Clear();
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }

        public void SetAntiAliasing(int level)
        {
            try
            {
                antiAliasing = level;
                foreach (var rt in _usingRenderTexture)
                {
                    rt.antiAliasing = level;
                }

                foreach (var p in _unusedRenderTexture)
                {
                    foreach (var rt in p.Value)
                    {
                        rt.antiAliasing = level;
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }
        }


        private string _GenKey(int width, int height, int depth)
        {
            return $"{width}_{height}_{depth}";
        }
    }
}