using System;
using System.Collections.Generic;
using System.Text;

namespace cg1
{
    public class Node
    {
        int MAX_DEPTH = 8;
        public MyColor color;
        public int pixelCount;
        public int paletteIndex;
        public List<Node> children;
        public bool empty = true;

        public Node(int level, Quantizer parent)
        {
            color = new MyColor(0, 0, 0);
            pixelCount = 0;
            paletteIndex = 0;
            children = new List<Node>();
            if (level < MAX_DEPTH -1)
            {
                parent.AddLevelNode(level, this);
            }
        }
        public bool IsLeaf()
        {
            return pixelCount > 0;
        }

        public List<Node> LeafNodes()
        {
            var leafNodes = new List<Node>();
            List<Node> tmp = new List<Node>();
            foreach (var node in children)
            {
                if (node == null) continue;
                if (node.IsLeaf())
                {
                    leafNodes.Add(node);
                }
                else
                {
                    tmp = node.LeafNodes();
                    leafNodes.AddRange(tmp);
                }
            }
            return leafNodes;
        }
        public void AddColor(MyColor color, int level, Quantizer parent)
        {
            if (level >= MAX_DEPTH)
            {
                this.color.Add(color);
                pixelCount++;
                return;
            }
            var index = GetColorIndex(color, level);
            if(children.Count == 0)
            {
                children.Add(new Node(level, parent));
                return;
            }
            if (children[index].empty)
            {
                children[index].AddColor(color, level + 1, parent);
                children[index].empty = !(children[index].empty);
            }
        }

        public int GetPaletteIndex(MyColor color, int level)
        {
            if (IsLeaf())
            {
                return paletteIndex;
            }
            var index = GetColorIndex(color, level);
            if (children[index] != null)
            {
                return children[index].GetPaletteIndex(color, level + 1);
            }
            else
            {
                int xd = 0;
                foreach (var node in children)
                {
                    xd = node.GetPaletteIndex(color, level + 1);
                }
                return xd;
            }
        }

        public int RemoveLeaves()
        {
            var result = 0;
            foreach (var node in children)
            {
                if (node == null)
                {
                    continue;
                }
                color.Add(node.color);
                pixelCount += node.pixelCount;
                result++;
            }
            children = new List<Node>();
            return result - 1;
        }

        public int GetColorIndex(MyColor color, int level)  
        {
            var index = 0;
            var mask = 0b10000000 >> level;
            if ((color.red & mask)==1) index |= 0b100;
            if ((color.green & mask)== 1) index |= 0b010;
            if ((color.blue & mask)== 1) index |= 0b001;
            return index;
        }
    }
}
