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

        int workingDaysId;
        WorkingDay workingDay = new WorkingDay();

        int timeSlotId;
        TimeTableSlot timeSlot = new TimeTableSlot();


        public WorkingDaysAndHours()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            workingDaysPerWeekDataGrid.ItemsSource = _db.WorkingDaysPerWeeks.ToList();
            workingTimePerDayDataGrid.ItemsSource = _db.WorkingTimePerDays.ToList();
            workingDayDataGrid.ItemsSource = _db.WorkingDays.ToList();
            timeTableSlotDataGrid.ItemsSource = _db.TimeTableSlots.ToList();
            
            workingDaysPerWeekSave.Visibility = Visibility.Hidden;
            saveTimePerDayButton.Visibility = Visibility.Hidden;
            saveWorkingDays_btn.Visibility = Visibility.Hidden;
            saveSlotsForTimeTable_btn.Visibility = Visibility.Hidden;

        }
        private void clean()
        {
            numberTextBox.Text = "";
            time_per_day.Text = "";
            dayComboBox.Text = "";
            timeSlotComboBox.Text = "";
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

        //Add working Days
        private void AddWorkingDays_btn_Click(object sender, RoutedEventArgs e)
        {
            workingDay.day = dayComboBox.Text;
            _db.WorkingDays.Add(workingDay);
            _db.SaveChanges();
            workingDayDataGrid.ItemsSource = _db.WorkingDays.ToList();
            clean();
        }

        private void SaveWorkingDays_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        //update days 
        private void dayUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (workingDayDataGrid.SelectedItem as WorkingDay).id;
            WorkingDay updateDay = (from m in _db.WorkingDays
                                            where m.id == Id
                                            select m).Single();
            dayComboBox.SelectedItem = updateDay.ToString();
            workingDaysId = updateDay.id;
            saveWorkingDays_btn.Visibility = Visibility.Visible;
            addWorkingDays_btn.IsEnabled = false;
        }

        //Delete working days
        private void dayDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (workingDayDataGrid.SelectedItem as WorkingDay).id;
            var deletedDay = _db.WorkingDays.Where(m => m.id == Id).Single();
            _db.WorkingDays.Remove(deletedDay);
            _db.SaveChanges();
            workingDayDataGrid.ItemsSource = _db.WorkingDays.ToList();
        }

        //Add time slots for time table
        private void AddSlotsForTimeTable_btn_Click(object sender, RoutedEventArgs e)
        {
            timeSlot.time = timeSlotComboBox.Text;            
            _db.TimeTableSlots.Add(timeSlot);
            _db.SaveChanges();
            timeTableSlotDataGrid.ItemsSource = _db.TimeTableSlots.ToList();
            clean();
        }

        //Get time slot
        private void timeSlotUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (timeTableSlotDataGrid.SelectedItem as TimeTableSlot).id;
            TimeTableSlot updateTimeSlot = (from m in _db.TimeTableSlots
                                    where m.id == Id
                                    select m).Single();
            dayComboBox.SelectedValue = updateTimeSlot.ToString();
            timeSlotId = updateTimeSlot.id;
            saveSlotsForTimeTable_btn.Visibility = Visibility.Visible;
            addSlotsForTimeTable_btn.IsEnabled = false;
        }

        //Delete time slots
        private void timeSlotDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (timeTableSlotDataGrid.SelectedItem as TimeTableSlot).id;
            var deletedTimeSlot = _db.TimeTableSlots.Where(m => m.id == Id).Single();
            _db.TimeTableSlots.Remove(deletedTimeSlot);
            _db.SaveChanges();
            timeTableSlotDataGrid.ItemsSource = _db.TimeTableSlots.ToList();
        }
    }
}
