using GTimeTable.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Syncfusion.UI.Xaml.Grid.Converter;

namespace GTimeTable
{
    /// <summary>
    /// Interaction logic for AllTimeTable.xaml
    /// </summary>
    public partial class AllTimeTable : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();

        List<WorkingDaysOfWeek> workingDaysList;
        List<Session> SessionDetails;

        public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

        private WorkingDaysAndHoursNew WorkingDaysAndHours = new WorkingDaysAndHoursNew();
        private ObservableCollection<ConnectionItem> _connectionitems = new ObservableCollection<ConnectionItem>();

        public AllTimeTable()
        {
            InitializeComponent();
            AllTimeTableLoad();
        }

        private void AllTimeTableLoad()
        {
            //Loading Working days
            using (var ctx = new GTimeTableEntities())
            {
                var daysAndHoursDetails = ctx.WorkingDaysAndHours.SqlQuery("Select * from WorkingDaysAndHours").FirstOrDefault();
                workingDaysList = ctx.WorkingDaysOfWeeks.SqlQuery("Select * from WorkingDaysOfWeek").ToList();
                
            }
            

            DataGridTextColumn column = new DataGridTextColumn(); ;

            //column.Header = "";
            //column.Binding = new Binding("TimeSlot");
            //column.Width = 150;
            //AllTimeTableGrid.Columns.Add(column);

            ////Adding Working Days
            //foreach (var workingDay in workingDaysList)
            //{
            //    column = new DataGridTextColumn();
            //    column.Header = workingDay.day;
            //    column.Binding = new Binding(workingDay.day);
            //    AllTimeTableGrid.Columns.Add(column);
            //}

            

            //Loading session

            using (var ctx = new GTimeTableEntities())
            {
                SessionDetails = ctx.Sessions.SqlQuery("Select * from Sessions").ToList();

                foreach (var item in SessionDetails)
                {
                    Console.WriteLine(item.id);
                }

            }

            //Adding Time slots
            //foreach (var timeslot in WorkingDaysAndHours.TimeSlots)
            //{
            //    AllTimeTableGrid.Items.Add(new { TimeSlot = timeslot });
            //}




            //Sync Function

            //ConnectionItems.Add(new ConnectionItem { TimeSlot = "Item1", Ping = "150ms" });
            //ConnectionItems.Add(new ConnectionItem { TimeSlot = "Item2", Ping = "122ms" });

            foreach (var timeslot in WorkingDaysAndHours.TimeSlots)
            {
                ConnectionItems.Add(new ConnectionItem { TimeSlot = timeslot });
            }



        }

        public ObservableCollection<ConnectionItem> ConnectionItems
        {
            get { return _connectionitems; }
            set { _connectionitems = value; }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // to change a value jus find the item you want in the list and change it
            // because your ConnectionItem class implements INotifyPropertyChanged
            // ite will automaticly update the dataGrid

            // Example
           // ConnectionItems[2].Ping = daysAndHoursDetails[1].id.ToString();

            //for (int i = 0; i < SessionDetails.Count(); i++)
            //{
                
            //    if(SessionDetails[i].id.ToString() != "")
            //    {
            //        ConnectionItems[i].Monday = SessionDetails[i].id.ToString();

            //    }
            //}

            int nofSession = SessionDetails.Count();
            int currentSession = 0;

            for (int i = 0; i < WorkingDaysAndHours.TimeSlots.Count(); i++)
            {
                if( i < nofSession)
                {
                    int id = SessionDetails[i].id;

                    string message = formatSessionText(id);

                    ConnectionItems[i].Monday = message;
                    currentSession = ++currentSession;
                }
            }

            Console.WriteLine(currentSession);

            nofSession = nofSession - currentSession;

            //Tuesday
            if(nofSession > 0)
            {
                for (int i = 0 ; i < WorkingDaysAndHours.TimeSlots.Count(); i++)
                {
                    if (i + currentSession < SessionDetails.Count())
                    {
                        int id = SessionDetails[i + currentSession].id;

                        string message = formatSessionText(id);

                        ConnectionItems[i].Tuesday = message;
                        currentSession = ++currentSession;
                    }
                }
            }

            nofSession = nofSession - currentSession;

            //Wednesday
            if (nofSession > 0)
            {
                for (int i = 0; i < WorkingDaysAndHours.TimeSlots.Count(); i++)
                {
                    if (i + currentSession < SessionDetails.Count())
                    {
                        int id = SessionDetails[i + currentSession].id;

                        string message = formatSessionText(id);

                        ConnectionItems[i].Wednesday = message;
                        currentSession = ++currentSession;
                    }
                }
            }

            //foreach (var item in ConnectionItems)
            //{
            //    item.Ping = "HEllooo";
            //}

        }

        private string formatSessionText(int id) {
            List<SessionDto> SessionDTO = new List<SessionDto>();


            List<LecturesSession> lecturerSessionList = new List<LecturesSession>();

            using (var ctx = new GTimeTableEntities())
            {
                string sql1 = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                       "from Sessions S " +
                                                       "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                       "INNER JOIN Tags T ON S.tag = T.id " +
                                                       "INNER JOIN Student ST ON S.student = ST.id " +
                                                       "where S.id = " + id;

                SessionDTO = ctx.Database.SqlQuery<SessionDto>(sql1).ToList();

                string sql2 = "Select LS.* " +
                                "from Sessions S " +
                                "INNER JOIN LecturesSessions LS ON LS.session = S.id " +
                                "INNER JOIN Lecturers L ON LS.lecture = L.id " +
                                "WHERE LS.session = " + id;

                lecturerSessionList = ctx.Database.SqlQuery<LecturesSession>(sql2).ToList();
            }

            List<Lecturer> lecturers = new List<Lecturer>();
            Lecturer newLec = new Lecturer();

            foreach (var item in lecturerSessionList)
            {
                newLec = (from m in _db.Lecturers
                          where m.id == item.lecture
                          select m).Single();

                lecturers.Add(newLec);
            }
            string message = "";

            for (int j = 0; j < lecturers.Count(); j++)
            {
                if (j == (lecturers.Count() - 1))
                {
                    message += lecturers[j].name + "\n";
                }
                else
                {
                    message += lecturers[j].name + ", ";
                }
            }

            message += SessionDTO[0].subject_name + "(" + SessionDTO[0].subject_code + ")\n" +
                       SessionDTO[0].tag + "\n" + SessionDTO[0].groupId;

            return message;
        }

        private void download_pdf_Click(object sender, RoutedEventArgs e)
        {
            var document = dataGridView.ExportToPdf();

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            document.Save(folder + "/timetable.pdf");
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int value = TypeComboBox.SelectedIndex;
            Console.WriteLine(value);

            selectComboBox.Items.Clear();

            switch (value)
            {
                case 0:
                    var values = _db.Lecturers.ToList();
                    foreach (var lec in values)
                    {
                        selectComboBox.Items.Add(lec.name);
                    }
                    break;
                case 1:
                    var rooms = _db.Rooms.ToList();
                    foreach (var room in rooms)
                    {
                        selectComboBox.Items.Add(room.roomId);
                    }
                    break;
                case 2:
                    var students = _db.Students.ToList();
                    foreach (var st in students)
                    {
                        selectComboBox.Items.Add(st.groupId);
                    }
                    break;
                default:
                    break;
            }

        }
    }
    public class ConnectionItem : INotifyPropertyChanged
    {
        private string _timeSlot;

        
        private string _monday;
        private string _tuesday;
        private string _wednesday;
        private string _thursday;
        private string _friday;



        public string TimeSlot
        {
            get { return _timeSlot; }
            set { _timeSlot = value; NotifyPropertyChanged("TimeSlot"); }
        }

        public string Monday
        {
            get { return _monday; }
            set { _monday = value; NotifyPropertyChanged("Monday"); }
        }

        public string Tuesday
        {
            get { return _tuesday; }
            set { _tuesday = value; NotifyPropertyChanged("Tuesday"); }
        }

        public string Wednesday
        {
            get { return _wednesday; }
            set { _wednesday = value; NotifyPropertyChanged("Wednesday"); }
        }
        public string Thursday
        {
            get { return _thursday; }
            set { _thursday = value; NotifyPropertyChanged("Thursday"); }
        }
        public string Friday
        {
            get { return _friday; }
            set { _friday = value; NotifyPropertyChanged("Friday"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="property">The info.</param>
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
