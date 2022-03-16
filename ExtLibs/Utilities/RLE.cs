using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public class RLE
    {
        public static void test()
        {

            var data = File.ReadAllBytes(@"C:\Users\michael\Downloads\2020-05-30 11-31-08.bin");
            RLE.RunLengthEncode(data);

        }

        public static byte[] RunLengthEncode(byte[] s)
        {
            try
            {
                List<byte> srle = new List<byte>();
                int ccnt = 1; //char counter
                for (int i = 0; i < s.Length-1; i++ )
                {
                    if (s[i] != s[i + 1]) //..a break in character repetition or the end of the string
                    {
                        if (s[i] == 0)
                            srle.AddRange(new byte[] {(byte) ccnt, s[i]});
                        else
                            srle.AddRange(new byte[] {s[i]});
                        ccnt = 1; //reset char repetition counter
                    }
                    else {
                        if (ccnt == 250)
                        {
                            srle.AddRange(new byte[] {(byte)ccnt, s[i]});
                            ccnt = 1;
                        }
                        ccnt++;
                    }

                }

                return srle.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in RLE:" + e.Message);
                return null;
            }
        }

    }

    public class HuffMan
    {

        public class Node
        {
            public byte? Symbol { get; set; } = null;
            public int Frequency { get; set; }
            public Node Right { get; set; }
            public Node Left { get; set; }

            // Find the encoded code for a symbol from the current node
            public List<bool> Traverse(byte symbol, List<bool> data)
            {
                // Leaf
                if (Right == null && Left == null)
                {
                    if (symbol.Equals(this.Symbol))
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    List<bool> left = null;
                    List<bool> right = null;

                    if (Left != null)
                    {
                        List<bool> leftPathData = new List<bool>();
                        leftPathData.AddRange(data);
                        leftPathData.Add(false);

                        left = Left.Traverse(symbol, leftPathData);
                    }

                    if (Right != null)
                    {
                        List<bool> rightPathData = new List<bool>();
                        rightPathData.AddRange(data);
                        rightPathData.Add(true);
                        right = Right.Traverse(symbol, rightPathData);
                    }

                    if (left != null)
                    {
                        return left;
                    }
                    else
                    {
                        return right;
                    }
                }
            }
        }

        public static void test()
        {
            
            var data = File.ReadAllBytes(@"C:\Users\michael\Downloads\2020-05-30 11-31-08.bin");

            var huff= new HuffMan.HuffmanTree();
            Enumerable.Range(0, 257).Select(a => huff.SymbolFrequencies[(byte) a] = 0).ToArray();
            huff.BuildHuffmanTree(data);

            var tree = huff.Root.ToJSON();
            File.WriteAllText("huff.json", tree);

            var hufftree = File.ReadAllText("huff.json");
            huff.Root = JsonConvert.DeserializeObject<HuffMan.Node>(hufftree);

            var output = huff.Encode(data);

            var size = output.Length / 8;

            foreach (var item in Enumerable.Range(0, 256))
            {
                var ans = huff.Encode(new byte[] {(byte) item});
                Console.Write("{0} {0:X} = ", item);
                foreach (bool bit in ans)
                {
                    Console.Write(bit ? "1" : "0");
                }
                Console.WriteLine("");
            }

        

            var decode = huff.Decode(output);
        }

        public class HuffmanTree
        {
            private List<Node> nodes = new List<Node>();
            public Node Root { get; set; }
            public Dictionary<byte, int> SymbolFrequencies = new Dictionary<byte, int>();

            public void BuildHuffmanTree(ReadOnlySpan<byte> source)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    if (!SymbolFrequencies.ContainsKey(source[i]))
                    {
                        SymbolFrequencies.Add(source[i], 0);
                    }

                    SymbolFrequencies[source[i]]++;
                }

                // Step# 1: Create list of nodes with symbol and frequencies
                foreach (KeyValuePair<byte, int> symbol in SymbolFrequencies)
                {
                    nodes.Add(new Node() {Symbol = symbol.Key, Frequency = symbol.Value});
                }

                // Generate root nodes for the lowest frequencies and add it to the end of ordered nodes till only 1 node is left as main root of the complete huffman tree
                while (nodes.Count >= 2)
                {
                    // Step# 2: Sort the list of nodes based on its frequencies in ascending order
                    List<Node> orderedNodes = nodes.OrderBy(node => node.Frequency).ToList<Node>();

                    if (orderedNodes.Count >= 2)
                    {
                        // Take first two items
                        List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                        // Create a parent node by combining the frequencies
                        Node parent = new Node()
                        {
                            Symbol = null,
                            Frequency = taken[0].Frequency+taken[1].Frequency,
                            Left = taken[0],
                            Right = taken[1]
                        };
                        //Remove left and right nodes and add their parent to the end of nodes list
                        nodes.Remove(taken[0]);
                        nodes.Remove(taken[1]);
                        nodes.Add(parent);
                    }

                    this.Root = nodes.FirstOrDefault();
                }
            }

            public BitArray Encode(ReadOnlySpan<byte> source)
            {
                List<bool> encodedSource = new List<bool>();

                for (int i = 0; i < source.Length; i++)
                {
                    List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());

                    encodedSource.AddRange(encodedSymbol);

                    // Print the bit value for each symbol
                    //Console.Write("Symbol: " + source[i] + " Encoded: ");
                    //foreach (bool bit in new BitArray(encodedSymbol.ToArray()))
                    {
                        //Console.Write(bit);
                    }

                    //Console.WriteLine();
                }

                BitArray bits = new BitArray(encodedSource.ToArray());
                return bits;
            }

            public byte[] Decode(BitArray bits)
            {
                // Start from root of the huffman tree
                Node current = this.Root;
                List<byte> decoded = new List<byte>();

                foreach (bool bit in bits)
                {
                    if (bit) // If true then move right
                    {
                        if (current.Right != null)
                        {
                            current = current.Right;
                        }
                    }
                    else
                    {
                        // If false then move left
                        if (current.Left != null)
                        {
                            current = current.Left;
                        }
                    }

                    // Every leaf node is a symbol so once you reach there then add it to decoded and then reset the current to the root of huffman tree
                    if (IsLeaf(current))
                    {
                        decoded.Add(current.Symbol.Value);
                        current = this.Root;
                    }
                }

                return decoded.ToArray();
            }

            public bool IsLeaf(Node node)
            {
                return (node.Left == null && node.Right == null);
            }

        }
    }

    public class Delta
    {
        public static void delta_encode(ref byte[] buffer)
        {
            byte last = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                byte current = buffer[i];
                buffer[i] = (byte) (current - last);
                last = current;
            }
        }

        public static void delta_decode(ref byte[] buffer)
        {
            byte last = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                byte delta = buffer[i];
                buffer[i] = (byte) (delta + last);
                last = buffer[i];
            }
        }
    }
}
