using Syncfusion.PMML;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Net.Mime.MediaTypeNames;

namespace testing_Program
{
    /// <summary>
    /// Interaction logic for Equipment.xaml
    /// </summary>
    public partial class Equipment : Page
    {
        private List<string> tractors = new List<string>();
        private ObservableCollection<TractInfo> items;
        public Equipment()
        {
            InitializeComponent();
            stPanTract.Visibility = Visibility.Hidden;
            stSprayTract.Visibility = Visibility.Hidden;
            LoadInfo();
        }

        #region Tractors
        public void chkReqTTrail_Checked(object sender, RoutedEventArgs e)
        {
            stPanTract.Visibility = Visibility.Visible;
        }

        public void chkReqTTrail_UnChecked(object sender, RoutedEventArgs e)
        {
            stPanTract.Visibility = Visibility.Hidden;
        }

        public void chkReqT_Checked(object sender, RoutedEventArgs e)
        {
            stSprayTract.Visibility = Visibility.Visible;
        }

        public void chkReqT_Unchecked(object sender, RoutedEventArgs e)
        {
            stSprayTract.Visibility = Visibility.Hidden;
        }

        private void btnTracSave_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtTracMake.Text))
            {
                Message message = new Message("No Tractor Model Entered");
                message.ShowDialog();
                return;
            }

            if(string.IsNullOrEmpty(txtTracMod.Text))
            {
                Message message = new Message("No Tractor Model Entered");
                message.ShowDialog();
                return;
            }

            using (var cont = new TableContext())
            {
                if(string.IsNullOrEmpty(lblTracID.Content.ToString()))
                {
                    Tractors tract = new Tractors
                    {
                        TractorMake = txtTracMake.Text,
                        TractorModel = txtTracMod.Text,
                        Registration = txtTracReg.Text,
                        Remarks = txtTracRem.Text,
                        Name = txtTracName.Text,
                    };
                    cont.Tractors.Add(tract);
                }
                else
                {
                    int tractID = Convert.ToInt32(lblTracID.Content.ToString());
                    var tract = cont.Tractors.FirstOrDefault(t => t.TractorID == tractID);

                    if(tract != null)
                    {
                        tract.TractorMake = txtTracMake.Text;
                        tract.TractorModel = txtTracMod.Text;
                        tract.Remarks = txtTracRem.Text;
                        tract.Registration = txtTracReg.Text;
                        tract.Name = txtTracName.Text;
                    }
                }
                cont.SaveChanges();
            }
            LoadInfo();
            ClearTract();
        }

        private void ClearTract()
        {
            txtTracMake.Text = string.Empty;
            txtTracMod.Text = string.Empty;
            txtTracName.Text = string.Empty;
            txtTracReg.Text = string.Empty;
            txtTracRem.Text = string.Empty;
            chkTracAct.IsChecked = true;
            btnTracNew.IsEnabled = false;
            lblTracID.Content = string.Empty;
        }

        private void btnTracCan_Click(object sender, RoutedEventArgs e)
        {
            ClearTract() ;
        }

        private void grdTracts_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            dynamic selectedRow = grdTracts.SelectedItem;
            if (selectedRow != null)
            {
                int tractID = Convert.ToInt32(selectedRow.TractorID);

                using (var cont = new TableContext())
                {
                    var tracts = cont.Tractors
                        .FirstOrDefault(t => t.TractorID == tractID);

                    if (tracts != null)
                    {
                        txtTracMake.Text = tracts.TractorMake;
                        txtTracMod.Text = tracts.TractorModel;
                        txtTracName.Text = tracts.Name;
                        txtTracReg.Text = tracts.Registration;
                        txtTracRem.Text = tracts.Remarks;
                        lblTracID.Content = tracts.TractorID;
                        btnTracNew.IsEnabled = true;
                    }
                }
            }
            else
            {
                lblTracID.Content = "";
                btnTracNew.IsEnabled = false;
            }
        }

        private void btnTracNew_Click(object sender, RoutedEventArgs e)
        {
            ClearTract();
        }
        #endregion

        #region Sprayers
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClearSpray();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtSprayMake.Text))
            {
                Message message = new Message("No Sprayer Make Entered");
                message.ShowDialog();
                return;
            }
            if(string.IsNullOrEmpty(txtSprayMod.Text))
            {
                Message message = new Message("No Sprayer Model Entered");
                message.ShowDialog();
                return;
            }

            int sprayID = 0;
            int chkReq = Convert.ToInt32(chkReqT.IsChecked);

            using (var cont = new TableContext())
            {
                if (string.IsNullOrEmpty(lblSprayID.Content?.ToString()))
                {
                    Sprayers sprayers = new Sprayers
                    {
                        SprayerMake = txtSprayMake.Text,
                        SprayerModel = txtSprayMod.Text,
                        Volume = Convert.ToInt32(txtSprayVol.Text),
                        Name = txtSprayName.Text,
                        Remarks = txtSprayRem.Text,
                        ReqTractor = chkReq,
                    };
                    cont.Sprayers.Add(sprayers);
                    cont.SaveChanges();
                    sprayID = sprayers.SprayerID;
                }
                else
                {
                    var spray = cont.Sprayers
                        .FirstOrDefault(s => s.SprayerID == Convert.ToInt32(lblSprayID.Content.ToString()));

                    sprayID = spray.SprayerID;
                    spray.Name = txtSprayName.Text;
                    spray.SprayerModel = txtSprayMod.Text;
                    spray.SprayerMake = txtSprayMake.Text;
                    spray.Remarks = txtSprayRem.Text;
                    spray.Volume = Convert.ToInt32(txtSprayVol.Text);
                    spray.ReqTractor = chkReq;
                }

                foreach (TractInfo item in chkListTractsSpray.ItemsSource)
                {
                    var trackSpray = cont.TractorSprayer.FirstOrDefault(ts => ts.TractorID == item.TractorID && ts.SprayerID == sprayID);

                    if (chkListTractsSpray.SelectedItems.Contains(item))
                    {
                        if (trackSpray == null)
                        {
                            TractorSprayer tractorSprayer = new TractorSprayer
                            {
                                TractorID = item.TractorID,
                                SprayerID = sprayID,
                            };
                            cont.TractorSprayer.Add(tractorSprayer);
                        }
                    }
                    else
                    {
                        if (trackSpray != null)
                        {
                            cont.TractorSprayer.Remove(trackSpray);
                        }
                    }
                }
                cont.SaveChanges();
            }
            MessageBox.Show("Saved Info");
            ClearSpray();
            LoadInfo();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ClearSpray();
        }

        private void grdSprayers_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            dynamic selectedRow = grdSprayers.SelectedItem;
            if (selectedRow != null)
            {
                int sprayID = Convert.ToInt32(selectedRow.SprayerID);

                using (var cont = new TableContext())
                {
                    var tracts = cont.Sprayers
                        .FirstOrDefault(t => t.SprayerID == sprayID);

                    if (tracts != null)
                    {
                        chkListTractsSpray.SelectedItems.Clear();

                        txtSprayMake.Text = tracts.SprayerMake;
                        txtSprayMod.Text = tracts.SprayerModel;
                        txtSprayName.Text = tracts.Name;
                        txtSprayVol.Text = tracts.Volume.ToString();
                        txtSprayRem.Text = tracts.Remarks;
                        lblSprayID.Content = tracts.SprayerID;
                        btnSprayNew.IsEnabled = true;

                        if (Convert.ToBoolean(tracts.ReqTractor))
                        {
                            chkReqT.IsChecked = true;
                        }
                        else
                        {
                            chkReqT.IsChecked = false;
                        }

                        var linkedTracts = cont.TractorSprayer
                            .Where(ts => ts.SprayerID == tracts.SprayerID)
                            .Select(ts => ts.TractorID)
                            .ToList();

                        if(linkedTracts != null)
                        {
                            foreach(TractInfo item in chkListTractsSpray.ItemsSource)
                            {
                                if (linkedTracts.Contains(item.TractorID))
                                {
                                    chkListTractsSpray.SelectedItems.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                lblSprayID.Content = "";
                btnSprayNew.IsEnabled = false;
            }
        }

        private void ClearSpray()
        {
            txtSprayMake.Text = string.Empty;
            txtSprayMod.Text = string.Empty;
            txtSprayName.Text = string.Empty;
            txtSprayRem.Text = string.Empty;
            txtSprayVol.Text = string.Empty;
            chkListTractsSpray.SelectedItems.Clear();
        }
        #endregion

        #region Trailers
        private void btnTrailAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearTrail();
        }

        private void btnTrailSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtTrailMake.Text == string.Empty)
            {
                Message message = new Message("No Trailer Make Entered");
                message.ShowDialog();
                return;
            }
            if(txtTrailMod.Text == string.Empty)
            {
                Message message = new Message("No Trailer Model Entered");
                message.ShowDialog();
                return;
            }

            int trailID = 0;
            using (var cont = new TableContext())
            {
                if (string.IsNullOrEmpty(lblTrailID.Content?.ToString()))
                {
                    Trailers trailer = new Trailers
                    {
                        TrailerMake = txtTrailMake.Text,
                        TrailerModel = txtTrailMod.Text,
                        Remarks = txtTrailRem.Text,
                        Capacity = Convert.ToInt32(txtVol.Text),
                        Name = txtTrailName.Text,
                        ReqTractor = Convert.ToInt32(chkReqTTrail.IsChecked)
                    };
                    cont.Trailers.Add(trailer);
                    cont.SaveChanges();
                    trailID = trailer.TrailerID;
                }
                else
                {
                    trailID = Convert.ToInt32(lblTrailID.Content?.ToString());
                    var trailer = cont.Trailers
                        .FirstOrDefault(tr => tr.TrailerID == trailID);

                    trailer.TrailerMake = txtTrailMake.Text;
                    trailer.TrailerModel = txtTrailMod.Text;
                    trailer.Name = txtTrailName.Text;
                    trailer.Capacity = Convert.ToInt32(txtVol.Text);
                    trailer.Remarks = txtTrailRem.Text;
                    trailer.ReqTractor = Convert.ToInt32(chkReqTTrail.IsChecked);
                }

                if(chkReqTTrail.IsChecked == true)
                {
                    foreach (TractInfo item in chkListTractsTrail.ItemsSource)
                    {
                        var trackTrail = cont.TractorTrailer.FirstOrDefault(ts => ts.TractorID == item.TractorID && ts.TrailerID == trailID);

                        if (chkListTractsTrail.SelectedItems.Contains(item))
                        {
                            if (trackTrail == null)
                            {
                                TractorTrailer tractorTrail = new TractorTrailer
                                {
                                    TractorID = item.TractorID,
                                    TrailerID = trailID,
                                };
                                cont.TractorTrailer.Add(tractorTrail);
                            }
                        }
                        else
                        {
                            if (trackTrail != null)
                            {
                                cont.TractorTrailer.Remove(trackTrail);
                            }
                        }
                    }
                }
                cont.SaveChanges();
            }
            MessageBox.Show("Save Complete");
            LoadInfo();
            ClearTrail();
        }

        private void btnTrailCan_Click(object sender, RoutedEventArgs e)
        {
            ClearTrail();
        }

        private void grdTrailers_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            dynamic selectedRow = grdTrailers.SelectedItem;
            if (selectedRow != null)
            {
                int sprayID = Convert.ToInt32(selectedRow.TrailerID);

                using (var cont = new TableContext())
                {
                    var tracts = cont.Trailers
                        .FirstOrDefault(t => t.TrailerID == sprayID);

                    if (tracts != null)
                    {
                        chkListTractsTrail.SelectedItems.Clear();

                        txtTrailMake.Text = tracts.TrailerMake;
                        txtTrailMod.Text = tracts.TrailerModel;
                        txtTrailName.Text = tracts.Name;
                        txtVol.Text = tracts.Capacity.ToString();
                        txtTrailRem.Text = tracts.Remarks;
                        lblTrailID.Content = tracts.TrailerID;
                        btnTrailAdd.IsEnabled = true;

                        if (Convert.ToBoolean(tracts.ReqTractor))
                        {
                            chkReqTTrail.IsChecked = true;
                        }
                        else
                        {
                            chkReqTTrail.IsChecked = false;
                        }

                        var linkedTracts = cont.TractorTrailer
                            .Where(ts => ts.TrailerID == tracts.TrailerID)
                            .Select(ts => ts.TractorID)
                            .ToList();

                        if (linkedTracts != null)
                        {
                            foreach (TractInfo item in chkListTractsTrail.ItemsSource)
                            {
                                if (linkedTracts.Contains(item.TractorID))
                                {
                                    chkListTractsTrail.SelectedItems.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                lblSprayID.Content = "";
                btnSprayNew.IsEnabled = false;
            }
        }

        private void ClearTrail()
        {
            txtTrailMake.Text = string.Empty;
            txtTrailMod.Text = string.Empty;
            txtTrailName.Text = string.Empty;
            txtTrailRem.Text = string.Empty;
            txtVol.Text = string.Empty;
            chkReqTTrail.IsChecked = false;
            chkListTractsTrail.SelectedItems.Clear();
            lblTrailID.Content = null;
        }
        #endregion

        #region General Stuff
        private void LoadInfo()
        {
            using (var cont = new TableContext())
            {
                var tracts = cont.Tractors
                    .ToList();
                grdTracts.ItemsSource = tracts;

                var sprays = cont.Sprayers
                    .ToList();
                grdSprayers.ItemsSource = sprays;

                var trails = cont.Trailers
                    .ToList();
                grdTrailers.ItemsSource = trails;

                chkListTractsSpray.ItemsSource = GetInfo();
                chkListTractsSpray.DisplayMemberPath = "Tractor";
                chkListTractsTrail.ItemsSource = GetInfo();
                chkListTractsTrail.DisplayMemberPath = "Tractor";
            }
        }

        private List<TractInfo> GetInfo()
        {
            using (var cont = new TableContext())
            {
                var tracts = cont.Tractors
                    .Select(t => new TractInfo
                    {
                        TractorID = t.TractorID,
                        Tractor = t.Name + ", " + t.TractorMake + " - " + t.TractorModel,
                    }).ToList();
                return tracts;
            }
        }

        #endregion
    }
}
