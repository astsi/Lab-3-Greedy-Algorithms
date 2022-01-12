using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PAALab3
{
    [Serializable]
    public class HuffmanNode
    {
        public char Value { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode LeftChild { get; set; }
        public HuffmanNode RightChild { get; set; }

        public HuffmanNode(char c, int frequency = 0)
        {
            Value = c;
            Frequency = frequency;
            LeftChild = RightChild = null;
        }
    }

    public class Huffman
    {
        public HuffmanNode Root { get; set; }
        public string Input { get; set; }
        public Dictionary<char, int> HashTable = new Dictionary<char, int>();
        public Dictionary<char, BitArray> Dictionary = new Dictionary<char, BitArray>();


        public void Build(string text)
        {
            Input = text;
            for (int i = 0; i < text.Length; i++)
            {
                if (!HashTable.ContainsKey(text[i]))
                {
                    HashTable.Add(text[i], 1);
                }
                else
                    HashTable[text[i]]++;
            }

            Heap<HuffmanNode> priorityQueue = new Heap<HuffmanNode>(HashTable.Count);
            foreach (KeyValuePair<char, int> pair in HashTable)
            {
                HuffmanNode node = new HuffmanNode(pair.Key, pair.Value);
                priorityQueue.Insert(pair.Value, node);
            }

            while (priorityQueue.Count > 1)
            {
                HeapNode<HuffmanNode> node1 = priorityQueue.ExtractMinimum();
                HeapNode<HuffmanNode> node2 = priorityQueue.ExtractMinimum();
                HuffmanNode parent = new HuffmanNode('*', node1.Value.Frequency + node2.Value.Frequency);
                parent.LeftChild = node1.Value;
                parent.RightChild = node2.Value;
                priorityQueue.Insert(parent.Frequency, parent);
            }
            Root = priorityQueue.ExtractMinimum().Value;

            if (Root != null)
            {
                Inorder(Root.LeftChild, new BitArray(1, false));
                Inorder(Root.RightChild, new BitArray(1, true));
            }
        }

        public string Encode(string text)
        {
            string result = "";
            foreach (char c in text)
            {
                result += PrintCode(Dictionary[c]);
            }
            return result;
        }

        public void Inorder(HuffmanNode node, BitArray bitArray)
        {
            if (node == null)
                return;
            if (node.LeftChild == null && node.RightChild == null)
            {
                Dictionary.Add(node.Value, bitArray);
            }
            else
            {
                BitArray leftArray = new BitArray(bitArray.Length + 1);
                BitArray rightArray = new BitArray(bitArray.Length + 1);

                for (int i = 0; i < bitArray.Length; i++)
                {
                    leftArray[i] = rightArray[i] = bitArray[i];
                }
                leftArray[leftArray.Length - 1] = false;
                rightArray[rightArray.Length - 1] = true;
                Inorder(node.LeftChild, leftArray);
                Inorder(node.RightChild, rightArray);
            }
        }


        public void SaveCompression2(string path)
        {
            BitArray output = new BitArray(8);
            int opCounter = 0;
            byte[] write = new byte[1];
            using (FileStream binaryFile = new FileStream(path, FileMode.Create))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormater.Serialize(binaryFile, Root);
                BinaryWriter bw = new BinaryWriter(binaryFile);
                BitArray reference;
                int refCounter = 0;
                foreach (char c in Input)
                {
                    reference = Dictionary[c];
                    refCounter = 0;
                    while (refCounter < reference.Count)
                    {
                        output[opCounter] = reference[refCounter];
                        opCounter++;
                        if (opCounter >= 8)
                        {
                            output.CopyTo(write, 0);
                            bw.Write(write);
                            opCounter = 0;
                        }
                        refCounter++;
                    }
                }
                if (opCounter > 0)
                {
                    output.CopyTo(write, 0);
                    bw.Write(write);
                }
            }
        }


        public void LoadCompressed2(string path, string path2)
        {
            using (FileStream binaryFile = new FileStream(path, FileMode.Open))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Root = (HuffmanNode)binaryFormater.Deserialize(binaryFile);

                BinaryReader br = new BinaryReader(binaryFile);
                byte[] read = new byte[1];
                HuffmanNode ptr = Root;
                BitArray arr;
                int arrPtr = 0;
                using (StreamWriter sw = new StreamWriter(new FileStream(path2, FileMode.Create)))
                {
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        read[0] = br.ReadByte();
                        arr = new BitArray(read);
                        arrPtr = 0;
                       
                        while (arrPtr < arr.Length)
                        {

                            if (ptr.LeftChild != null && ptr.RightChild != null)//mozda moze i samo jedna provera
                            {
                                if (arr[arrPtr])
                                    ptr = ptr.RightChild;
                                else ptr = ptr.LeftChild;
                                arrPtr++;
                            }
                            else
                            {
                                sw.Write(ptr.Value);
                                ptr = Root;
                            }

                        }
                    }
                }
            }
        }

        public void PrintDictionary()
        {
            foreach (KeyValuePair<char, BitArray> pair in Dictionary)
            {
                Console.WriteLine(pair.Key + " " + PrintCode(pair.Value));
            }
        }
        public static string PrintCode(BitArray arr)
        {//Stampanje kodova krece od nazad zbog naopakog redosleda upisivanja cifara u kod
            string ret = "";
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == true)
                    ret += "1";
                else
                    ret += "0";
            }


            return ret;
        }
       /* public static string PrintCode(byte[] arr)
        {
            string ret = "";
            foreach (byte x in arr)
                ret += Convert.ToString(x, 2);
            return ret;
        } */
        public static BitArray Append(BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public string LoadCompressed(string path)
        {
            byte[] bytes;
            using (FileStream binaryFile = new FileStream(path, FileMode.Open))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Root = (HuffmanNode)binaryFormater.Deserialize(binaryFile);

                BinaryReader br = new BinaryReader(binaryFile);
                int count = br.ReadInt32();
                bytes = br.ReadBytes(count);
            }
            BitArray bitArray = new BitArray(bytes);
            int i = 0;
            HuffmanNode ptr = Root;
            string result = "";
            while (i < bitArray.Length)
            {

                if (ptr.LeftChild != null && ptr.RightChild != null)//mozda moze i samo jedna provera
                {
                    if (bitArray[i])
                        ptr = ptr.RightChild;
                    else ptr = ptr.LeftChild;
                    i++;
                }
                else
                {
                    result += ptr.Value;
                    ptr = Root;
                }

            }
            return result;

        }
        public void SaveCompression(string path)
        {
            BitArray output = new BitArray(Dictionary[Input[0]]);
            for (int i = 1; i < Input.Length; i++)
            {
                output = Append(output, Dictionary[Input[i]]);
            }
            using (FileStream binaryFile = new FileStream(path, FileMode.Create))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormater.Serialize(binaryFile, Root);
                BinaryWriter bw = new BinaryWriter(binaryFile);
                byte[] bytes = new byte[output.Count / 8 + (output.Count % 8 == 0 ? 0 : 1)];
                output.CopyTo(bytes, 0);
                bw.Write(bytes.Length);
                bw.Write(bytes);
            }


        }
    }
}
