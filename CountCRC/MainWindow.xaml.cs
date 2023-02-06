using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CountCRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextRange range = null;
        private ViewModel m_viewmodel;
        public ViewModel viewmodel
        {
            get { return m_viewmodel; }
            set { m_viewmodel = value; }
        }

        public MainWindow()
        {
            InitializeComponent();

            viewmodel = new ViewModel();
            this.DataContext = viewmodel.parameterlist[ElementDefine.selectIndex];
        }


        #region Button Action
        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            bool bFile = false;
            string fContent = string.Empty;
            string fileName = "Txt";
            string title = "Open Txt File";
            string filter = "Hex files (*.hex)|*.hex|Txt files (*.txt)|*.txt|Bin files(*.bin)|*.bin||";

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = title;
            openFileDialog.Filter = filter;
            openFileDialog.FileName = fileName;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            if (openFileDialog.ShowDialog() == false) return;

            FileInfo pfl = new FileInfo(openFileDialog.FileName);
            filePathLabel.Content = pfl.FullName;

            switch (pfl.Extension.Replace(".", "").ToLower())
            {
                case "hex":
                    bFile = ParseHexFile(pfl.FullName, ref fContent);
                    break;
                case "bin":
                    bFile = ParseBinFile(pfl.FullName, ref fContent);
                    break;
                case "txt":
                    bFile = ParseTxtFile(pfl.FullName, ref fContent);
                    break;
            }
            if (!bFile)
            {
                MessageBox.Show("加载数据文件失败，请检查！！");
                return;
            }
            range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(fContent)))
            {
                range.Load(ms, DataFormats.Text);
            }
        }

        private void countBtn_Click(object sender, RoutedEventArgs e)
        {
            bool bFile = false;
            Model model = null;
            List<byte> dataBuff = new List<byte>();
            UInt32 wCrc = 0;
            bFile = legalCheck(ref dataBuff);
            if (!bFile)
            {
                MessageBox.Show("非法16进制数据，请检查！！");
                return;
            }
            wCrc = CountCRC(dataBuff);
            model = crcAlgorithmCB.DataContext as Model;
            if (model == null) return;
            switch (model.algorithm_Width)
            {
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                    hexResultLab.Content = String.Format("{0:x2}", wCrc).ToUpper();
                    binResultLab.Content = String.Join(String.Empty, hexResultLab.Content.ToString().Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                    break;
                case "16":
                    hexResultLab.Content = String.Format("{0:x4}", wCrc).ToUpper();
                    binResultLab.Content = String.Join(String.Empty, hexResultLab.Content.ToString().Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                    break;
                case "32":
                    hexResultLab.Content = String.Format("{0:x8}", wCrc).ToUpper();
                    binResultLab.Content = String.Join(String.Empty, hexResultLab.Content.ToString().Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
                    break;

            }
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();
        }

        private void hexResultCopyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(hexResultLab.Content.ToString())) return;
            hexResultLab.Focus();
            Clipboard.SetDataObject(hexResultLab.Content);
        }

        private void binResultCopyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(binResultLab.Content.ToString())) return;
            binResultLab.Focus();
            Clipboard.SetDataObject(binResultLab.Content);
        }
        private void crcAlgorithmCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedValue == null) return;

            Model model = viewmodel.parameterlist[cmb.SelectedIndex];
            if (model == null) return;
            model.algorithm_Index = cmb.SelectedIndex;
            this.DataContext = model;
        }

        private void inToggleButton_Click(object sender, RoutedEventArgs e)
        {
            range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string sContent = range.Text.Trim().Replace("\r", "").Replace("\n", "").ToLower();
            string[] sArray = sContent.Split(" ");
            if (sArray.Length == 0)
            {
                MessageBox.Show("无效数据，请检查！！！");
                return;
            }
            Array.Reverse(sArray);
            range.Text = String.Join(" ", sArray);
        }

        private void outToggleButton_Click(object sender, RoutedEventArgs e)
        {
            UInt32 wval = 0;
            UInt16 uvalH = 0, uvalL = 0;
            if (string.IsNullOrWhiteSpace(hexResultLab.Content.ToString())) return;
            string str = hexResultLab.Content.ToString().Trim().ToLower();
            switch (str.Length)
            {
                case 2:
                    break;
                case 4:
                    uvalL = Convert.ToUInt16(str, 16);
                    uvalL = (UInt16)((uvalL & 0x00FF) << 8 | (uvalL & 0xFF00) >> 8);
                    hexResultLab.Content = String.Format("{0:x4}", uvalL).ToUpper();
                    break;
                case 8:
                    wval = Convert.ToUInt32(str, 16);
                    uvalL = (UInt16)wval;
                    uvalL = (UInt16)((uvalL & 0x00FF) << 8 | (uvalL & 0xFF00) >> 8);
                    uvalH = (UInt16)(wval >> 16);
                    uvalH = (UInt16)((uvalH & 0x00FF) << 8 | (uvalH & 0xFF00) >> 8);
                    wval = (UInt32)(uvalL << 16 | uvalH);
                    hexResultLab.Content = String.Format("{0:x8}", wval).ToUpper();
                    break;
            }
            char[] cArray = binResultLab.Content.ToString().Trim().ToCharArray();
            Array.Reverse(cArray);
            binResultLab.Content = String.Join("", cArray);
        }
        #endregion

        #region Functions
        public bool ParseHexFile(string fullPath, ref string fContent)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return false;
            if (!File.Exists(fullPath)) return false;
            fContent = File.ReadAllText(fullPath, Encoding.Default);
            return true;
        }

        public bool ParseBinFile(string fullPath, ref string fContent)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return false;
            if (!File.Exists(fullPath)) return false;
            return true;
        }

        public bool ParseTxtFile(string fullPath, ref string fContent)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) return false;
            if (!File.Exists(fullPath)) return false;
            return true;
        }

        public bool legalCheck(ref List<byte> dataBuff)
        {
            range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            string sContent = range.Text.Trim().Replace("\r", "").Replace("\n", "").ToLower();
            string[] sArray = sContent.Split(" ");
            foreach (string s in sArray)
            {
                try
                {
                    dataBuff.Add(Convert.ToByte(s, 16));
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public UInt32 CountCRC(List<byte> dataBuff)
        {
            UInt32 wCrc = 0;
            switch (crcAlgorithmCB.SelectedItem.ToString().Trim())
            {
                case "CRC-4-ITU":
                    break;
                case "CRC-5-EPC":
                    break;
                case "CRC-5-ITU":
                    break;
                case "CRC-5-USB":
                    break;
                case "CRC-6-ITU":
                    break;
                case "CRC-7-MMC":
                    break;
                case "CRC-8":
                    wCrc = (UInt32)Algorithm.CRC8(dataBuff.ToArray(), dataBuff.Count);
                    break;
                case "CRC-8-ITU":
                    break;
                case "CRC-8-ROHC":
                    break;
                case "CRC-8-MAXIM":
                    break;
                case "CRC-16-IBM":
                    wCrc = (UInt32)Algorithm.CRC16_IBM(dataBuff.ToArray(), dataBuff.Count);
                    break;
                case "CRC-16-MAXIM":
                    wCrc = (UInt32)Algorithm.CRC16_MAXIM(dataBuff.ToArray(), dataBuff.Count);
                    break;
                case "CRC-16-USB":
                    break;
                case "CRC-16-MODBUS":
                    break;
                case "CRC-16-CCITT":
                    break;
                case "CRC-16-CCITT-FALSE":
                    break;
                case "CRC-16-X25":
                    wCrc = (UInt32)Algorithm.CRC16_X25(dataBuff.ToArray(), dataBuff.Count);
                    break;
                case "CRC-16-XMODEM":
                    break;
                case "CRC-16-XMODEM2":
                    break;
                case "CRC-16-DNP":
                    break;
                case "CRC-32":
                    break;
                case "CRC-32-C":
                    break;
                case "CRC-32-KOOPMAN":
                    break;
                case "CRC-32-MPEG-2":
                    break;
                case "CRC-64-ISO":
                    break;
                case "CRC-64-ECMA":
                    break;
            }
            return wCrc;
        }
        #endregion
    }
}
