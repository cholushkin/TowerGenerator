using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace TowerGenerator
{
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
                public string CurrentMeta;
                public int GroupsNumber;
                public Vector3[] Sizes;
                public string[] Tags;
            }

            public MetaInfos MetaInfosState;
            public MetaControlPanel MetaControlPanelState;
            public EntityControlPanel EntityControlPanelState;
            public bool ShowGUI;
        }

        public GUIState State;

        public int[] DelayButtonValues;
        public EntityPlace EntPlace;

        static readonly Vector2 ButtonSize = new Vector2(100, 100);
        static readonly Vector2 ButtonWideSize = new Vector2(230, 100);
        static readonly Vector2 LabelSize = new Vector2(200, 50);
        static readonly Vector2 BigDataLabelSize = new Vector2(300, 300);
        static readonly Vector2 Offset = new Vector2(32, 32);


        void Awake()
        {
            GetData();
            StartCoroutine(ProcessShowing());
        }

        private IEnumerator ProcessShowing()
        {
            NextEntity();
            yield return new WaitForSeconds(DelayButtonValues[_pointer]);
        }

        private void NextEntity()
        {
            //MetaProvider.Instance.Metas
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
                GUI.Box(new Rect(Screen.width - BigDataLabelSize.x - Offset.x, Offset.y, BigDataLabelSize.x, BigDataLabelSize.y),
                    $"{JsonUtility.ToJson(State.EntityControlPanelState, true)}");
            }

            // ShowGUI
            if (GUI.Button(new Rect(
                Screen.width - ButtonSize.x - Offset.x,
                Screen.height - ButtonSize.y - Offset.y,
                ButtonSize.x, ButtonSize.y), $"[DevGUI]:\n{State.ShowGUI}"))
                OnPressShowGUI();
            //GUI.Label(new Rect(0, 0, 200, 50), $"BB size {_currentBoundsSize.ToString()}");
            //GUI.Label(new Rect(0, 50, 200, 50), $"Groups {_amountOfGroups.x} of {_amountOfGroups.y}");
            //GUI.Label(new Rect(0, 100, 200, 50), $"Seed {_seed}");
        }


        private void GetData()
        {
            State.MetaInfosState.MetasNumber = MetaProvider.Instance.Metas.Length;
            State.MetaInfosState.ChunkRoofPeakNumber = MetaProvider.Instance.Metas.Count(
                x => x is MetaChunk chunk && chunk.ChunkType == Entity.EntityType.ChunkRoofPeak);
            State.MetaInfosState.ChunkRoofPeakNumberPercent =
                (int)((float)State.MetaInfosState.ChunkRoofPeakNumber / State.MetaInfosState.MetasNumber * 100f);
            // todo: all params
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

