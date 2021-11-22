using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Computer_Vision_Homework
{
    public partial class Form1 : Form
    {
        enum Filter
        {
            Mean,
            Median,
            Laplacian
        }

        enum Size
        {
            Three,
            Five,
            Seven
        }

        private Image _srcImage,_dstImage;
        private Bitmap _srcBitImage, _dstBitImage;

        private Filter _selectedFilter;
        private Size _selectedSize;
        private int[,] _convolutionMask;
        private int startX = 0, startY = 0, endX = 0, endY = 0;

        public Form1()
        {
            InitializeComponent();
            //this.exeBtn.Click += new System.EventHandler(this.button1_Click);
            //this.BrowseBtn.Click += new System.EventHandler(this.button2_Click);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this._selectedFilter = Filter.Mean;
            this._selectedSize = Size.Three;
            radioButton4.Checked = true;
            radioButton1.Checked = true;
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            _srcImage = null;
            _dstImage = null;
        }

        //실행 버튼
        private void exeBtn_Click(object sender, EventArgs e)
        {
            //Bitmap b = _srcImage as Bitmap;
            //MessageBox.Show($"{b.Width} {b.Height}");
            if (_srcImage == null)
            {
                MessageBox.Show("이미지를 등록해주세요.");
                return;
            }
            Show_Filter_Option();
            Execute_Filtering();
        }

        //찾아오기 버튼
        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            pictureBox5.Image = null;
            Browse_Picture();

            if(_srcImage != null)
                textBox2.Text = _srcImage.Width.ToString() + " X " + _srcImage.Height.ToString() + " px";
        }

        //저장하기 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            Save_Picture();
        }

        // Mean Filter 버튼
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this._selectedFilter = Filter.Mean;
            Size_Button_Visible(true);
        }

        // Median Filter 버튼
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this._selectedFilter = Filter.Median;
            Size_Button_Visible(true);
        }

        // Laplacian Filter 버튼
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this._selectedFilter = Filter.Laplacian;
            this._selectedSize = Size.Three;
            radioButton4.Checked = true;
            Size_Button_Visible(false);
        }

        // 3X3 버튼
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this._selectedSize = Size.Three;
        }

        // 5X5 버튼
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            this._selectedSize = Size.Five;
        }

        // 7X7 버튼
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            this._selectedSize = Size.Seven;
        }

        private void Size_Button_Visible(bool b)
        {
            radioButton5.Visible = b;
            radioButton6.Visible = b;
        }

        // RGB의 r,g,b 가 모두 같은경우 회색조 이미지가 된다. 참고바람.
        // 외곽 부분을 처리할때는 값을 어떻게 줘야 하는가?
        private void Execute_Filtering()
        {
            _srcBitImage = _srcImage as Bitmap;

            Filtering();
            _dstImage = _dstBitImage as Image;

            if (_dstImage != null)
            {
                pictureBox5.Image = _dstImage;
                pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            
        }

        private void Filtering()
        {
            SetMask();
            SetNewBitmap();
        }

        private void SetMask()
        {
            switch (_selectedSize)
            {
                case Size.Three:
                    if (_selectedFilter == Filter.Laplacian)
                    {
                        _convolutionMask = new int[3, 3]{
                            { 1, 1, 1 },
                            { 1, -8, 1 },
                            { 1, 1, 1 }
                        };
                    }
                    else
                    {
                        _convolutionMask = new int[3, 3];
                        SetArray(_convolutionMask, 3, 3, 1);
                    }
                    break;
                case Size.Five:
                    _convolutionMask = new int[5, 5];
                    SetArray(_convolutionMask, 5, 5, 1);
                    break;
                case Size.Seven:
                    _convolutionMask = new int[7, 7];
                    SetArray(_convolutionMask, 7, 7, 1);
                    break;
            }
        }

        private void SetNewBitmap()
        {
            int w = _srcBitImage.Width;
            int h = _srcBitImage.Height;
            int[,] image_Gray_Level=null;
            int size = 0;
            int trim = 0;

            switch (_selectedSize)
            {
                case Size.Three:
                    image_Gray_Level = new int[h + 2, w + 2];
                    if (_selectedFilter == Filter.Laplacian)
                    {
                        SetArray(ref image_Gray_Level, 3, h + 2, w + 2, 0);
                        size = 3;
                        trim = 1;
                        break;
                    }
                    SetArray(ref image_Gray_Level, 3, h + 2, w + 2, -1);
                    size = 3;
                    trim = 1;
                    break;
                case Size.Five:
                    image_Gray_Level = new int[h + 4, w + 4];
                    SetArray(ref image_Gray_Level, 5, h + 4, w + 4, -1);
                    size = 5;
                    trim = 2;
                    break;
                case Size.Seven:
                    image_Gray_Level = new int[h + 6, w + 6];
                    SetArray(ref image_Gray_Level, 7, h + 6, w + 6, -1);
                    size = 7;
                    trim = 3;
                    break;
            }

            Enter_Result_Value(image_Gray_Level, endX - trim, endY - trim, size);
            
        }

        private void SetArray(int[,] arr, int row, int column, int fillNum)
        {
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    arr[i, j] = fillNum;
        }

        private void SetArray(ref int[,] arr, int size, int row, int column, int fillNum)
        {
            int fix_Pixel_Cartesian = 0;
            switch (size)
            {
                case 3:
                    startX = startY = size - 2;
                    endX = column - 2;
                    endY = row - 2;
                    fix_Pixel_Cartesian = 1;
                    break;
                case 5:
                    startX = startY = size - 3;
                    endX = column - 3;
                    endY = row - 3;
                    fix_Pixel_Cartesian = 2;
                    break;
                case 7:
                    startX = startY = size - 4;
                    endX = column - 4;
                    endY = row - 4;
                    fix_Pixel_Cartesian = 3;
                    break;
            }

            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    arr[i, j] = fillNum;

            // 픽셀의 위치정보와 arr의 위치정보가 다름을 파악.
            for (int i = startY; i <= endY; i++)
                for (int j = startX; j <= endX; j++)
                    arr[i, j] = _srcBitImage.GetPixel(j - fix_Pixel_Cartesian, i - fix_Pixel_Cartesian).R;

        }

        // 콘볼루션 마스크를 가져오는 파라미터도 추가하면 어떨까?
        private void Enter_Result_Value(int [,]arr, int stop_X, int stop_Y, int size)
        {
            _dstBitImage = _srcBitImage.Clone() as Bitmap;
            Color color;
            int res = 0; 

            for (int i=0; i<stop_Y;i++)
            {
                for (int j=0; j < stop_X; j++)
                {
                    // 픽셀의 순서가 행렬의 순서와 같은지 확인해보기.
                    switch(_selectedFilter)
                    {
                        case Filter.Mean:
                            res = Mean_Result(ExtractArr(arr, j, i, j + size, i + size));
                            break;
                        case Filter.Median:
                            res = Median_Result(ExtractArr(arr, j, i, j + size, i + size));
                            break;
                        case Filter.Laplacian:
                            res = Laplacian_Result(ExtractArr(arr, j, i, j + size, i + size));
                            break;
                    }
                    
                    color= Color.FromArgb(_dstBitImage.GetPixel(j, i).A, res, res, res);
                    _dstBitImage.SetPixel(j, i, color);    

                }
            }
        }

        

        private int[,] ExtractArr(int[,] arr, int initX, int initY, int finishX, int finishY)
        {
            int length = _convolutionMask.GetLength(0);
            int[,] result = new int[length, length];
            Queue<int> list = new Queue<int>();


            for (int i = initY; i < finishY; i++)
                for (int j = initX; j < finishX; j++)
                    list.Enqueue(arr[i, j]);

            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    result[i, j] = list.Dequeue();

            return result;
        }

        private int Mean_Result(int[,] arr)
        {
            int length = _convolutionMask.GetLength(0);
            int count = arr.Length;
            int result = 0;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    if (arr[i, j] == -1)
                    {
                        count--;
                        continue;
                    }
                    result += arr[i, j] * _convolutionMask[i, j];
                }

            return (result == 0) ? 0 : (result / count);
        }

        private int Median_Result(int[,] arr)
        {
            int length = _convolutionMask.GetLength(0);
            List<int> list = new List<int>();

            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    if (arr[i, j] == -1)
                        continue;
                    list.Add(arr[i, j] * _convolutionMask[i, j]);
                }

            list.Sort();
            return list.ElementAt(list.Count / 2);
        }

        private int Laplacian_Result(int[,] arr)
        {
            int length = _convolutionMask.GetLength(0);
            int result = 0;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    result += arr[i, j] * _convolutionMask[i, j];

            if (result >= 0 && result <= 255) return result;
            else if (result < 0 && result >= -255) return Math.Abs(result);
            else return 255;
        }

        private void Browse_Picture()
        {
            string image_file = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.InitialDirectory = @"C:\";
            dialog.Filter = "모든 파일 (*.*)|*.*|JPEG File(*.jpg)|*.jpg|Bitmap File(*.bmp)|*.bmp|PNG File(*.png)|*.png";

            if (dialog.ShowDialog() == DialogResult.OK)
                image_file = dialog.FileName;
            else if (dialog.ShowDialog() == DialogResult.Cancel)
                return;

            _srcImage = Bitmap.FromFile(image_file);
            pictureBox2.Image = _srcImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Save_Picture()
        {
            if (_dstImage == null)
            {
                MessageBox.Show("이미지를 변환한 뒤에 눌러주세요!");
                return;
            }

            string fileName;
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "변환된 이미지 저장위치 지정";
            save.OverwritePrompt = true;
            save.Filter = "JPEG File(*.jpg)|*.jpg|Bitmap File(*.bmp)|*.bmp|PNG File(*.png)|*.png";
            
            if (save.ShowDialog() == DialogResult.OK)
            {
                fileName = save.FileName;
                _dstImage.Save(fileName);
            }
        }

        private void Show_Filter_Option()
        {
            string str = "";
            switch (_selectedFilter)
            {
                case Filter.Mean:
                    str += "Mean Filter";
                    break;
                case Filter.Median:
                    str += "Median Filter";
                    break;
                case Filter.Laplacian:
                    str += "Laplacian Filter";
                    break;
            }

            switch (_selectedSize)
            {
                case Size.Three:
                    str += " (3X3)";
                    break;
                case Size.Five:
                    str += " (5X5)";
                    break;
                case Size.Seven:
                    str += " (7X7)";
                    break;
            }

            textBox1.Text = str;
        }

    }
}
