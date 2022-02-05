using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ContentAwareResize
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public class ContentAwareResize
    {
        public struct coord
        {
            public int row;
            public int column;
        }
        //========================================================================================================
        //Your Code is Here:
        //===================
        /// <summary>
        /// Develop an efficient algorithm to get the minimum vertical seam to be removed
        /// </summary>
        /// <param name="energyMatrix">2D matrix filled with the calculated energy for each pixel in the image</param>
        /// <param name="Width">Image's width</param>
        /// <param name="Height">Image's height</param>
        /// <returns>BY REFERENCE: The min total value (energy) of the selected seam in "minSeamValue" & List of points of the selected min vertical seam in seamPathCoord</returns>
        public void CalculateSeamsCost(int[,] energyMatrix, int Width, int Height, ref int minSeamValue, ref List<coord> seamPathCoord)
        {
            // throw new NotImplementedException();
            int rows = Height;
            int colms = Width;
            int[,] myArray = new int[rows,colms];
            // memomization
            for(int j=0; j<colms; j++)
            {
                myArray[0, j] = energyMatrix[0, j]; //copy first row to myArray
            }
            for (int i = 1; i < rows; i++)
            {
                for (int j = 0; j < colms; j++)
                {
                    if (j == 0) //first cloumn
                    {
                        myArray[i,j] = energyMatrix[i,j] + Math.Min(myArray[i - 1, j], myArray[i - 1, j + 1]);
                    }
                    else if (j == colms - 1) //last column
                    {
                        myArray[i,j] = energyMatrix[i,j] + Math.Min(myArray[i - 1, j - 1], myArray[i - 1 ,j]);
                    }
                    else
                    {
                        int m1 = Math.Min(myArray[i - 1, j - 1], myArray[i - 1, j]);
                        myArray[i,j] = energyMatrix[i,j] + Math.Min(m1 , myArray[i - 1,j + 1]);
                    }
                }
            }
            int min = myArray[rows - 1, 0];  //initially min is in ((last row, first index))
            int index = 0; // first index

            for (int j = 1; j < colms; j++) // loop on last row
            {
                if (myArray[rows - 1, j] < min)
                {
                    min = myArray[rows - 1 ,j];
                    index = j;
                }
            }
            minSeamValue = min;

            //trace back
            coord R = new coord();
            List<coord> PathCoord = new List<coord>();
            R.row = rows - 1;
            R.column = index;
            PathCoord.Add(R); // down to up
            for (int i = rows - 1; i > 0; i--)
            {
                if (index == 0)
                {
                    int m1 = myArray[i - 1 , index];
                    int m2 = myArray[i - 1 , index + 1];
                    int mini = Math.Min(m1, m2);
                    if (mini == m2)
                    {
                        index = 1; // index =1 and go to next row
                    }
                }
                else if (index == colms - 1)
                {
                    int m1 = myArray[i - 1 , index];
                    int m2 = myArray[i - 1 , index - 1];
                    int mini = Math.Min(m1, m2);
                    if (mini == m2)
                    {
                        index = colms - 2; // then go to next row
                    }
                }
                else
                {
                    int m1 = myArray[i - 1 , index];
                    int m2 = myArray[i - 1 , index - 1];
                    int m3 = myArray[i - 1 , index + 1];
                    if (m2 < m1 && m2 < m3)
                    {
                        index = index - 1;
                    }
                    else if (m3 < m1 && m3 < m2)
                    {
                        index = index + 1;
                    }
                }
                R.row = i - 1;
                R.column = index;
                PathCoord.Add(R);
            }
            seamPathCoord = PathCoord;
            
        }

        // *****************************************
        // DON'T CHANGE CLASS OR FUNCTION NAME
        // YOU CAN ADD FUNCTIONS IF YOU NEED TO 
        // *****************************************
        #region DON'TCHANGETHISCODE
        public MyColor[,] _imageMatrix;
        public int[,] _energyMatrix;
        public int[,] _verIndexMap;
        public ContentAwareResize(string ImagePath)
        {
            _imageMatrix = ImageOperations.OpenImage(ImagePath);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);
            int _height = _energyMatrix.GetLength(0);
            int _width = _energyMatrix.GetLength(1);
        }
        public void CalculateVerIndexMap(int NumberOfSeams, ref int minSeamValueFinal, ref List<coord> seamPathCoord)
        {
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);

            int minSeamValue = -1;
            _verIndexMap = new int[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    _verIndexMap[i, j] = int.MaxValue;

            bool[] RemovedSeams = new bool[Width]; 
            for (int j = 0; j < Width; j++)
                RemovedSeams[j] = false;

            for (int s = 1; s <= NumberOfSeams; s++)
            {
                CalculateSeamsCost(_energyMatrix, Width, Height, ref minSeamValue, ref seamPathCoord);
                minSeamValueFinal = minSeamValue;

                //Search for Min Seam # s
                int Min = minSeamValue;

                //Mark all pixels of the current min Seam in the VerIndexMap
                if (seamPathCoord.Count != Height)
                    throw new Exception("You selected WRONG SEAM");
                for (int i = Height - 1; i >= 0; i--)
                {
                    if (_verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] != int.MaxValue)
                    {
                        string msg = "overalpped seams between seam # " + s + " and seam # " + _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column];
                        throw new Exception(msg);
                    }
                    _verIndexMap[seamPathCoord[i].row, seamPathCoord[i].column] = s;
                    //remove this seam from energy matrix by setting it to max value
                    _energyMatrix[seamPathCoord[i].row, seamPathCoord[i].column] = 100000;
                }

                //re-calculate Seams Cost in the next iteration again
            }
        }
        public void RemoveColumns(int NumberOfCols)
        {  
            int Width = _imageMatrix.GetLength(1);
            int Height = _imageMatrix.GetLength(0);
            _energyMatrix = ImageOperations.CalculateEnergy(_imageMatrix);

            int minSeamValue = 0;
            List<coord> seamPathCoord = null;
            //CalculateSeamsCost(_energyMatrix,Width,Height,ref minSeamValue, ref seamPathCoord);
            CalculateVerIndexMap(NumberOfCols,ref minSeamValue,ref seamPathCoord);
                
            MyColor[,] OldImage = _imageMatrix;
            _imageMatrix = new MyColor[Height, Width - NumberOfCols];
            for (int i = 0; i < Height; i++)
            {
                int cnt = 0;
                for (int j = 0; j < Width; j++)
                {
                    if (_verIndexMap[i,j] == int.MaxValue)
                        _imageMatrix[i, cnt++] = OldImage[i, j];
                }
            }
            
        }
        #endregion
    }
}
