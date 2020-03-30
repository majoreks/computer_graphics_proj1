using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cg1
{
    public class Quantizer
    {
        public Node root;
        public List<List<Node>> levels = new List<List<Node>>();
        private int MAX_DEPTH = 8;

        public Quantizer()
        {
            root = new Node(0, this);
            //levels.Add(new List<Node>());
        }

        public void AddLevelNode(int level, Node node)
        {
            if (levels.ElementAtOrDefault(level) == null)
            {
                levels.Add(new List<Node>());
            }
            levels[level].Add(node);
        }

        public void AddColor(MyColor color)
        {
            root.AddColor(color, 0, this);
        }

        public List<Node> LeafNodes()
        {
            return root.LeafNodes();
        }

        public List<MyColor> MakePalette(int colorCount)
        {
            var palette = new List<MyColor>();
            var paletteIndex = 0;
            var leafCount = LeafNodes().Count;
            for (var level = MAX_DEPTH - 1; level > -1; level -= 1)
            {
                if (levels.Count == 0)
                {

                }
                if (levels.ElementAtOrDefault(level) != null)
                {
                    foreach(var node in levels[level])
                    {
                        leafCount -= node.RemoveLeaves();
                        if (leafCount <= colorCount)
                        {
                            break;
                        }
                    }
                    if (leafCount <= colorCount)
                    {
                        break;
                    }
                    levels[level] = new List<Node>();
                }
            }
            foreach (var node in LeafNodes())
            {
                if (paletteIndex >= colorCount)
                {
                    break;
                }
                if (node.IsLeaf())
                {
                    palette.Add(node.color);
                }
                node.paletteIndex = paletteIndex;
                paletteIndex++;
            }
            return palette;
        }
    }
}
