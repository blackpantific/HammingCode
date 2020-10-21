using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HammingCode
{
    class Program
    {
        public static int N { get; set; } = 0;
        public static List<int> IntArrayString { get; set; } = new List<int>();
        public static int KNumber { get; set; }

        public static List<int> PositionOf2NumberInArray { get; set; } = new List<int>();
        public static Dictionary<int, List<int>> PairsOfCorrectSymbols { get; set; } = new Dictionary<int, List<int>>();
        public static List<List<int>> GeneratorMatrix { get; set; }
        public static List<List<int>> ParityCheckMatrix { get; set; }
        public static int[,] UsersCodeword { get; set; }
        public static int[,] ConvertedMatrix { get; set; }

        static void Main(string[] args)
        {
            Console.Write("Enter n >= 3: ");
            N = Convert.ToInt32(Console.ReadLine());

            KNumber = FindKValue(N);

            for (int i = 0; i < KNumber; i++)//finding position of 2^k elements
            {
                PositionOf2NumberInArray.Add(Convert.ToInt32(Math.Pow(2, i)));
                PairsOfCorrectSymbols.Add(PositionOf2NumberInArray[i], new List<int>());
            }

            bool flag;
            for (int i = 0; i < N; i++)//establishing compliance between numbers of user's word m and correct symbols
            {
                flag = true;
                for (int j = 0; j < PositionOf2NumberInArray.Count; j++)
                {
                    if (i + 1 == PositionOf2NumberInArray[j])
                    {
                        flag = false;
                    }
                }

                if(flag == false)
                {
                    continue;
                }
                else
                {
                    List<int> binaryIntString = BinaryStringToIntList(i+1);
                    for (int bi = 0; bi < binaryIntString.Count; bi++)
                    {
                        foreach (var keyValue in PairsOfCorrectSymbols.Keys)
                        {
                            if (binaryIntString[binaryIntString.Count - 1 - bi] * Math.Pow(2, bi) ==
                                keyValue)
                            {
                                List<int> valueListOfKey = PairsOfCorrectSymbols[keyValue];
                                valueListOfKey.Add(i+1);
                                break;
                            }
                        }
                    }

                }
            }
            Console.WriteLine("Generator matrix:");

            GeneratorMatrix = new List<List<int>>();
            ParityCheckMatrix = new List<List<int>>();

            for (int i = 0; i < N - KNumber; i++)
            {
                GeneratorMatrix.Add(new List<int>());
                ParityCheckMatrix.Add(new List<int>());
                for (int j = 0; j < N - KNumber; j++)
                {
                    if (i == j)
                    {
                        GeneratorMatrix[i].Add(1);
                    }
                    else
                    {
                        GeneratorMatrix[i].Add(0);
                    }
                }
            }

            bool flagForMatrix;
            int matrixI = 0;
            for (int i = 0; i < N; i++)
            {
                flagForMatrix = true;
                foreach (var item in PairsOfCorrectSymbols.Keys)
                {
                    if(i+1 == item)
                    {
                        flagForMatrix = false;
                    }
                }

                if (!flagForMatrix)
                {
                    continue;
                }
                else
                {
                    
                    foreach (var key in PairsOfCorrectSymbols.Keys)
                    {
                        List<int> values = PairsOfCorrectSymbols[key];
                        bool ifFound = false;
                        foreach (var item in values)
                        {
                            if(item == i + 1)
                            {
                                ifFound = true;
                                GeneratorMatrix[matrixI].Add(1);
                                ParityCheckMatrix[matrixI].Add(1);
                            }
                            
                        }
                        if (!ifFound)
                        {
                            GeneratorMatrix[matrixI].Add(0);
                            ParityCheckMatrix[matrixI].Add(0);
                        }

                    }
                    matrixI++;
                }
            }

            foreach (var row in GeneratorMatrix)
            {
                Console.WriteLine("\n");
                foreach (var rowItem in row)
                {
                    Console.Write($"{rowItem}, ");
                }
            }

            Console.WriteLine("\n\nParity-check matrix: ");

            for (int i = 0; i < ParityCheckMatrix[0].Count; i++)
            {
                ParityCheckMatrix.Add(new List<int>());
                for (int j = 0; j < ParityCheckMatrix[0].Count; j++)
                {
                    if(i == j)
                    {
                        ParityCheckMatrix[ParityCheckMatrix.Count - 1].Add(1);
                    }
                    else
                    {
                        ParityCheckMatrix[ParityCheckMatrix.Count - 1].Add(0);
                    }
                }
            }

            foreach (var row in ParityCheckMatrix)
            {
                Console.WriteLine("\n");
                foreach (var rowItem in row)
                {
                    Console.Write($"{rowItem}, ");
                }
            }

            Console.WriteLine("\n\nEnter codeword: ");

            var keywordString = Console.ReadLine();

            var resultOfMultiplication = MatrixMultiplication(StringConverterToIntArray(keywordString),
                ListConverterToIntArray(ParityCheckMatrix));
            int rows = resultOfMultiplication.GetUpperBound(0) + 1;
            int columns = resultOfMultiplication.Length / rows;

            for (int i = 0; i < rows; i++)
            {
                Console.WriteLine("\n");
                for (int j = 0; j < columns; j++)
                {
                    Console.Write($"{resultOfMultiplication[i, j]} \t");
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }

        public static int FindKValue(int n)
        {
            int k = 2;
            while(k!=0)
            {
                if(Math.Pow(2, k) >= k + (n-k) + 1)
                {
                    return k;
                }else
                {

                    k++;
                }

            }
            return 0;//?
        }

        public static string ToBinaryString(string text)
        {
            return string.Join("", Encoding.ASCII.GetBytes(text).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
        }

        public static List<int> BinaryStringToIntList(int positionInList)
        {
            List<int> listOfIntToReturn = new List<int>();
            var binaryArray = Convert.ToString(positionInList, 2);//ToBinaryString(Convert.ToString(positionInList));
            for (int i = 0; i < binaryArray.Length; i++)
            {
                listOfIntToReturn.Add(Convert.ToInt32(binaryArray[i].ToString()));
            }
            return listOfIntToReturn;
        }


        public static int[,] MatrixMultiplication(int[,] matrixA, int[,] matrixB)
        {
            if (matrixA.ColumnsCount() != matrixB.RowsCount())
            {
                throw new Exception("Умножение не возможно! Количество столбцов первой матрицы не равно количеству строк второй матрицы.");
            }

            var matrixC = new int[matrixA.RowsCount(), matrixB.ColumnsCount()];

            for (var i = 0; i < matrixA.RowsCount(); i++)
            {
                for (var j = 0; j < matrixB.ColumnsCount(); j++)
                {
                    matrixC[i, j] = 0;

                    for (var k = 0; k < matrixA.ColumnsCount(); k++)
                    {
                        matrixC[i, j] ^= matrixA[i, k] * matrixB[k, j];
                    }
                }
            }

            return matrixC;
        }

        public static int[,] StringConverterToIntArray(string str)
        {
            int[,] intArray = new int[1,str.Length];
            var tempStr = str.ToCharArray();

            for (int i = 0; i < str.Length; i++)
            {
                intArray[0, i] = Convert.ToInt32(tempStr[i].ToString());
            }

            return intArray;
        }

        public static int[,] ListConverterToIntArray(List<List<int>> list)
        {
            int[,] intArray = new int[list.Count, list[0].Count];

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[0].Count; j++)
                {
                    intArray[i, j] = list[i][j];
                }
            }
            return intArray;
        }


        //Console.Write("Enter string to convert: ");

        //UserString = Console.ReadLine();
        //var UserStringList = ToBinaryString(UserString);

        //for (int i = 0; i < UserStringList.Length; i++)
        //{
        //    ByteArrayInputString.Add(Convert.ToInt32(UserStringList[i].ToString()));
        //}





        //string someString = Encoding.ASCII.GetString(bytes);

    }
}
