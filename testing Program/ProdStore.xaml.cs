﻿using Syncfusion.SfSkinManager;
using System;
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

namespace testing_Program
{
    /// <summary>
    /// Interaction logic for ProdStore.xaml
    /// </summary>
    public partial class ProdStore : Page
    {
        public ProdStore()
        {
            SfSkinManager.ApplyStylesOnApplication = true;
            InitializeComponent();
        }
    }
}
