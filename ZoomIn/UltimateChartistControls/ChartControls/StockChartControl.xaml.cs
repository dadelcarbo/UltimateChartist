﻿using System;
using System.Collections.Generic;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace UltimateChartistControls.ChartControls
{
    /// <summary>
    /// Interaction logic for StockChartControl.xaml
    /// </summary>
    public partial class StockChartControl : UserControl
    {
        public StockChartControl(ChartControlViewModel viewModel)
        {
            InitializeComponent();

            foreach (var child in grid.ChildrenOfType<ChartControlBase>())
            {
                child.ViewModel = viewModel;
            }
        }
    }
}
