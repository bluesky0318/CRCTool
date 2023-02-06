using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountCRC
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #region Algorithm
        private ObservableCollection<string> m_Algorithm_Collection = new ObservableCollection<string>();
        public ObservableCollection<string> algorithm_Collection
        {
            get { return m_Algorithm_Collection; }
            set { m_Algorithm_Collection = value; }
        }

        private int m_Algorithm_Index;
        public int algorithm_Index
        {
            get { return m_Algorithm_Index; }
            set
            {
                if (m_Algorithm_Index == value) return;
                m_Algorithm_Index = value;
                OnPropertyChanged("algorithm_Index");
            }
        }

        private string m_Algorithm_Name;
        public string algorithm_Name
        {
            get { return m_Algorithm_Name; }
            set
            {
                m_Algorithm_Name = value;
                OnPropertyChanged("algorithm_Name");
            }
        }

        private string m_Algorithm_Polynomial;
        public string algorithm_Polynomial
        {
            get { return m_Algorithm_Polynomial; }
            set
            {
                m_Algorithm_Polynomial = value;
                OnPropertyChanged("algorithm_Polynomial");
            }
        }

        private string m_Algorithm_Width;
        public string algorithm_Width
        {
            get { return m_Algorithm_Width; }
            set
            {
                m_Algorithm_Width = value;
                OnPropertyChanged("algorithm_Width");
            }
        }

        private string m_Algorithm_Poly;
        public string algorithm_Poly
        {
            get { return m_Algorithm_Poly; }
            set
            {
                m_Algorithm_Poly = value;
                OnPropertyChanged("algorithm_Poly");
            }
        }

        private string m_Algorithm_InitValue;
        public string algorithm_InitValue
        {
            get { return m_Algorithm_InitValue; }
            set
            {
                m_Algorithm_InitValue = value;
                OnPropertyChanged("algorithm_InitValue");
            }
        }

        private string m_Algorithm_XOROUT;
        public string algorithm_XOROUT
        {
            get { return m_Algorithm_XOROUT; }
            set
            {
                m_Algorithm_XOROUT = value;
                OnPropertyChanged("algorithm_XOROUT");
            }
        }

        private string m_Algorithm_Summary;
        public string algorithm_Summary
        {
            get { return m_Algorithm_Summary; }
            set
            {
                m_Algorithm_Summary = value;
                OnPropertyChanged("algorithm_Summary");
            }
        }
        
        #endregion

        #region Element
        private String m_outToggleStatus = "关";
        public String outToggleStatus
        {
            get { return m_outToggleStatus; }
            set
            {
                m_outToggleStatus = value;
                OnPropertyChanged("outToggleStatus");
            }
        }

        private String m_inToggleStatus = "关";
        public String inToggleStatus
        {
            get { return m_inToggleStatus; }
            set
            {
                m_inToggleStatus = value;
                OnPropertyChanged("inToggleStatus");
            }
        }

        private bool m_ReversalInBtnIsCheck = false;
        public bool reversalInBtnIsCheck
        {
            get { return m_ReversalInBtnIsCheck; }
            set
            {
                m_ReversalInBtnIsCheck = value;
                OnPropertyChanged("reversalInBtnIsCheck");
                inToggleStatus = reversalInBtnIsCheck ? "开" : "关";
            }
        }

        private bool m_ReversalOutBtnIsCheck = false;
        public bool reversalOutBtnIsCheck
        {
            get { return m_ReversalOutBtnIsCheck; }
            set
            {
                m_ReversalOutBtnIsCheck = value;
                OnPropertyChanged("reversalOutBtnIsCheck");
                outToggleStatus = reversalOutBtnIsCheck ? "开" : "关";
            }
        }
        #endregion
    }
}
