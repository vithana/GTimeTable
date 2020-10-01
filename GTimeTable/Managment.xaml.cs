using BespokeFusion;
using GTimeTable.Dtos;
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
    /// Interaction logic for Managment.xaml
    /// </summary>
    public partial class Managment : UserControl
    {

        GTimeTableEntities _db = new GTimeTableEntities();

        private WorkingDaysAndHoursNew WorkingDaysAndHours = new WorkingDaysAndHoursNew();

        public Managment()
        {
            InitializeComponent();
            NotAvilableTimesLoad();
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

        private void NotAvilableTimesLoad()
        {
            List<Lecturer> lecturers = new List<Lecturer>();
            lecturers = _db.Lecturers.ToList();

            foreach (var lectrure in lecturers)
            {
                lectrureCombobox.Items.Add(lectrure.name);
            }

            foreach (var timeSlot in WorkingDaysAndHours.TimeSlots)
            {
                timeSlotComboBox.Items.Add(timeSlot);
            }

            List<NotAvaibleTimeOfLectrureDto> notAvaibleTimeOfLectrureDtos = new List<NotAvaibleTimeOfLectrureDto>();

            using (var ctx = new GTimeTableEntities())
            {
                notAvaibleTimeOfLectrureDtos = ctx.Database.SqlQuery<NotAvaibleTimeOfLectrureDto>("Select NL.id ,L.name as lecturer, NL.timeSlot  " +
                                                                "from NotAvailbleTimesOfLecturers NL INNER JOIN Lecturers L ON NL.[lecturer] = L.id   ").ToList();
            }

            notAvailbleTimesOfLecturerDataGrid.ItemsSource = notAvaibleTimeOfLectrureDtos;

        }

        private void not_avilable_time_lectrure_deleteBtn_Click(object sender, RoutedEventArgs e)
        {

            int Id = (notAvailbleTimesOfLecturerDataGrid.SelectedItem as NotAvaibleTimeOfLectrureDto).id;

            var deletedItem = _db.NotAvailbleTimesOfLecturers.Where(m => m.id == Id).Single();
            _db.NotAvailbleTimesOfLecturers.Remove(deletedItem);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                notAvailbleTimesOfLecturerDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvaibleTimeOfLectrureDto>("Select  NL.id , L.name as lecturer, NL.timeSlot  " +
                                                                "from NotAvailbleTimesOfLecturers NL INNER JOIN Lecturers L ON NL.[lecturer] = L.id   ").ToList();
            }            
        }

        private void Not_Avilable_TIme_Add_Button_Click(object sender, RoutedEventArgs e)
        {
           
            if(timeSlotComboBox.Text == "" || lectrureCombobox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
            {
                Lecturer lecturer = (from m in _db.Lecturers
                                     where m.name.Equals(lectrureCombobox.Text)
                                     select m).Single();

                NotAvailbleTimesOfLecturer notAvailbleTimesOfLecturer = new NotAvailbleTimesOfLecturer();

                notAvailbleTimesOfLecturer.lecturer = lecturer.id;
                notAvailbleTimesOfLecturer.timeSlot = timeSlotComboBox.Text;

                _db.NotAvailbleTimesOfLecturers.Add(notAvailbleTimesOfLecturer);
                _db.SaveChanges();

                using (var ctx = new GTimeTableEntities())
                {
                    notAvailbleTimesOfLecturerDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvaibleTimeOfLectrureDto>("Select  NL.id , L.name as lecturer, NL.timeSlot  " +
                                                                    "from NotAvailbleTimesOfLecturers NL INNER JOIN Lecturers L ON NL.[lecturer] = L.id   ").ToList();
                }
                cleanLectrure();
            }
            

           
        }

        private void cleanLectrure()
        {
            lectrureCombobox.SelectedIndex = -1;
            timeSlotComboBox.SelectedIndex = -1;
        }



    }
}
