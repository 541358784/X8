using System;
using System.Collections.Generic;
using System.IO;
using LevelEditor;
using Newtonsoft.Json;
using Screw.Configs;
using UnityEditor;
using UnityEngine;

namespace Screw.Editor
{
    public class GenerateLevelData : MonoBehaviour
    {
        public int _levelId;

        public List<ColorType> _layerColors = new List<ColorType>();

        public List<Order> _orders = new List<Order>();


        public void GenerateLevel()
        {
            LevelLayout layout = new LevelLayout();
            layout.levelId = _levelId;

            Fill_Layout_Guide(layout);
            Fill_Layout_Order(layout);
            Fill_Layout_Layer(layout);
            Fill_Layout_Shield(layout);
            
            WriteToFile(layout);
        }

        private void Fill_Layout_Guide(LevelLayout layout)
        {
            var guideObj = GameObject.Find("TutorialText");
            if (guideObj == null || guideObj.GetComponent<TextMesh>() == null)
                return;

            var pos = guideObj.GetComponent<RectTransform>().anchoredPosition3D;
            layout.guidePosition = new Vector3Float(pos);
        }

        private void Fill_Layout_Order(LevelLayout layout)
        {
            for (int i = 0; i < _orders.Count; i++)
            {
                Order order = new Order();
                order.colorType = _orders[i].colorType;
                order.slotCount = _orders[i].slotCount;
                order.shapes = new List<ScrewShape>(_orders[i].shapes);

                layout.orders.Add(order);
            }
        }

        private void Fill_Layout_Layer(LevelLayout layout)
        {
            var layerRoot = transform.Find("Layers");

            for (int i = 0; i < layerRoot.childCount; i++)
            {
                var layerTr = layerRoot.GetChild(i);
                var screwsRoot = layerTr.Find("Screws");
                var panelsRoot = layerTr.Find("Panels");

                LevelLayer layer = new LevelLayer();
                Fill_Layer_Panel(layer, panelsRoot, i);
                Fill_Layer_Screw(layer, screwsRoot);

                layout.layers.Add(layer);
            }
        }

        private void Fill_Layer_Panel(LevelLayer layer, Transform panelRoot, int index)
        {
            if(panelRoot == null)
                return;
            
            for (int i = 0; i < panelRoot.childCount; i++)
            {
                var panelTr = panelRoot.GetChild(i);

                LevelPanel panel = new LevelPanel();

                panel.instanceId = panelTr.GetInstanceID();

                var bodyTr = panelTr.Find("Body");
                panel.scale = new Vector3Float(panelTr.localScale);
                panel.position = new Vector3Float(panelTr.position);
                panel.rotate = new Vector3Float(panelTr.localEulerAngles);

                panel.bodyScale = new Vector3Float(bodyTr.localScale);
                panel.bodyRotate = new Vector3Float(bodyTr.localEulerAngles);
                panel.bodyImageName = bodyTr.GetComponent<SpriteRenderer>().sprite.name;

                var shadowTr = panelTr.Find("Shadow");

                panel.shadowPosition = new Vector3Float(shadowTr.position);

                var colorProvider = panelTr.GetComponent<ColorProvider>();
                if (colorProvider)
                {
                    if (index < _layerColors.Count)
                    {
                        colorProvider.colorType = _layerColors[index];
                    }

                    panel.colorType = colorProvider.colorType;
                }
                else
                    Debug.LogError("木板Body下没有ColorProvider组件！！！！！！！！！！！");

                var holes = panelTr.GetComponentsInChildren<HoleProvider>();
                for (int j = 0; j < holes.Length; j++)
                {
                    LevelHole hole = new LevelHole();

                    hole.instanceId = holes[j].GetInstanceID();
                    hole.position = new Vector3Float(holes[j].transform.position);

                    panel.holes.Add(hole);
                }

                layer.panels.Add(panel);
            }
        }

        private void Fill_Layer_Screw(LevelLayer layer, Transform screwRoot)
        {
#if UNITY_EDITOR
            if(screwRoot == null)
                return;
            
            for (int i = 0; i < screwRoot.childCount; i++)
            {
                var screwTr = screwRoot.GetChild(i);

                var levelScrew = new LevelScrew();
                levelScrew.instanceID = screwTr.GetInstanceID();
                levelScrew.position = new Vector3Float(screwTr.position);

                levelScrew.shape = screwTr.GetComponent<ScrewProvider>().screwType;
                levelScrew.colorType = screwTr.GetComponent<ColorProvider>().colorType;

                var component = screwTr.GetComponent<CircleCollider2D>();
                levelScrew.radius = component.radius;

                var connectProvider = screwTr.GetComponent<ConnectProvider>();
                if (connectProvider != null && connectProvider.linkScrews != null && connectProvider.linkScrews.Count > 0)
                {
                    for (int j = 0; j < connectProvider.linkScrews.Count; j++)
                    {
                        var screwId = connectProvider.linkScrews[j].GetInstanceID();

                        LevelScrewBlock screwBlock = new LevelScrewBlock();
                        screwBlock.blockType = ScrewBlocker.ConnectBlocker;
                        screwBlock.connetIds.Add(screwId);
                        
                        levelScrew.screwBlocks.Add(screwBlock);
                    }
                }

                var iceBlockProvider = screwTr.GetComponent<IceProvider>();
                if (iceBlockProvider != null)
                {
                    LevelScrewBlock screwBlock = new LevelScrewBlock();
                    screwBlock.blockType = ScrewBlocker.IceBlocker;
                        
                    levelScrew.screwBlocks.Add(screwBlock);
                }

                var shutterProvider = screwTr.GetComponent<ShutterProvider>();
                if (shutterProvider != null)
                {
                    LevelScrewBlock screwBlock = new LevelScrewBlock();
                    screwBlock.blockType = ScrewBlocker.ShutterBlocker;
                    screwBlock.isOpen = shutterProvider.isOpen;
                        
                    levelScrew.screwBlocks.Add(screwBlock);
                }

                var bombProvider = screwTr.GetComponent<BombProvider>();
                if (bombProvider != null)
                {
                    LevelScrewBlock screwBlock = new LevelScrewBlock();
                    screwBlock.blockType = ScrewBlocker.BombBlocker;
                    screwBlock.stageCount = bombProvider.stageCount;
                        
                    levelScrew.screwBlocks.Add(screwBlock);
                }

                var lockProvider = screwTr.GetComponent<LockProvider>();
                if (lockProvider != null)
                {
                    LevelScrewBlock screwBlock = new LevelScrewBlock();
                    screwBlock.blockType = ScrewBlocker.LockBlocker;
                    screwBlock.keyId = lockProvider.keyTarget.GetInstanceID();
                        
                    levelScrew.screwBlocks.Add(screwBlock);
                }

                var tieProvider = screwTr.GetComponent<TieProvider>();
                if (tieProvider != null)
                {
                    LevelScrewBlock screwBlock = new LevelScrewBlock();
                    screwBlock.blockType = ScrewBlocker.TieBlocker;
                    for (int j = 0; j < tieProvider.tieJams.Count; j++)
                    {
                        screwBlock.tieIds.Add(tieProvider.tieJams[j].GetInstanceID());
                    }
                        
                    levelScrew.screwBlocks.Add(screwBlock);
                }
                
                layer.screws.Add(levelScrew);
            }
            #endif
        }

        private void Fill_Layout_Shield(LevelLayout layout)
        {
            var layerRoot = transform.Find("Layers");

            for (int i = 0; i < layerRoot.childCount; i++)
            {
                var layerTr = layerRoot.GetChild(i);
                var shieldsRoot = layerTr.Find("Shields");
                if(shieldsRoot == null)
                    continue;

                Fill_Layer_Shield(layout, i, shieldsRoot);
            }
        }

        private void Fill_Layer_Shield(LevelLayout layout, int layerIndex, Transform shieldRoot)
        {
            if(layerIndex >= layout.layers.Count)
                return;
            
            var layer = layout.layers[layerIndex];
            
            for (int i = 0; i < shieldRoot.childCount; i++)
            {
                var shieldTr = shieldRoot.GetChild(i);

                LevelShield shield = new LevelShield();

                shield.instanceId = shieldTr.GetInstanceID();

                var bodyTr = shieldTr.Find("Body");
                shield.scale = new Vector3Float(shieldTr.localScale);
                shield.position = new Vector3Float(shieldTr.position);
                shield.rotate = new Vector3Float(shieldTr.localEulerAngles);

                shield.bodyScale = new Vector3Float(bodyTr.localScale);
                shield.bodyRotate = new Vector3Float(bodyTr.localEulerAngles);
                shield.bodyImageName = bodyTr.GetComponent<SpriteRenderer>().sprite.name;
                
                PolygonCollider2D shieldCollider = shieldTr.transform.GetComponent<PolygonCollider2D>();

                shield.coverPanelIds = new List<int>(CalculateCoverPanel(layerIndex, shieldCollider));
                layer.shields.Add(shield);
            }
        }

        private List<int> CalculateCoverPanel(int layerIndex, PolygonCollider2D shieldCollider)
        {
            List<int> cover = new List<int>();
            
            var layerRoot = transform.Find("Layers");

            for (int i = 0; i < layerRoot.childCount; i++)
            {
                if(i <= layerIndex)
                    continue;
                
                var layerTr = layerRoot.GetChild(i);
                var panelsRoot = layerTr.Find("Panels");

                for (int j = 0; j < panelsRoot.childCount; j++)
                {
                    var panelTr = panelsRoot.GetChild(j);

                    PolygonCollider2D panelCollider = panelTr.transform.GetComponent<PolygonCollider2D>();
                    if(panelCollider == null)
                        continue;
                
                    Bounds bounds1 = panelCollider.bounds;
                    Bounds bounds2 = shieldCollider.bounds;

                    if (!bounds1.Intersects(bounds2))
                        continue;
                    
                    cover.Add(panelTr.GetInstanceID());
                }
            }

            return cover;
        }
        public void WriteToFile(LevelLayout layout, Formatting formatting = Formatting.None)
        {
#if UNITY_EDITOR
            var saveText = JsonConvert.SerializeObject(layout, formatting);

            string folderName = "Group{0}";
            if (_levelId < 10000)
            {
                folderName = string.Format(folderName, 0);
            }
            else
            {
                int group = int.Parse(_levelId.ToString()[0].ToString());
                folderName = string.Format(folderName, group);
            }

            string folderPath = ConstConfig.FolderNameConfigPath(folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            folderPath += $"/{_levelId.ToString()}.json";
        
            StreamWriter sw = null;
            try
            {
                FileInfo t = new FileInfo(folderPath);
                DirectoryInfo dir = t.Directory;
                if (!dir.Exists)
                {
                    dir.Create();
                }
        
                sw = t.CreateText();
                //以行的形式写入信息
                sw.WriteLine(saveText);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                //关闭流
                sw?.Close();
                //销毁流
                sw?.Dispose();
            }
            AssetDatabase.Refresh();
#endif
        }
    }
}