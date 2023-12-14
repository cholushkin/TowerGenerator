#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
#endif
using System;
using System.Linq;
using GameLib.Alg;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

using Random = UnityEngine.Random;

namespace TowerGenerator
{
    public class ChunkControllerDebug : MonoBehaviour
    {
        public ChunkControllerBase ChunkController;
        public bool InitOnAwake;
        public bool IsDrawTree;
        [ResizableTextArea]
        public string TreeText;

        public bool IsDrawSuppressionLabels;
        public bool IsDrawInductionLabels;


        private ChunkControllerBase.DebugChunkControllerBase _dbgChunkController;
        private long _prevSeed = -1;

        void Awake()
        {
            if (InitOnAwake)
            {
                Init();
                SetRandomConfiguration();
            }
        }

        void Update()
        {
            if (IsDrawTree)
            {
                TreeText = "";
                var index = 0;
                foreach (var treeNode in ChunkController.GetImpactTree().TraverseDepthFirstPreOrder())
                {
                    var group = treeNode.Data.GetComponent<Group>();
                    Assert.IsNotNull(group);
                    var path = $"{treeNode.Data.transform.GetDebugName()}:g[{group.GetItemsCount()}]";
                    TreeText += (index == 0 ? "" : "\n") + path;
                    ++index;
                }
            }
        }

        public bool IsInitialized()
        {
            if (ChunkController == null)
                return false;
            if (ChunkController.GetImpactTree() == null)
                return false;
            return true;
        }

        //private string PrintGroupState(Transform groupTransform)
        //{
        //    var group = groupTransform.GetComponent<Group>();
        //    Assert.IsNotNull(group);

        //    var strOutcome = $"{group.GetType().Name}:{groupTransform.GetDebugName(false)}:";
        //    for (int i = 0; i < groupTransform.childCount; ++i)
        //    {
        //        strOutcome += groupTransform.GetChild(i).gameObject.activeSelf ? "V" : "X";
        //    }
        //    Debug.Log(strOutcome);
        //}

        [HideIf("IsInitialized")]
        [Button()]
        public void Init()
        {
            if (ChunkController == null)
            {
                ChunkController = GetComponent<ChunkControllerBase>();
            }
            _dbgChunkController = new ChunkControllerBase.DebugChunkControllerBase(ChunkController);
            ChunkController.Init();
        }

        [ShowIf("IsInitialized")]
        [Button()]
        public void SetRandomConfiguration()
        {
            if (ChunkController.Seed == _prevSeed)
            {
                ChunkController.Seed = Random.Range(0, Int32.MaxValue);
                _prevSeed = ChunkController.Seed;
            }
            ChunkController.SetConfiguration();
        }

        public void DbgPrintImpactTree()
        {
            Debug.Log(">>>>> Impact tree");
            var tree = ChunkController.GetImpactTree();
            foreach (var treeNode in tree.TraverseDepthFirstPreOrder())
                Debug.Log($"{treeNode.Data.transform.GetDebugName()}: level:{treeNode.Level} branch level:{treeNode.BranchLevel} ");
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!IsInitialized())
                return;

            // ----- Draw caption
            int lineIndex = 0;
            DrawTextLine(ref lineIndex, "Chunk: " + _dbgChunkController.GetChunkControllerName());

            // ----- Draw suppression labels
            if (IsDrawSuppressionLabels)
            {
                var keys = _dbgChunkController.GetSuppressionDictionary().Select(x => $"{x.Key}");
                var suppressionLabels = "Suppression: " + string.Join(" ", keys);
                DrawTextLine(ref lineIndex, suppressionLabels);
            }

            // ----- Draw induction labels
            if (IsDrawInductionLabels)
            {
                var keys = _dbgChunkController.GetInductionDictionary().Select(x => $"{x.Key}");
                var inductionLabels = "Induction: " + string.Join(" ", keys);
                DrawTextLine(ref lineIndex, inductionLabels);
            }


        }

        void DrawTextLine(ref int line, string text, Color color = default)
        {
            Handles.color = color;
            Handles.Label(transform.position + Vector3.down * line++, text);
        }
#endif
    }
}