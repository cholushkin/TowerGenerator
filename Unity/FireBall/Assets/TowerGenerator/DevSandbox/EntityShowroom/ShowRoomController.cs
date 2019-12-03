﻿using System;
using System.Collections;
using System.Linq;
using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    // todo: pause button (for save seeds)
    // todo: dump current entity to file
    public class ShowRoomController : MonoBehaviour
    {
        [Serializable]
        public class GUIState
        {
            [Serializable]
            public class MetaInfos
            {
                public int MetasNumber;

                public int ChunkRoofPeakNumber;
                public int ChunkRoofPeakNumberPercent;
                public int ChunkStdNumber;
                public float ChunkStdNumberPercent;
                public int ChunkIslandAndBasementNumber;
                public float ChunkIslandAndBasementNumberPercent;
                public int ChunkSideEarNumber;
                public float ChunkSideEarNumberPercent;
                public int ChunkBottomEarNumber;
                public float ChunkBottomEarNumberPercent;
                public int ChunkConnectorVerticalNumber;
                public float ChunkConnectorVerticalNumberPercent;
                public int ChunkConnectorHorizontal;
                public float ChunkConnectorHorizontalPercent;
            }

            [Serializable]
            public class MetaControlPanel
            {
                public bool ChunkRoofPeak;
                public bool ChunkStd;
                public bool ChunkIslandAndBasement;
                public bool ChunkSideEar;
                public bool ChunkBottomEar;
                public bool ChunkConnectorVertical;
                public bool ChunkConnectorHorizontal;
                public int Delay;
            }

            [Serializable]
            public class EntityControlPanel
            {
                public long CurrentSeed;
                //public int GroupsNumber;
                //public Vector3[] Sizes;
                //public string[] Tags;
            }

            public MetaInfos MetaInfosState;
            public MetaControlPanel MetaControlPanelState;
            public EntityControlPanel EntityControlPanelState;
            public string CurrentMeta = "asdasda";
            public bool IsPauseButtonPressed;
            public bool ShowGUI;
        }

        public GUIState State;

        public int[] DelayButtonValues;
        public EntityPlace EntPlace;
        private static RandomHelper _rnd = new RandomHelper(-1);

        static readonly Vector2 ButtonSize = new Vector2(100, 100);
        static readonly Vector2 ButtonWideSize = new Vector2(230, 100);
        static readonly Vector2 LabelSize = new Vector2(200, 50);
        static readonly Vector2 BigDataLabelSize = new Vector2(300, 300);
        static readonly Vector2 Offset = new Vector2(32, 32);


        void Awake()
        {
            UpdateData();
            StartCoroutine(ProcessShowing());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                OnPressPause();
        }

        private IEnumerator ProcessShowing()
        {
            while (true)
            {
                var nextMetaToShow = GetNextMeta();
                var seed = _rnd.GetCurrentSeed();
                Debug.Log($"Seed&Meta to show:{seed} {nextMetaToShow}");
                if (nextMetaToShow == null)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                UpdateDataForMeta(nextMetaToShow, seed);
                EntPlace.Place(nextMetaToShow, seed);
                yield return new WaitForSeconds(DelayButtonValues[_pointer]);
            }
        }
        
        private MetaBase GetNextMeta()
        {
            Entity.EntityType CookFlaggedValue(GUIState.MetaControlPanel stateMetaControlPanelState)
            {
                Entity.EntityType result = Entity.EntityType.Undefined;
                result |= stateMetaControlPanelState.ChunkRoofPeak ? Entity.EntityType.ChunkRoofPeak : 0;
                result |= stateMetaControlPanelState.ChunkStd ? Entity.EntityType.ChunkStd : 0;
                result |= stateMetaControlPanelState.ChunkIslandAndBasement ? Entity.EntityType.ChunkIslandAndBasement : 0;
                result |= stateMetaControlPanelState.ChunkSideEar ? Entity.EntityType.ChunkSideEar : 0;
                result |= stateMetaControlPanelState.ChunkBottomEar ? Entity.EntityType.ChunkBottomEar : 0;
                result |= stateMetaControlPanelState.ChunkConnectorVertical ? Entity.EntityType.ChunkConnectorVertical : 0;
                result |= stateMetaControlPanelState.ChunkConnectorHorizontal ? Entity.EntityType.ChunkConnectorHorizontal : 0;
                return result;
            }

            var filter = new MetaProvider.Filter(
                entFlags : CookFlaggedValue( State.MetaControlPanelState )
            );

            var metas = MetaProvider.Instance.GetMetas(filter);
            if (!metas.Any())
                return null;
            return _rnd.FromEnumerable(metas);
        }


        void OnGUI()
        {
            if (State.ShowGUI)
            {
                // MetaInfos
                GUI.Box(new Rect(Offset.x, Offset.y, BigDataLabelSize.x, BigDataLabelSize.y),
                    $"{JsonUtility.ToJson(State.MetaInfosState, true)}");

                // MetaControlPanel
                GUI.Label(new Rect(Offset.x, Offset.y + 350, LabelSize.x, LabelSize.y),
                    "Filters:");
                
                if (GUI.Button(new Rect(Offset.x, Offset.y + 400, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkRoofPeak: {State.MetaControlPanelState.ChunkRoofPeak}"))
                    State.MetaControlPanelState.ChunkRoofPeak = !State.MetaControlPanelState.ChunkRoofPeak;

                if (GUI.Button(new Rect(Offset.x, Offset.y + 500, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkStd: {State.MetaControlPanelState.ChunkStd}"))
                    State.MetaControlPanelState.ChunkStd = !State.MetaControlPanelState.ChunkStd;

                if (GUI.Button(new Rect(Offset.x, Offset.y + 600, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkIslandAndBasement: {State.MetaControlPanelState.ChunkIslandAndBasement}"))
                    State.MetaControlPanelState.ChunkIslandAndBasement =
                        !State.MetaControlPanelState.ChunkIslandAndBasement;

                if (GUI.Button(new Rect(Offset.x, Offset.y + 700, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkSideEar: {State.MetaControlPanelState.ChunkSideEar}"))
                    State.MetaControlPanelState.ChunkSideEar = !State.MetaControlPanelState.ChunkSideEar;

                if (GUI.Button(new Rect(Offset.x, Offset.y + 800, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkBottomEar: {State.MetaControlPanelState.ChunkBottomEar}"))
                    State.MetaControlPanelState.ChunkBottomEar = !State.MetaControlPanelState.ChunkBottomEar;

                if (GUI.Button(new Rect(Offset.x, Offset.y + 900, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkConnectorVertical: {State.MetaControlPanelState.ChunkConnectorVertical}"))
                    State.MetaControlPanelState.ChunkConnectorVertical =
                        !State.MetaControlPanelState.ChunkConnectorVertical;

                if (GUI.Button(new Rect(Offset.x, Offset.y + 1000, ButtonWideSize.x, ButtonWideSize.y),
                    $"ChunkConnectorHorizontal: {State.MetaControlPanelState.ChunkConnectorHorizontal}"))
                    State.MetaControlPanelState.ChunkConnectorHorizontal =
                        !State.MetaControlPanelState.ChunkConnectorHorizontal;

                if (GUI.Button(new Rect(Offset.x, Screen.height - ButtonWideSize.y - Offset.y, ButtonWideSize.x, ButtonWideSize.y),
                    $"Delay: {State.MetaControlPanelState.Delay}"))
                    OnPressDelay();

                // EntityControlPanel
                GUI.Box(new Rect(Screen.width - ButtonWideSize.x - Offset.x, Offset.y, ButtonWideSize.x, ButtonWideSize.y),
                    $"{JsonUtility.ToJson(State.EntityControlPanelState, true)}");

                GUI.Box(new Rect(
                        Screen.width - BigDataLabelSize.x - Offset.x, 
                        Offset.y + ButtonWideSize.y + Offset.y, 
                        BigDataLabelSize.x, BigDataLabelSize.y * 2f),
                    $"{State.CurrentMeta}");
            }

            // ShowGUI
            if (GUI.Button(new Rect(
                Screen.width - ButtonSize.x - Offset.x,
                Screen.height - ButtonSize.y - Offset.y,
                ButtonSize.x, ButtonSize.y), $"[DevGUI]:\n{State.ShowGUI}"))
                OnPressShowGUI();


            if (GUI.Button(new Rect(
                Screen.width - ButtonSize.x * 2 - Offset.x,
                Screen.height - ButtonSize.y - Offset.y,
                ButtonSize.x, ButtonSize.y), State.IsPauseButtonPressed ? "[ ▶ ]" : "[ ॥ ]"))
                OnPressPause();
        }

        private void OnPressPause()
        {
            State.IsPauseButtonPressed = !State.IsPauseButtonPressed;
            Time.timeScale = State.IsPauseButtonPressed ? 0f : 1f;
        }


        private void UpdateData()
        {
            // get meta number data
            {
                State.MetaInfosState.MetasNumber = MetaProvider.Instance.Metas.Length;

                // ChunkRoofPeakNumber
                State.MetaInfosState.ChunkRoofPeakNumber = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkRoofPeak);
                State.MetaInfosState.ChunkRoofPeakNumberPercent =
                    (int) ((float) State.MetaInfosState.ChunkRoofPeakNumber / State.MetaInfosState.MetasNumber * 100f);

                // ChunkStdNumber
                State.MetaInfosState.ChunkStdNumber = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkStd);
                State.MetaInfosState.ChunkStdNumberPercent =
                    (int)((float)State.MetaInfosState.ChunkStdNumber / State.MetaInfosState.MetasNumber * 100f);

                // ChunkIslandAndBasementNumber
                State.MetaInfosState.ChunkIslandAndBasementNumber = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkIslandAndBasement);
                State.MetaInfosState.ChunkIslandAndBasementNumberPercent =
                    (int)((float)State.MetaInfosState.ChunkIslandAndBasementNumber / State.MetaInfosState.MetasNumber * 100f);

                // ChunkSideEarNumber
                State.MetaInfosState.ChunkSideEarNumber = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkSideEar);
                State.MetaInfosState.ChunkSideEarNumberPercent =
                    (int)((float)State.MetaInfosState.ChunkSideEarNumber / State.MetaInfosState.MetasNumber * 100f);

                // ChunkBottomEarNumber
                State.MetaInfosState.ChunkBottomEarNumber = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkBottomEar);
                State.MetaInfosState.ChunkBottomEarNumberPercent =
                    (int)((float)State.MetaInfosState.ChunkBottomEarNumber / State.MetaInfosState.MetasNumber * 100f);

                // ChunkConnectorVerticalNumber
                State.MetaInfosState.ChunkConnectorVerticalNumber = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkConnectorVertical);
                State.MetaInfosState.ChunkConnectorVerticalNumberPercent =
                    (int)((float)State.MetaInfosState.ChunkConnectorVerticalNumber / State.MetaInfosState.MetasNumber * 100f);

                // ChunkConnectorHorizontal
                State.MetaInfosState.ChunkConnectorHorizontal = MetaProvider.Instance.Metas.Count(
                    x => x is MetaChunk chunk && chunk.EntityType == Entity.EntityType.ChunkConnectorHorizontal);
                State.MetaInfosState.ChunkConnectorHorizontalPercent =
                    (int)((float)State.MetaInfosState.ChunkConnectorHorizontal / State.MetaInfosState.MetasNumber * 100f);
            }
        }

        private void UpdateDataForMeta(MetaBase meta, long seed)
        {
            State.EntityControlPanelState.CurrentSeed = seed;
            State.CurrentMeta = meta.ToString();
        }

        void OnPressShowGUI()
        {
            State.ShowGUI = !State.ShowGUI;
        }
        
        private int _pointer;
        void OnPressDelay()
        {
            _pointer = (_pointer + 1) % DelayButtonValues.Length;
            State.MetaControlPanelState.Delay = DelayButtonValues[_pointer];
        }
    }
}

