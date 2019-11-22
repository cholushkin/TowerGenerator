using System.Collections.Generic;
using System.Linq;
using GameLib.DataStructures;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class SegmentBuilder
    {
        public class StepResult
        {
            public int Index;
            public Bounds Bounds;

            public Vector3 Size => Bounds.size;
            public Vector3 Position => Bounds.center;
            public Vector3 BuildDirection;

            public TreeNode<Blueprint.Segment> Segment { get; internal set; }
            public StepResult Prev { get; internal set; }
            public StepResult Next { get; internal set; }

            public bool IsFirstOne { get; set; }

            public bool IsLastOne { get; set; }

            public bool IsDeadlock { get; set; }

            protected internal SegmentBuilder _builder;

            public void BuildSegment(Entity.EntityType segment = Entity.EntityType.ChunkStd,
                bool isOpened = false)
            {
                if (Segment != null)
                    return;
                if (_builder == null) // for proxy step result
                    return;
                Segment = _builder._generator.CreateSegment(
                    Prev.Segment,
                    BuildDirection,
                    Size,
                    (Index == 0) ? _builder._trunkOffset : Vector3.zero
                );
                Segment.Data.Topology.EntityType = segment;
                Segment.Data.Topology.IsOpenedForGenerator = isOpened;
                _builder.CreatedCount++;
            }
        }

        public Vector3 CurrentDirection { get; set; }
        private StepResult _steps;
        private int _segCount;
        protected TopologyGeneratorBase _generator;
        private StepResult _fromStep;
        private Vector3 _initialDirection;
        protected Vector3 _trunkOffset;
        private Range _segmentSize;
        public int CreatedCount { get; set; }

        // flying island support
        public SegmentBuilder(int segCount, TreeNode<Blueprint.Segment> fromSegment, Vector3 initialDirection, TopologyGeneratorBase generator, Vector3 offsetFromTrunk)
        {
            _segCount = segCount;
            _initialDirection = initialDirection;
            CurrentDirection = _initialDirection;
            _generator = generator;
            _segmentSize = generator.GetConfig().SegmentsSize;
            _trunkOffset = offsetFromTrunk;

            Assert.IsNotNull(fromSegment);

            // create proxy _fromStep
            _fromStep = new StepResult
            {
                Index = -1,
                Bounds = new Bounds(fromSegment.Data.Topology.Position, fromSegment.Data.Topology.AspectRatio),
                BuildDirection = fromSegment.Data.Topology.BuildDirection,
                Segment = fromSegment,
                Next = null,
                Prev = null,
                IsDeadlock = false,
                IsFirstOne = false,
                IsLastOne = false,
                _builder = null
            };
        }

        public SegmentBuilder(int segCount, StepResult fromStep, Vector3 initialDirection, TopologyGeneratorBase generator, Vector3 offsetFromTrunk)
        {
            _segCount = segCount;
            _fromStep = fromStep;
            _initialDirection = initialDirection;
            CurrentDirection = _initialDirection;
            _generator = generator;
            _segmentSize = generator.GetConfig().SegmentsSize;
            _trunkOffset = offsetFromTrunk;
        }

        public IEnumerable<StepResult> Step()
        {
            var prevStepResult = _fromStep;
            for (int i = 0; i < _segCount; ++i)
            {
                // new step
                var trunkOffset = i == 0 ? _trunkOffset : Vector3.zero;
                var parentBounds = prevStepResult.Bounds;

                // in addition to tree collision check we also need to check for self collision
                var prevBounds = Steps().Select(x => x.Bounds);
                var fitSize = _generator.GetNextSegmentRndFitSize(_segmentSize, parentBounds, CurrentDirection, trunkOffset, prevBounds);
                var bounds = _generator.CreateBoundsForChild(parentBounds, CurrentDirection, fitSize, trunkOffset);

                var currentStepResult = new StepResult
                {
                    Index = i,
                    Bounds = bounds,
                    BuildDirection = CurrentDirection,
                    Segment = null,
                    Next = null,
                    Prev = prevStepResult,
                    IsDeadlock = fitSize.x < _segmentSize.From,
                    IsFirstOne = i == 0,
                    IsLastOne = i == _segCount - 1,
                    _builder = this
                };

                if (i == 0)
                    _steps = currentStepResult;

                if (prevStepResult != _fromStep)
                    prevStepResult.Next = currentStepResult;

                yield return currentStepResult;
                if (currentStepResult.IsDeadlock)
                {
                    currentStepResult.Prev.IsLastOne = true;
                    yield break;
                }

                prevStepResult = currentStepResult;
            }
        }

        public IEnumerable<StepResult> Steps()
        {
            StepResult pointer = _steps;
            while (pointer != null)
            {
                yield return pointer;
                pointer = pointer.Next;
            }
        }

        public void SetSegmentSize(Range segmentSize) // by default takes it from cfg
        {
            _segmentSize = segmentSize;
        }
    }
}