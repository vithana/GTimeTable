using BespokeFusion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for woekingDaysAndHoursNew.xaml
    /// </summary>
    public partial class WorkingDaysAndHoursNew : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();
        int workingDaysAndHoursId;
        List<WorkingDaysOfWeek> workingDaysList;
        public List<String> TimeSlots;
        int workingHours  ;
        string timeSlot  ;

        WorkingDaysOfWeek workingDaysOfWeek = new WorkingDaysOfWeek();
        public WorkingDaysAndHoursNew()
        {
            InitializeComponent();
            Load();

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

        public void GetValuesOfWorkingDaysAndHours() {

            using (var ctx = new GTimeTableEntities())
            {
                var daysAndHoursDetails = ctx.WorkingDaysAndHours.SqlQuery("Select * from WorkingDaysAndHours").FirstOrDefault();
                workingDaysList = ctx.WorkingDaysOfWeeks.SqlQuery("Select * from WorkingDaysOfWeek").ToList();
                workingDaysAndHoursId = daysAndHoursDetails.id;
                NoOfDaysTextBox.Text = daysAndHoursDetails.no_of_working_days.ToString();
                TimeTextBox.Text = daysAndHoursDetails.working_time_per_day.ToString();
                //slotTextBox.Text = daysAndHoursDetails.slots_for_time_table.ToString();
                string slotTime = daysAndHoursDetails.slots_for_time_table.ToString();

                if (slotTime.Equals("1hr"))
                {
                    timeTableSlotComboBox.SelectedIndex = 0;
                }
                else if (slotTime.Equals("30min"))
                {
                    timeTableSlotComboBox.SelectedIndex = 1;
                }
                else
                    timeTableSlotComboBox.SelectedIndex = -1;

            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Load()
        {
            GetValuesOfWorkingDaysAndHours();            
            workingDaysOfWeekDataGrid.ItemsSource = _db.WorkingDaysOfWeeks.ToList();


            workingHours = int.Parse(TimeTextBox.Text);
            timeSlot = timeTableSlotComboBox.SelectionBoxItem.ToString();

            calTimeSlots();
            //workingDaysOfWeekDataGrid.CanUserAddRows = false;
        }

        private void AddDayButton_Click(object sender, RoutedEventArgs e)
        {
            workingDaysOfWeek.day = workingDayComboBox.SelectionBoxItem.ToString();
            using (var ctx = new GTimeTableEntities())
            {
                workingDaysList = ctx.WorkingDaysOfWeeks.SqlQuery("Select * from WorkingDaysOfWeek").ToList();
            }
            if (int.Parse(NoOfDaysTextBox.Text) == workingDaysOfWeekDataGrid.Items.Count)
            {
                MaterialMessageBox.ShowError(@"You can not add more days ");
                return;
            }

            foreach (var days in workingDaysList)
            {
                if (days.day.Equals(workingDaysOfWeek.day))
                {
                    MaterialMessageBox.ShowError(workingDaysOfWeek.day + @" is already Added");
                    return;
                }
            }
            
            using (var ctx = new GTimeTableEntities())
            {
                var workingDays = ctx.Database.ExecuteSqlCommand("INSERT into WorkingDaysOfWeek(day,working_day_and_hours_id) values(@dayValue, 1 )", 
                                                                    new SqlParameter("@dayValue", workingDayComboBox.SelectionBoxItem.ToString()));
            }


            //_db.WorkingDaysOfWeeks.Add(workingDaysOfWeek);
            _db.SaveChanges();
            workingDaysOfWeekDataGrid.ItemsSource = _db.WorkingDaysOfWeeks.ToList();

        }

        private void dayDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (workingDaysOfWeekDataGrid.SelectedItem as WorkingDaysOfWeek).id;
            var deleteDay = _db.WorkingDaysOfWeeks.Where(m => m.id == Id).Single();
            _db.WorkingDaysOfWeeks.Remove(deleteDay);
            _db.SaveChanges();
            workingDaysOfWeekDataGrid.ItemsSource = _db.WorkingDaysOfWeeks.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //using (var ctx = new GTimeTableEntities())
            //{
            //    var workingDaysAndHours = ctx.Database.ExecuteSqlCommand("UPDATE WorkingDaysAndHours set no_of_working_days=@noOfDays where id = 1",
            //                                                        new SqlParameter("@noOfDays", NoOfDaysTextBox)//,
            //                                                        //new SqlParameter("@time", TimeTextBox.ToString()),
            //                                                        //new SqlParameter("@timeSlot", timeTableSlotComboBox.SelectionBoxItem.ToString())
            //                                                        );
            //}              
            Console.WriteLine(NoOfDaysTextBox.Text.GetType());
            {

            }
            if (int.Parse(NoOfDaysTextBox.Text) > workingDaysOfWeekDataGrid.Items.Count) {
                MaterialMessageBox.ShowError(@"Please add "+  (int.Parse(NoOfDaysTextBox.Text)- workingDaysOfWeekDataGrid.Items.Count) + " more days or change the number of days per week ");
                return;
            }
            else if(int.Parse(NoOfDaysTextBox.Text) < workingDaysOfWeekDataGrid.Items.Count)
            {
                MaterialMessageBox.ShowError(@"Please remove " + ( workingDaysOfWeekDataGrid.Items.Count - int.Parse(NoOfDaysTextBox.Text) ) + " more days or change the number of days per week");
                return;
            }
            WorkingDaysAndHour updateWorkingDaysAndHours = (from m in _db.WorkingDaysAndHours
                                                           where m.id == 1
                                                         select m).Single();
            updateWorkingDaysAndHours.no_of_working_days = int.Parse(NoOfDaysTextBox.Text);
            updateWorkingDaysAndHours.working_time_per_day = int.Parse(TimeTextBox.Text);
            updateWorkingDaysAndHours.slots_for_time_table = timeTableSlotComboBox.SelectionBoxItem.ToString();
            _db.SaveChanges();


            MaterialMessageBox.Show( @" Successfuly Updated");

            int workingHours = int.Parse(TimeTextBox.Text);

            calTimeSlots();
        }


        public void calTimeSlots()
        {
            DateTime startingDate = new DateTime(2008, 9, 22, 8, 00, 0);
            TimeSlots = new List<string>();


            if (this.timeSlot.Equals("1hr"))
            {
                for (int i = 0; i < this.workingHours; i++)
                {
                    TimeSlots.Add(startingDate.ToShortTimeString() + "-" + startingDate.AddHours(+1).ToShortTimeString());                    
                    startingDate = startingDate.AddHours(+1);
                }
            }
            else
            {
                for (int i = 0; i < this.workingHours*2; i++)
                {
                    TimeSlots.Add(startingDate.ToShortTimeString() + "-" + startingDate.AddMinutes(+30).ToShortTimeString());                   
                    startingDate = startingDate.AddMinutes(+30);
                }
            }           
        }

    }
}
