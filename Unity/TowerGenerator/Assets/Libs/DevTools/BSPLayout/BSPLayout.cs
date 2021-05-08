using UnityEngine;
using UnityEngine.Assertions;

namespace GameGUI.Dev
{
    public class BSPLayout : MonoBehaviour
    {
        public enum BSPTreeLayout
        {
            V2_2,
            H2_2,
            V3_3,
            V3_9,
            V3_12,
            V3_a,
        }

        protected static class BspTreeLayoutFactory
        {
            public static BspTree.Leaf Create(BSPTreeLayout layoutName)
            {
                switch (layoutName)
                {
                    case BSPTreeLayout.V2_2:
                        return new BspTree.Leaf().SplitV(0.5f, out _, out _);
                    case BSPTreeLayout.H2_2:
                        return new BspTree.Leaf().SplitH(0.5f, out _, out _);

                    case BSPTreeLayout.V3_3:
                        {
                            BspTree.Leaf p1, p2, p3, p4;
                            var tree = new BspTree.Leaf();
                            tree.SplitV(0.25f, out p1, out p2);
                            p2.SplitV(2 / 3f, out p3, out p4);
                            return tree;
                        }

                    case BSPTreeLayout.V3_9:
                        {
                            BspTree.Leaf p1, p2, p3, p4;
                            var tree = new BspTree.Leaf();
                            tree.SplitV(0.25f, out p1, out p2);
                            p2.SplitV(2 / 3f, out p3, out p4);

                            // p1
                            {
                                BspTree.Leaf rest;
                                p1.SplitH(1f / 3f, out _, out rest);
                                rest.SplitH(0.5f, out _, out _);
                            }

                            // p3
                            {
                                BspTree.Leaf rest;
                                p3.SplitH(1f / 3f, out _, out rest);
                                rest.SplitH(0.5f, out _, out _);
                            }

                            // p4
                            {
                                BspTree.Leaf rest;
                                p4.SplitH(1f / 3f, out _, out rest);
                                rest.SplitH(0.5f, out _, out _);
                            }
                            return tree;
                        }
                    case BSPTreeLayout.V3_12:
                        {
                            BspTree.Leaf p1, p2, p3, p4;
                            var tree = new BspTree.Leaf();
                            tree.SplitV(0.25f, out p1, out p2);
                            p2.SplitV(2 / 3f, out p3, out p4);

                            // p1
                            {
                                BspTree.Leaf a, b, c, d;
                                p1.SplitH(0.5f, out a, out b);
                                a.SplitH(0.5f, out c, out d);
                                b.SplitH(0.5f, out c, out d);
                            }

                            // p3
                            {
                                BspTree.Leaf a, b, c, d;
                                p3.SplitH(0.5f, out a, out b);
                                a.SplitH(0.5f, out c, out d);
                                b.SplitH(0.5f, out c, out d);

                            }

                            // p4
                            {
                                BspTree.Leaf a, b, c, d;
                                p4.SplitH(0.5f, out a, out b);
                                a.SplitH(0.5f, out c, out d);
                                b.SplitH(0.5f, out c, out d);

                            }
                            return tree;
                        }
                    case BSPTreeLayout.V3_a:
                        {
                            BspTree.Leaf p1, p2, p3, p4;
                            var tree = new BspTree.Leaf();
                            tree.SplitV(0.25f, out p1, out p2);
                            p2.SplitV(2 / 3f, out p3, out p4);

                            // p1
                            {
                                BspTree.Leaf a, b, c, d;
                                p1.SplitH(0.5f, out a, out b);
                                a.SplitH(0.5f, out c, out d);
                                c.SplitH(0.5f, out _, out _);
                                d.SplitH(0.5f, out _, out _);

                                b.SplitH(0.5f, out c, out d);
                                c.SplitH(0.5f, out _, out _);
                                d.SplitH(0.5f, out _, out _);
                            }

                            // p3
                            {
                                BspTree.Leaf a, b, c, d;
                                p3.SplitH(0.5f, out a, out b);
                                a.SplitH(0.5f, out c, out d);
                                b.SplitH(0.5f, out c, out d);
                            }

                            // p4
                            {
                                BspTree.Leaf a, b, c, d;
                                p4.SplitH(0.5f, out a, out b);
                                a.SplitH(0.5f, out c, out d);
                                b.SplitH(0.5f, out c, out d);
                            }
                            return tree;
                        }



                    default:
                        return null;
                }
            }
        }

        public BSPTreeLayout BspTreeLayout;
        public GameObject PrefabNode;

        private BspTree.Leaf _BSPTree;

        void Awake()
        {
            _BSPTree = BspTreeLayoutFactory.Create(BspTreeLayout);
            ToGUI();
        }

        public void ToGUI()
        {
            var rootRectTransform = GetComponent<RectTransform>();

            var rectIndex = 0;
            foreach (var leaf in _BSPTree.GetTopLeafs())
            {
                var node = Instantiate(PrefabNode, rootRectTransform);
                node.name = $"Rect{rectIndex++}";
                var rectTransform = node.GetComponent<RectTransform>();
                Assert.IsNotNull(rectTransform);

                rectTransform.anchorMin = leaf.Min;
                rectTransform.anchorMax = leaf.Min + leaf.Size;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
    }
}