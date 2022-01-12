using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PAALab3
{
    [Serializable]
    public class SFNode
    {
        public char Value { get; set; }

        [NonSerialized]
        private BitArray _code;

        public BitArray Code { get => _code; set => _code = value; }

        public int Frequency { get; set; }
        public SFNode LeftChild { get; set; }
        public SFNode RightChild { get; set; }

        public SFNode() { }
        public SFNode(char val, int frequency)
        {
            Value = val;
            Frequency = frequency;
        }
    }

    public class ShannonFano
    {
        public Dictionary<char, int> HashTable = new Dictionary<char, int>();
        public string Input { get; set; }
        public Dictionary<char, BitArray> Dictionary = new Dictionary<char, BitArray>();
        List<SFNode> list = new List<SFNode>();
        public SFNode Root { get; set; }

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
            foreach (KeyValuePair<char, int> pair in HashTable)
            {
                SFNode node = new SFNode(pair.Key, pair.Value);
                list.Add(node);
            }
            list.OrderByDescending(x => x.Frequency);

            Root = GenerateCode(0, list.Count - 1);
        }
        public string Encode(string text)
        {
            string ret = "";
            foreach (char c in text)
            {
                ret += c + " ";
                ret += Huffman.PrintCode(Dictionary[c]) + " ";
            }
            return ret;

        }

        public SFNode GenerateCode(int start, int end)
        {
            if (start == end)
            {
                SFNode node = new SFNode(list[start].Value, list[start].Frequency);
                node.Code = list[start].Code;
                node.LeftChild = null;
                node.RightChild = null;
                Dictionary.Add(node.Value, node.Code);
                return node;
            }
            else if (start > end)
            {
                return null;
            }
            int startSum = 0, endSum = 0;
            int i = start, j = end;
            while (i <= j)
            {
                if (startSum <= endSum)
                {
                    startSum += list[i].Frequency;
                    if (list[i].Code != null)
                        list[i].Code = BitArrayPrepend(list[i].Code, false);
                    else
                    {
                        list[i].Code = new BitArray(1);
                        list[i].Code[0] = false;
                    }
                    i++;
                }
                else
                {
                    endSum += list[j].Frequency;

                    if (list[j].Code != null)
                        list[j].Code = BitArrayPrepend(list[j].Code, true);
                    else
                    {
                        list[j].Code = new BitArray(1);
                        list[j].Code[0] = true;
                    }
                    j--;
                }
            }
            SFNode left = GenerateCode(start, i - 1);
            SFNode right = GenerateCode(j + 1, end);
            SFNode retNode = new SFNode('*', left.Frequency + right.Frequency);
            retNode.LeftChild = left;
            retNode.RightChild = right;
            return retNode;

        }

        public void SaveCompression(string path)
        {


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
                    reference = Dictionary[c];//bitovi su zapisani unazad
                    refCounter = reference.Count - 1;// 0;
                    while (refCounter >= 0)
                    {
                        output[opCounter] = reference[refCounter];
                        opCounter++;
                        if (opCounter >= 8)
                        {
                            output.CopyTo(write, 0);
                            bw.Write(write);
                            opCounter = 0;
                        }
                        refCounter--;
                    }
                }
                if (opCounter > 0)
                {
                    output.CopyTo(write, 0);
                    bw.Write(write);
                }
            }
        }

        public string LoadCompressed(string path)
        {
            Dictionary<byte[], char> decoder;
            string result = "";
            using (System.IO.FileStream binaryFile = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                decoder = (Dictionary<byte[], char>)binaryFormater.Deserialize(binaryFile);
                System.IO.BinaryReader br = new System.IO.BinaryReader(binaryFile);

                while (br.BaseStream.Position != br.BaseStream.Length)
                {
                    byte[] bytes = new byte[1];
                    bytes[0] = br.ReadByte();
                    while (!decoder.ContainsKey(bytes))
                    {
                        byte[] bArr = new byte[bytes.Length + 1];
                        bytes.CopyTo(bArr, 0);
                        bArr[bArr.Length - 1] = br.ReadByte();
                        bytes = bArr;
                    }
                    result += decoder[bytes];
                }
                return result;
            }
        }
        public void LoadCompressed2(string path, string path2)
        {
            
            using (FileStream binaryFile = new FileStream(path, FileMode.Open))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormater =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Root = (SFNode)binaryFormater.Deserialize(binaryFile);

                BinaryReader br = new BinaryReader(binaryFile);
                byte[] read = new byte[1];
                SFNode ptr = Root;
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
                            if (ptr.LeftChild != null && ptr.RightChild != null)
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

        public void PrintDictionary()//kodovi su invertovani ali se to sredjuje u output fajlovima
        {
            foreach (KeyValuePair<char, BitArray> pair in Dictionary)
            {
                Console.WriteLine(pair.Key + " " + PrintCode(pair.Value));
            }
        }

        public static string PrintCode(BitArray arr)
        {//Stampanje kodova krece od nazad zbog naopakog redosleda upisivanja cifara u kod
            string ret = "";
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (arr[i] == true)
                    ret += "1";
                else
                    ret += "0";
            }

            return ret;
        }

        public static BitArray BitArrayPrepend(BitArray array, bool val)
        {
            BitArray retArr = new BitArray(array.Length + 1);
            retArr[0] = val;
            for (int i = 0; i < array.Count; i++)
                retArr[i + 1] = array[i];
            return retArr;
        }

        public static void CompressTheFile(string path)
        {

        }

        public static void DecompressTheFile(string path)
        {

        }

    }

  
}
