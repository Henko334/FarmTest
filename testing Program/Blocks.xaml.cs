using Syncfusion.SfSkinManager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using testing_Program.Entities;
using static Syncfusion.Windows.Forms.TabBar;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static testing_Program.Entities.TableContext;

namespace testing_Program
{
    public partial class Blocks : Page
    {
        private TableContext _dbContext;
        public Blocks()
        {
            SfSkinManager.ApplyStylesOnApplication = true; // Applies theme globally
            SfSkinManager.SetVisualStyle(Application.Current.MainWindow, VisualStyles.Lime);
            InitializeComponent();
            _dbContext = new TableContext();

            var dataFromDb = _dbContext.BlockData
                .Select(b => new
                {
                    BlockID = b.BlockID,
                    BlockName = b.BlockName,
                    FruitID = b.FruitID,
                    FruitType = b.FruitType.FruitName,
                    VarID = b.VarID,
                    Variant = b.FruitVariant.VarName,
                    Size = b.Size,
                })
                .ToList();
            grdBlocks.ItemsSource = dataFromDb;

            var fruits = _dbContext.FruitType
                .ToList();
            cboFruit.ItemsSource = fruits;
        }

        private void btnAddBlock_Clicked(object sender, RoutedEventArgs e)
        {
            ClearInfo();
            btnAdd.Visibility = Visibility.Hidden;
        }

        private void OnSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            dynamic selectedRow = grdBlocks.SelectedItem;
            int blockID = Convert.ToInt32(selectedRow.BlockID);

            LoadInfo(blockID);
        }

        private void cboFruitChange(object sender, RoutedEventArgs e)
        {
            var cults = _dbContext.FruitVariant
                .Where(f => f.FruitID == Convert.ToInt32(cboFruit.SelectedValue))
                .OrderBy(f => f.VarName)
                .ToList();
            cboVar.ItemsSource = cults;
        }

        private void DatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(dtpPlant.Text)) { return; }
            DateTime selectedDate = Convert.ToDateTime(dtpPlant.Text);
            DateTime currentDate = DateTime.Now;

            int yearDifference = currentDate.Year - selectedDate.Year;

            if (currentDate.Month < selectedDate.Month || (currentDate.Month == selectedDate.Month && currentDate.Day < selectedDate.Day))
            {
                yearDifference--;
            }
            txtAge.Text = yearDifference.ToString();
        }

        private void btnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBlock.Text))
            {
                MessageBox.Show("No Block Name Added", "Error");
                return;
            }

            if (string.IsNullOrEmpty(cboFruit.Text))
            {
                MessageBox.Show("No Fruit Type Selected", "Error");
                return;
            }

            if (string.IsNullOrEmpty(cboVar.Text))
            {
                MessageBox.Show("No Cultivar Selected", "Error");
                return;
            }

            if (!string.IsNullOrEmpty(txtBlockID.Text))
            {
                var blockDets = _dbContext.BlockData
                    .FirstOrDefault(b => b.BlockID == Convert.ToInt32(txtBlockID.Text));

                if(blockDets != null) 
                {
                    blockDets.BlockName = txtBlock.Text;
                    blockDets.Size = Convert.ToDecimal(txtSize.Text);
                    blockDets.FruitID = Convert.ToInt32(cboFruit.SelectedValue);
                    blockDets.VarID = Convert.ToInt32(cboVar.SelectedValue);
                }

                var blockInfo = _dbContext.BlockDetails
                    .FirstOrDefault(b => b.BlockID ==  blockDets.BlockID);

                if (blockInfo != null)
                {
                    blockInfo.PlantingDate = Convert.ToDateTime(dtpPlant.Text);
                    blockInfo.Age = Convert.ToInt32(txtAge.Text);
                    blockInfo.PlantSpacing = Convert.ToDecimal(txtPspace.Text);
                    blockInfo.RowSpacing = Convert.ToDecimal(txtRspace.Text);
                    blockInfo.PlantsHA = Convert.ToInt32(txtPlants.Text);
                }
                else
                {
                    BlockDetails dets = new BlockDetails
                    {
                        BlockID = blockDets.BlockID,
                        PlantingDate = Convert.ToDateTime(dtpPlant.Text),
                        Age = Convert.ToInt32(txtAge.Text),
                        PlantSpacing = Convert.ToDecimal(txtPspace.Text),
                        RowSpacing = Convert.ToDecimal(txtRspace.Text),
                        PlantsHA = Convert.ToInt32(txtPlants.Text),
                    };
                    _dbContext.BlockDetails.Add(dets);
                }
                _dbContext.SaveChanges();
            }
            else
            {
                BlockData block = new BlockData
                {
                    BlockName = txtBlock.Text,
                    Size = Convert.ToDecimal(txtSize.Text),
                    FruitID = Convert.ToInt32(cboFruit.SelectedValue),
                    VarID = Convert.ToInt32(cboVar.SelectedValue),
                };
                _dbContext.BlockData.Add(block);
                _dbContext.SaveChanges();

                BlockDetails dets = new BlockDetails
                {
                    BlockID = block.BlockID,
                    PlantingDate = Convert.ToDateTime(dtpPlant.Text),
                    Age = Convert.ToInt32(txtAge.Text),
                    PlantSpacing = Convert.ToDecimal(txtPspace.Text),
                    RowSpacing = Convert.ToDecimal(txtRspace.Text),
                    PlantsHA = Convert.ToInt32(txtPlants.Text),
                };
                _dbContext.BlockDetails.Add(dets);
                _dbContext.SaveChanges();
            }
        }

        private void txtPsapce_FocusLost(object sender, EventArgs e)
        {
            CalcPlants();
        }

        private void txtRspace_FocusLost(object sender, EventArgs e)
        {
            CalcPlants();
        }

        private void txtSize_FocusLost(object sender, EventArgs e)
        {
            CalcPlants();
        }

        private void CalcPlants()
        {
            decimal size = 0;
            decimal rSize = 0;
            decimal pSize = 0;

            if (!string.IsNullOrEmpty(txtSize.Text))
            {
                if (!int.TryParse(txtSize.Text, out int qty))
                {
                    MessageBox.Show("Can Only Enter Numeric Values");
                    txtSize.Text = string.Empty;
                    return;
                }
                size = Convert.ToDecimal(txtSize.Text);
            }

            if (!string.IsNullOrEmpty(txtPspace.Text))
            {
                pSize = Convert.ToDecimal(txtPspace.Text);
            }

            if(!string.IsNullOrEmpty(txtRspace.Text))
            {
                rSize = Convert.ToDecimal( txtRspace.Text);
            }

            size = size * 10000;
            int plants = 0;

            if (pSize != 0 && rSize != 0)
            {
                plants = Convert.ToInt32(size / (pSize * rSize));
            }
            txtPlants.Text = plants.ToString();
        }

        private void LoadInfo(int blockID)
        {
            ClearInfo();
            var blockDets = _dbContext.BlockDetails
                .FirstOrDefault(b => b.BlockID == blockID);

            var blockData = _dbContext.BlockData
                .Select(b => new
                {
                    BlockID = b.BlockID,
                    BlockName = b.BlockName,
                    Size = b.Size,
                    FruitID = b.FruitID,
                    Variant = b.VarID,
                })
                .FirstOrDefault(b => b.BlockID == blockID);

            txtBlock.Text = blockData.BlockName;
            txtBlockID.Text = blockID.ToString();
            txtSize.Text = blockData.Size.ToString();
            cboFruit.SelectedValue = blockData.FruitID;
            cboVar.SelectedValue = blockData.Variant;

            if(blockDets != null)
            {
                txtAge.Text = blockDets.Age.ToString();
                txtPlants.Text = blockDets.PlantsHA.ToString();
                txtPspace.Text = blockDets.PlantSpacing.ToString();
                txtRspace.Text = blockDets.RowSpacing.ToString();
                dtpPlant.Text = blockDets.PlantingDate.ToString();
            }
        }

        private void ClearInfo()
        {
            txtPlants.Clear();
            txtPspace.Clear();
            txtRspace.Clear();
            txtSize.Clear();
            txtAge.Clear();
            txtBlock.Clear();
            txtBlockID.Clear();
            cboFruit.SelectedIndex = 0;
            cboVar.SelectedIndex = 0;
            dtpPlant.Text = null;
            btnAdd.Visibility = Visibility.Visible;
        }
    }
}
