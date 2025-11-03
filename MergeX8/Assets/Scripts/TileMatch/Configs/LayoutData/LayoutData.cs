using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LayoutData
{
    public class Neighbors
    {
        public int id;
        public int neighborType;
    }
    
    public class LayerBlock
    {
        public int id;
        public Vector3 position = new Vector3();
        public int blockType;
        public int blockId;
        public string blockParam;
        public List<int> children = new List<int>();
        public List<int> parent = new List<int>();
        public List<Neighbors> neighbors = new List<Neighbors>();
        public int index_X;
        public int index_Y;
        
        public LayerBlock()
        {
            blockType = 0;
            blockId = 0;
        }

        public LayerBlock(LayerBlock block)
        {
            id = block.id;
            position = new Vector3(block.position.x, block.position.y, block.position.z);
            blockType = block.blockType;
            blockId= block.blockId;
            blockParam = block.blockParam;
            if(block.children != null)
                children.AddRange(block.children);
            
            if(block.parent != null)
                parent.AddRange(block.parent);
            
            if(block.neighbors != null)
                neighbors.AddRange(block.neighbors);
            index_X = block.index_X;
            index_Y = block.index_Y;
        }
    }

    public class LayoutGroup
    {
        public List<Layout> layout = new List<Layout>();
        
        public int gridRow;
        public int gridCol;

        public Dictionary<string, string> config = new Dictionary<string, string>();
    }
    
    public class Layout
    {
        public int id;
        public List<Layer> layers = new List<Layer>();
        public Vector3 offset = new Vector3();
        public bool isStackLayout = false;
        public bool isMaskLayout = false;
        public string layerParam;
    }

    public class Layer
    {
        public int id = -1;
        public List<LayerBlock> layerBlocks = new List<LayerBlock>();
    }
}

