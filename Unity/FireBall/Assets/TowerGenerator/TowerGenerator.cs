using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    [RequireComponent(typeof(GeneratorProcessor))]
    public class TowerGenerator : MonoBehaviour
    {
        public Prototype InitialNodePrototype;
        public bool DoGenerateOnStart;

        public Blueprint Blueprint { get; private set; }
        public GeneratorPointer Pointers { get; private set; }
        public SegmentConstructor Constructor { get; private set; }
        public SegmentArchitect Architect { get; private set; }
        public GeneratorProcessor Processor;
        // visualizer


        void Start()
        {

            if (DoGenerateOnStart)
                Generate();
        }

        public void Generate()
        {
            var proto = InstantiatePrototypes(InitialNodePrototype.gameObject );
            Blueprint = new Blueprint();
            Pointers = new GeneratorPointer( Blueprint );
            Constructor = new SegmentConstructor(Blueprint);
            Architect = new SegmentArchitect(Constructor);
            proto.GetComponent<GeneratorProcessor>().Generate(this, null);
        }

        private GameObject InstantiatePrototypes(GameObject prototypePrefab)
        {                                                                                                   
            var root = new GameObject("Prototype");
            root.transform.SetParent(transform);
            var proto = Instantiate(prototypePrefab, root.transform);
            proto.name = prototypePrefab.name;
            return proto;
        }



        //public virtual TopGenStep EstablishTower()
        //{
        //    SegmentBuilder segmentBuilder = new segmen(this, _rnd.ValueInt());

        //    segmentBuilder.Project(
        //        (TreeNode<SegmentBuilder.MemorySegment>)null,
        //        Range.One,
        //        Vector3.up,
        //        Vector3.zero,
        //        Config.GetPlacementConfig(TopologyType.ChunkIsland),
        //        null,
        //        null
        //    );

        //    Assert.IsTrue(segmentBuilder.GetProjectVariantsNumber() == 1);
        //    segmentBuilder.ApplyProject(0);

        //    var segment = segmentBuilder.Build().First();
        //    Assert.IsNotNull(segment);
        //    return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        //}
    }
}
