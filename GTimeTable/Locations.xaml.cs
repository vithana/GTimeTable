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

namespace GTimeTable
{
    /// <summary>
    /// Interaction logic for Locations.xaml
    /// </summary>
    public partial class Locations : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();
        int buildingId;
        Building building = new Building();

        public Locations()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            buildingDataGrid.ItemsSource = _db.Buildings.ToList();

            saveBuildingName.Visibility = Visibility.Hidden;
        }

        private void clean() {
            buildingNameTextBox.Text = "";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }

        private void building_updateBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (buildingDataGrid.SelectedItem as Building).id;
            Building updateBuilding = (from m in _db.Buildings
                                             where m.id == Id
                                             select m).Single();
            buildingNameTextBox.Text = updateBuilding.name.ToString();
            buildingId = updateBuilding.id;
            saveBuildingName.Visibility = Visibility.Visible;
            addBuildingName.IsEnabled = false;
        }

        private void building_deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (buildingDataGrid.SelectedItem as Building).id;
            var deletedBuilding = _db.Buildings.Where(m => m.id == Id).Single();
            _db.Buildings.Remove(deletedBuilding);
            _db.SaveChanges();
            buildingDataGrid.ItemsSource = _db.Buildings.ToList();
        }

        private void AddBuildingName_Click(object sender, RoutedEventArgs e)
        {
            building.name = buildingNameTextBox.Text;
            _db.Buildings.Add(building);
            _db.SaveChanges();
            buildingDataGrid.ItemsSource = _db.Buildings.ToList();
            clean();
        }

        private void SaveBuildingName_Click(object sender, RoutedEventArgs e)
        {
            Building updateDays = (from m in _db.Buildings
                                             where m.id == buildingId
                                             select m).Single();
            updateDays.name = buildingNameTextBox.Text;
            _db.SaveChanges();
            buildingDataGrid.ItemsSource = _db.Buildings.ToList();
            clean();
            saveBuildingName.Visibility = Visibility.Hidden;
            addBuildingName.IsEnabled = true;
        }
    }
}
