#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using Assets.Plugins.Alg;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using Handles = UnityEditor.Handles;

namespace TowerGenerator
{
    public class ChunkControllerDebug : MonoBehaviour
    {
        public ChunkControllerBase ChunkController;
        public bool InitOnAwake;
        public bool IsDrawTree;
        public bool IsDrawSuppressionLabels;
        public bool IsDrawInductionLabels;
        

        private ChunkControllerBase.DebugChunkControllerBase _dbgChunkController;

        void Awake()
        {
            if(InitOnAwake)
                Init();
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

            // ----- Draw tree
            if (IsDrawTree)
            {
                foreach (var treeNode in ChunkController.GetImpactTree().TraverseDepthFirstPreOrder())
                {
                    var group = treeNode.Data.GetComponent<Group>();
                    var path = $"{treeNode.Data.transform.GetDebugName()}:g[{group.GetItemsCount()}]";
                    Assert.IsNotNull(group);

                    DrawTextLine(ref lineIndex, path);
                }
            }

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