using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountCRC
{
    public class ViewModel
    {
        private ObservableCollection<Model> m_ParameterList = new ObservableCollection<Model>();
        public ObservableCollection<Model> parameterlist 
        {
            get { return m_ParameterList; }
            set { m_ParameterList = value; }
        }

        public ViewModel()
        {
            Model model = null;
            parameterlist.Clear();
            foreach (var param in Algorithm.algorithm_Collection)
            {
                model = new Model();
                model.algorithm_Collection = new ObservableCollection<string>(Algorithm.algorithm_Collection.Select(t => t.Item1).ToList());
                model.algorithm_Name = param.Item1;
                model.algorithm_Polynomial = param.Item2;
                model.algorithm_Width = param.Item3;
                model.algorithm_Poly = param.Item4;
                model.algorithm_InitValue = param.Item5;
                model.algorithm_XOROUT = param.Item6;
                model.algorithm_Summary = param.Item7;
                model.algorithm_Index = ElementDefine.selectIndex;
                parameterlist.Add(model);
            }
        }
    }
}
