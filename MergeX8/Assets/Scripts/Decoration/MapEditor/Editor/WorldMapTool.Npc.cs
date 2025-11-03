using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using SomeWhere;
using Spine.Unity;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class WorldMapTool
{


    private void drawNpcScope()
    {
        if (GUILayout.Button("创建npc停靠点"))
        {
            var pointsRoot = GameObject.Find($"WorldMap/MapLayer/Viewport/Content/PathMap/PointsRoot").transform;
            {
               
            }
        }

        if (_testNpc == null)
        {
            if (GUILayout.Button("加载测试npc"))
            {
                _testNpc = GameObject.Find($"WorldMap/MapLayer/Viewport/Content/PathMap/Npc1001")?.GetComponent<IsometricMoving>();
                if (_testNpc == null)
                {
                    var pathMap = GameObject.Find($"WorldMap/MapLayer/Viewport/Content/PathMap").transform;
                    {
                        var npcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format(TEST_NPC_PATH));
                        if (npcPrefab != null)
                        {
                            _testNpc = (PrefabUtility.InstantiatePrefab(npcPrefab, pathMap) as GameObject).GetComponent<IsometricMoving>();
                        }
                    }
                }
            }
        }
        else
        {
            var newTestingToggle = GUILayout.Toggle(NpcTesting, "Npc移动");
            if (newTestingToggle != NpcTesting)
            {
                NpcTesting = newTestingToggle;
                if (NpcTesting)
                {
                    testNpcMove();
                }
            }

            if (GUILayout.Button("移除测试Npc"))
            {
                GameObject.DestroyImmediate(_testNpc);
            }
        }

    }

    private async void testNpcMove()
    {
        /*
        var _pathMap = GameObject.Find($"WorldMap/MapLayer/Viewport/Content/PathMap").GetComponent<PathMap>();
        var i = 0;
        var _currentSegment = _pathMap.segmentList[i];
        var point = _currentSegment.GetEnd(false);
        _testNpc.SetPosition(point.Position);
        var _skeletonAnimation = _testNpc.GetComponentInChildren<SkeletonAnimation>();
        var _currentJurney = 0f;
        var deltaTime = 100f;
        var npcPlayMoving = false;
        while (NpcTesting)
        {
            if (!npcPlayMoving && _skeletonAnimation.AnimationState != null)
            {
                _skeletonAnimation.AnimationState?.SetAnimation(0, "walk", true);
                npcPlayMoving = true;
            }

            var nextPos = _pathMap.GetBezierPointWithDistance(_currentSegment, _testNpc.transform.position, ref _currentJurney, deltaTime / 1000f, false);
            _testNpc.SetPosition(nextPos);

            if (_currentJurney >= 1f)
            {
                var nextIndex = ++i;
                if (nextIndex >= _pathMap.segmentList.Count)
                {
                    nextIndex = 0;
                }

                _currentSegment = _pathMap.segmentList[nextIndex];
                _currentJurney = 0;
            }

            await Task.Delay(100);
        }
            */
    }


}