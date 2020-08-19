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
    /// Interaction logic for WorkingDaysAndHours.xaml
    /// </summary>
    public partial class WorkingDaysAndHours : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();
        int workingDaysPerWeekID;
        WorkingDaysPerWeek workingDaysPerWeek = new WorkingDaysPerWeek();

        int workingTimePerDayId;
        WorkingTimePerDay time = new WorkingTimePerDay();
        



        public WorkingDaysAndHours()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            workingDaysPerWeekDataGrid.ItemsSource = _db.WorkingDaysPerWeeks.ToList();
            workingTimePerDayDataGrid.ItemsSource = _db.WorkingTimePerDays.ToList();
            
            workingDaysPerWeekSave.Visibility = Visibility.Hidden;
            saveTimePerDayButton.Visibility = Visibility.Hidden;
        }
        private void clean()
        {
            numberTextBox.Text = "";
            time_per_day.Text = "";
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }

        //Add working days per week
        private void AddWorkingDaysPerWeek_Click(object sender, RoutedEventArgs e)
        {
            workingDaysPerWeek.number = int.Parse(numberTextBox.Text);
            _db.WorkingDaysPerWeeks.Add(workingDaysPerWeek);
            _db.SaveChanges();
            workingDaysPerWeekDataGrid.ItemsSource = _db.WorkingDaysPerWeeks.ToList();
            clean();
        }

        //get details of working days of week
        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (workingDaysPerWeekDataGrid.SelectedItem as WorkingDaysPerWeek).id;
            WorkingDaysPerWeek updateDays = (from m in _db.WorkingDaysPerWeeks
                                       where m.id == Id
                                       select m).Single();
            numberTextBox.Text = updateDays.number.ToString();
            workingDaysPerWeekID = updateDays.id;
            workingDaysPerWeekSave.Visibility = Visibility.Visible;
            addWorkingDaysPerWeek.IsEnabled = false;


        }

        //update and save datails working days of week
        private void WorkingDaysPerWeekSave_Click(object sender, RoutedEventArgs e)
        {
            WorkingDaysPerWeek updateDays = (from m in _db.WorkingDaysPerWeeks
                                             where m.id == workingDaysPerWeekID
                                             select m).Single();
            updateDays.number = int.Parse(numberTextBox.Text);
            _db.SaveChanges();
            workingDaysPerWeekDataGrid.ItemsSource = _db.WorkingDaysPerWeeks.ToList();
            clean();
            workingDaysPerWeekSave.Visibility = Visibility.Hidden;
            addWorkingDaysPerWeek.IsEnabled = true;

        }

        //delete working days of week
        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            //int Id = (workingDaysPerWeekDataGrid.SelectedItem as Day).id;
            int Id = (workingDaysPerWeekDataGrid.SelectedItem as WorkingDaysPerWeek).id;
            var deletedDay = _db.WorkingDaysPerWeeks.Where(m => m.id == Id).Single();
            _db.WorkingDaysPerWeeks.Remove(deletedDay);
            _db.SaveChanges();
            workingDaysPerWeekDataGrid.ItemsSource = _db.WorkingDaysPerWeeks.ToList();

        }

        
        //Add working time per day
        private void Add_Time_Per_Day_Button(object sender, RoutedEventArgs e)
        {
            time.number = int.Parse(time_per_day.Text);
            _db.WorkingTimePerDays.Add(time);
            _db.SaveChanges();
            workingTimePerDayDataGrid.ItemsSource = _db.WorkingTimePerDays.ToList();
            clean();
        }

        //Get details of working time per day
        private void time_per_day_updateBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (workingTimePerDayDataGrid.SelectedItem as WorkingTimePerDay).id;
            WorkingTimePerDay updateTime = (from m in _db.WorkingTimePerDays
                                            where m.id == Id
                                             select m).Single();
            time_per_day.Text = updateTime.number.ToString();
            workingTimePerDayId = updateTime.id;
            saveTimePerDayButton.Visibility = Visibility.Visible;
            addWorkingTimePerDayWithLunch.IsEnabled = false;
        }

        //Delete working time per day
        private void time_per_day_deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (workingTimePerDayDataGrid.SelectedItem as WorkingTimePerDay).id;
            var deletedTime = _db.WorkingTimePerDays.Where(m => m.id == Id).Single();
            _db.WorkingTimePerDays.Remove(deletedTime);
            _db.SaveChanges();
            workingTimePerDayDataGrid.ItemsSource = _db.WorkingTimePerDays.ToList();
        }

        //update and save working time per day
        private void saveTimePerDay_Click(object sender, RoutedEventArgs e)
        {
            WorkingTimePerDay updateTime = (from m in _db.WorkingTimePerDays
                                            where m.id == workingTimePerDayId
                                            select m).Single();
            updateTime.number = int.Parse(time_per_day.Text);
            _db.SaveChanges();
            workingTimePerDayDataGrid.ItemsSource = _db.WorkingTimePerDays.ToList();
            clean();
            saveTimePerDayButton.Visibility = Visibility.Hidden;
            addWorkingTimePerDayWithLunch.IsEnabled = true;
        }


    }
}
