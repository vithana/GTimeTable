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

        NotAvailbleTimesOfGroup NotAvailbleTimesOfGroup;
        ParrelalSession ParrelalSession;

        private WorkingDaysAndHoursNew WorkingDaysAndHours = new WorkingDaysAndHoursNew();

        public Managment()
        {
            InitializeComponent();
            NotAvailbleSessionsLoad();
            notAvailableGroupLoad();
            NotAvilableLecturerLoad();
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

        private void clearMain()
        {
            cleanLectrure();
            clean_NotavilabeSession();
        }

        //=========================================== Not Availabe Times Add Function =========================================================

        //------------------ Tab Load Function
        private void TBNotavailabeTime(object sender, MouseButtonEventArgs e)
        {
            clearMain();
            NotAvailbleSessionsLoad();
            notAvailableGroupLoad();
            NotAvilableLecturerLoad();
        }

        //--------------- Load Not Availabe Time For Lectures ----------------------------------------------
        private void NotAvilableLecturerLoad()
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

        //--------------- Load Not Availabe Time For Sessions ----------------------------------------------
        private void NotAvailbleSessionsLoad()
        {
            List<SessionDto> sessionDto = new List<SessionDto>();
            using (var ctx = new GTimeTableEntities())
            {
                string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                                "from Sessions S " +
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
            }

            foreach (var session in sessionDto)
            {
                sessionComboBox.Items.Add(session.subject_code + "-" + session.tag + "-" + session.groupId);
            }

            foreach (var timeSlot in WorkingDaysAndHours.TimeSlots)
            {
                sessionTimeSlotComboBox.Items.Add(timeSlot);
            }

            List<NotAvailableTimeOfSessionDto> notAvailableTimeOfSessionDtos = new List<NotAvailableTimeOfSessionDto>();

            using (var ctx = new GTimeTableEntities())
            {
                notAvailbeleTimesOfSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvailableTimeOfSessionDto>("Select N.id as id ,S.id as session , N.timeSlot as timeSlot from NotAvailbeleTimesOfSessions N INNER JOIN Sessions S ON N.[session] = S.id").ToList();
            }

        }

        //-------------------- Load Not Availabe Times For Group ------------------------------------------------------
        private void notAvailableGroupLoad()
        {
            List<Student> students = new List<Student>();
            students = _db.Students.ToList();
            foreach (var timeSlot in WorkingDaysAndHours.TimeSlots)
            {
                groupTimeSlotComboBox.Items.Add(timeSlot);
            }
            foreach (var student in students)
            {
                groupComboBox.Items.Add(student.groupId);
            }

            using (var ctx = new GTimeTableEntities())
            {
                notAvailbleTimesOfGroupDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvailbleTimeOfGroupDto>("select N.id  as id, (select s.groupId from Student s where s.id = N.student) as student , N.timeSlot as timeSlot from NotAvailbleTimesOfGroup N").ToList();
            }

          
        }

        //================================ Not Availabe Times in Lectrers Functions ===============================
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
            if (timeSlotComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Time Slot Befor Save");
            }
            if (lectrureCombobox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Lecturer Befor Save");
            }
            else
            {
                try
                {
                    Lecturer lecturer = (from m in _db.Lecturers
                                         where m.name.Equals(lectrureCombobox.Text)
                                         select m).Single();

                    NotAvailbleTimesOfLecturer notAvailbleTimesOfLecturer = new NotAvailbleTimesOfLecturer();

                    notAvailbleTimesOfLecturer.lecturer = lecturer.id;
                    notAvailbleTimesOfLecturer.timeSlot = timeSlotComboBox.Text;

                    _db.NotAvailbleTimesOfLecturers.Add(notAvailbleTimesOfLecturer);
                    _db.SaveChanges();
                    
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }
                cleanLectrure();
                NotAvilableLecturerLoad();
            }
        }

        private void cleanLectrure()
        {
            lectrureCombobox.SelectedIndex = -1;
            timeSlotComboBox.SelectedIndex = -1;
            lectrureCombobox.Items.Clear();
            timeSlotComboBox.Items.Clear();
        }
        //===========================================================================================================


        //======================================== Start of not availble time of session  =====================================================   

        private void Not_Avilable_TIme_Session_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sessionComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Subject Befor Save");
            }
            else if (sessionTimeSlotComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Time Slot Befor Save");
            }
            else
            {
                try
                {
                    List<string> values = new List<string>(sessionComboBox.Text.Split('-'));

                    string subjectCodeValue = values[0].ToString();
                    string TagValue = values[1].ToString();
                    string groupIdValue = values[2].ToString();

                    Subject subject = (from s in _db.Subjects
                                       where s.code.Equals(subjectCodeValue)
                                       select s).Single();

                    Tag tag = (from m in _db.Tags
                               where m.tag1.Equals(TagValue)
                               select m).Single();

                    Student student = (from m in _db.Students
                                       where m.groupId.Equals(groupIdValue)
                                       select m).Single();

                    Session session = (from m in _db.Sessions
                                       where (m.subject == subject.id && m.tag == tag.id && m.student == student.id)
                                       select m).Single();



                    NotAvailbeleTimesOfSession notAvailbeleTimesOfSession = new NotAvailbeleTimesOfSession();

                    notAvailbeleTimesOfSession.session = session.id;
                    notAvailbeleTimesOfSession.timeSlot = sessionTimeSlotComboBox.Text;

                    _db.NotAvailbeleTimesOfSessions.Add(notAvailbeleTimesOfSession);
                    _db.SaveChanges();
                   
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }

                clean_NotavilabeSession();
                NotAvailbleSessionsLoad();
            }
        }

        private void not_avilable_session_deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (notAvailbeleTimesOfSessionDataGrid.SelectedItem as NotAvailableTimeOfSessionDto).id;

            var deletedItem = _db.NotAvailbeleTimesOfSessions.Where(m => m.id == Id).Single();
            _db.NotAvailbeleTimesOfSessions.Remove(deletedItem);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                notAvailbeleTimesOfSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvailableTimeOfSessionDto>("Select N.id as id ,S.id as session , N.timeSlot as timeSlot from NotAvailbeleTimesOfSessions N INNER JOIN Sessions S ON N.[session] = S.id").ToList();
            }
        }

        private void clean_NotavilabeSession()
        {
            sessionComboBox.Text = "";
            sessionTimeSlotComboBox.Text = "";
            sessionComboBox.Items.Clear();
            sessionTimeSlotComboBox.Items.Clear();

        }
        // ========================================= End of not availble time of session =================================================


        // ==================== Start of not available time of groups and sub groups  ===================================================      

        private void not_avilable_group_addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (groupComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Group Befor Save");
            }
            else if (groupTimeSlotComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Room Befor Save");
            }
            else
            {
                try
                {
                    NotAvailbleTimesOfGroup = new NotAvailbleTimesOfGroup();

                    Student student = (from m in _db.Students
                                       where m.groupId.Equals(groupComboBox.Text)
                                       select m).Single();


                    NotAvailbleTimesOfGroup.student = student.id;
                    NotAvailbleTimesOfGroup.timeSlot = groupTimeSlotComboBox.Text;
                    _db.NotAvailbleTimesOfGroups.Add(NotAvailbleTimesOfGroup);
                    _db.SaveChanges();
                    
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }
                clear_NotAailabegrup();
                notAvailableGroupLoad();

            }

        }

        private void not_avilable_group_deleteClick(object sender, RoutedEventArgs e)
        {
            int id = (notAvailbleTimesOfGroupDataGrid.SelectedItem as NotAvailbleTimeOfGroupDto).id;
            var deletedRow = _db.NotAvailbleTimesOfGroups.Where(r => r.id == id).Single();
            _db.NotAvailbleTimesOfGroups.Remove(deletedRow);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                notAvailbleTimesOfGroupDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvailbleTimeOfGroupDto>("select N.id  as id, (select s.groupId from Student s where s.id = N.student) as student , N.timeSlot as timeSlot from NotAvailbleTimesOfGroup N").ToList();
            }

        }

        private void clear_NotAailabegrup()
        {
            groupComboBox.Text = "";
            groupTimeSlotComboBox.Text = "";
            groupComboBox.Items.Clear();
            groupTimeSlotComboBox.Items.Clear();

        }

        //=================================================================================================================================


        //========================================================== Start of Concecutive Session ================================================================================
        private void concecutiveSessionTabLoad(object sender, MouseButtonEventArgs e)
        {
            List<SessionDto> sessionDto = new List<SessionDto>();

            using (var ctx = new GTimeTableEntities())
            {
                string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                "from Sessions S " +
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
            }

            List<ConcevutiveSessionDto> concevutiveSessionDto = new List<ConcevutiveSessionDto>();

            List<SessionDto> sessionDto2 = new List<SessionDto>();

            foreach (var session in sessionDto)
            {
                concecutiveSession1Combobox.Items.Add(session.subject_code + "-" + session.tag + "-" + session.groupId);

                if (session.consecutive_session_id != null)
                {
                    string session1 = session.subject_code + "-" + session.tag + "-" + session.groupId;

                    using (var ctx = new GTimeTableEntities())
                    {
                        string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                        "from Sessions S " +
                                                                        "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                        "INNER JOIN Tags T ON S.tag = T.id " +
                                                                        "INNER JOIN Student ST ON S.student = ST.id " +
                                                                        "where S.id =" + session.consecutive_session_id;

                        sessionDto2 = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                    }

                    string session2 = sessionDto2[0].subject_code + "-" + sessionDto2[0].tag + "-" + sessionDto2[0].groupId;

                    ConcevutiveSessionDto cons_session_dto = new ConcevutiveSessionDto();
                    cons_session_dto.concecutiveSession1 = session1;
                    cons_session_dto.concecutiveSession2 = session2;

                    concevutiveSessionDto.Add(cons_session_dto);
                }
            }

            concecutiveSessionDataGrid.ItemsSource = concevutiveSessionDto;

        }
        //---------------------------------- Combo Box Onchange --------------------------------------------------
        private void concecutiveSession1Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (concecutiveSession1Combobox.SelectedItem != null)
            {
                concecutiveSession2ComboBox.Items.Clear();

                string session_name = concecutiveSession1Combobox.SelectedItem.ToString();

                List<string> values = new List<string>(session_name.Split('-'));

                string subjectCodeValue = values[0].ToString();
                string TagValue = values[1].ToString();
                string groupIdValue = values[2].ToString();

                Subject subject = (from s in _db.Subjects
                                   where s.code.Equals(subjectCodeValue)
                                   select s).Single();

                Tag tag = (from m in _db.Tags
                           where m.tag1.Equals(TagValue)
                           select m).Single();

                Student student = (from m in _db.Students
                                   where m.groupId.Equals(groupIdValue)
                                   select m).Single();

                Session selected_session = (from m in _db.Sessions
                                            where (m.subject == subject.id && m.tag == tag.id && m.student == student.id)
                                            select m).Single();

                List<SessionDto> sessionDto = new List<SessionDto>();

                using (var ctx = new GTimeTableEntities())
                {
                    string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                    "from Sessions S " +
                                                                    " INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                    "INNER JOIN Tags T ON S.tag = T.id " +
                                                                    "INNER JOIN Student ST ON S.student = ST.id " +
                                                                    "where S.subject = " + subject.id +
                                                                    " and S.student = " + student.id;

                    sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                }

                foreach (var session in sessionDto)
                {
                    if (selected_session.id != session.id)
                    {
                        concecutiveSession2ComboBox.Items.Add(session.subject_code + "-" + session.tag + "-" + session.groupId);
                    }
                }

            }
        }

        //------------------------------------- Add Function ---------------------------------------------------------
        private void concecutiveSession_Add_Button_Click(object sender, RoutedEventArgs e)
        {

            if (concecutiveSession1Combobox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Session Befor Save");
            }
            else if (concecutiveSession2ComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Consective Session Befor Save");
            }
            else
            {
                try
                {
                    string session_name1 = concecutiveSession1Combobox.SelectedItem.ToString();

                    List<string> values1 = new List<string>(session_name1.Split('-'));

                    string subjectCodeValue1 = values1[0].ToString();
                    string TagValue1 = values1[1].ToString();
                    string groupIdValue1 = values1[2].ToString();

                    Subject subject1 = (from s in _db.Subjects
                                        where s.code.Equals(subjectCodeValue1)
                                        select s).Single();

                    Tag tag1 = (from m in _db.Tags
                                where m.tag1.Equals(TagValue1)
                                select m).Single();

                    Student student1 = (from m in _db.Students
                                        where m.groupId.Equals(groupIdValue1)
                                        select m).Single();

                    Session selected_session1 = (from m in _db.Sessions
                                                 where (m.subject == subject1.id && m.tag == tag1.id && m.student == student1.id)
                                                 select m).Single();

                    string session_name2 = concecutiveSession2ComboBox.SelectedItem.ToString();

                    List<string> values2 = new List<string>(session_name2.Split('-'));

                    string subjectCodeValue2 = values2[0].ToString();
                    string TagValue2 = values2[1].ToString();
                    string groupIdValue2 = values2[2].ToString();

                    Subject subject2 = (from s in _db.Subjects
                                        where s.code.Equals(subjectCodeValue2)
                                        select s).Single();

                    Tag tag2 = (from m in _db.Tags
                                where m.tag1.Equals(TagValue2)
                                select m).Single();

                    Student student2 = (from m in _db.Students
                                        where m.groupId.Equals(groupIdValue2)
                                        select m).Single();

                    Session selected_session2 = (from m in _db.Sessions
                                                 where (m.subject == subject2.id && m.tag == tag2.id && m.student == student2.id)
                                                 select m).Single();

                    selected_session1.consecutive_session_id = selected_session2.id;

                    _db.SaveChanges();

                    List<SessionDto> sessionDto = new List<SessionDto>();

                    using (var ctx = new GTimeTableEntities())
                    {
                        string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                        "from Sessions S " +
                                                                        "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                        "INNER JOIN Tags T ON S.tag = T.id " +
                                                                        "INNER JOIN Student ST ON S.student = ST.id ";

                        sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                    }

                    List<ConcevutiveSessionDto> concevutiveSessionDto = new List<ConcevutiveSessionDto>();

                    List<SessionDto> sessionDto2 = new List<SessionDto>();

                    foreach (var session in sessionDto)
                    {
                        if (session.consecutive_session_id != null)
                        {
                            string session1 = session.subject_code + "-" + session.tag + "-" + session.groupId;

                            using (var ctx = new GTimeTableEntities())
                            {
                                string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                                "from Sessions S " +
                                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                                "INNER JOIN Student ST ON S.student = ST.id " +
                                                                                "where S.id =" + session.consecutive_session_id;

                                sessionDto2 = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                            }

                            string session2 = sessionDto2[0].subject_code + "-" + sessionDto2[0].tag + "-" + sessionDto2[0].groupId;

                            ConcevutiveSessionDto cons_session_dto = new ConcevutiveSessionDto();
                            cons_session_dto.concecutiveSession1 = session1;
                            cons_session_dto.concecutiveSession2 = session2;

                            concevutiveSessionDto.Add(cons_session_dto);
                        }
                    }

                    concecutiveSessionDataGrid.ItemsSource = concevutiveSessionDto;
                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }
                cleanConcecutiveSession();
            }
        }

        //--------------------------------- Deleet Function -----------------------------------------------------------------------------
        private void concecutiveSession_Delete_Button_Click(object sender, RoutedEventArgs e)
        {

            string session_name1 = (concecutiveSessionDataGrid.SelectedItem as ConcevutiveSessionDto).concecutiveSession1;

            List<string> values1 = new List<string>(session_name1.Split('-'));

            string subjectCodeValue1 = values1[0].ToString();
            string TagValue1 = values1[1].ToString();
            string groupIdValue1 = values1[2].ToString();

            Subject subject1 = (from s in _db.Subjects
                                where s.code.Equals(subjectCodeValue1)
                                select s).Single();

            Tag tag1 = (from m in _db.Tags
                        where m.tag1.Equals(TagValue1)
                        select m).Single();

            Student student1 = (from m in _db.Students
                                where m.groupId.Equals(groupIdValue1)
                                select m).Single();

            Session selected_session1 = (from m in _db.Sessions
                                         where (m.subject == subject1.id && m.tag == tag1.id && m.student == student1.id)
                                         select m).Single();

            selected_session1.consecutive_session_id = null;

            _db.SaveChanges();

            List<SessionDto> sessionDto = new List<SessionDto>();

            using (var ctx = new GTimeTableEntities())
            {
                string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                "from Sessions S " +
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
            }

            List<ConcevutiveSessionDto> concevutiveSessionDto = new List<ConcevutiveSessionDto>();

            List<SessionDto> sessionDto2 = new List<SessionDto>();

            foreach (var session in sessionDto)
            {
                if (session.consecutive_session_id != null)
                {
                    string session1 = session.subject_code + "-" + session.tag + "-" + session.groupId;

                    using (var ctx = new GTimeTableEntities())
                    {
                        string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                        "from Sessions S " +
                                                                        "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                        "INNER JOIN Tags T ON S.tag = T.id " +
                                                                        "INNER JOIN Student ST ON S.student = ST.id " +
                                                                        "where S.id =" + session.consecutive_session_id;

                        sessionDto2 = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                    }

                    string session2 = sessionDto2[0].subject_code + "-" + sessionDto2[0].tag + "-" + sessionDto2[0].groupId;

                    ConcevutiveSessionDto cons_session_dto = new ConcevutiveSessionDto();
                    cons_session_dto.concecutiveSession1 = session1;
                    cons_session_dto.concecutiveSession2 = session2;

                    concevutiveSessionDto.Add(cons_session_dto);
                }
            }

            concecutiveSessionDataGrid.ItemsSource = concevutiveSessionDto;

            cleanConcecutiveSession();

        }

        //------------------------------ Clean Function ---------------------------------------------------------
        private void cleanConcecutiveSession()
        {
            concecutiveSession1Combobox.SelectedIndex = -1;
            concecutiveSession2ComboBox.SelectedIndex = -1;
            concecutiveSession2ComboBox.Items.Clear();
        }
        //=================================================================================================================================================



        //================================ Add Paralel Session ===============================================================
        private void TBClickParallelSession(object sender, MouseButtonEventArgs e)
        {
            clear_Parrelal_Session();
            Load_Parrellal_session();           

        }

        private void Load_Parrellal_session()
        {
            parrelalSessionDataGrid.ItemsSource = _db.ParrelalSessions.ToList();

            List<Session> LstSession_ParallelSession = new List<Session>();
            LstSession_ParallelSession = _db.Sessions.ToList();

            foreach (var session in LstSession_ParallelSession)
            {
                if (session != null)
                {
                    session_ParallelSession.Items.Add(session.id);
                }
            }

            List<Session> LstParrella_Session_ParallelSession = new List<Session>();
            LstParrella_Session_ParallelSession = _db.Sessions.ToList();

            foreach (var parellal in LstParrella_Session_ParallelSession)
            {
                if (parellal != null)
                {
                    parrelSession_ParallelSession.Items.Add(parellal.id);
                }

            }
        }

        private void clear_Parrelal_Session()
        {
            session_ParallelSession.Text = "";
            parrelSession_ParallelSession.Text = "";
            session_ParallelSession.Items.Clear();
            parrelSession_ParallelSession.Items.Clear();

        }
        //--------------------- Add Click ------------------------------------------
        private void AddBtnclik_ParrelalSession(object sender, RoutedEventArgs e)
        {
            if (session_ParallelSession.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Session Befor Save");
            }
            else if (parrelSession_ParallelSession.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Parrelal Session Befor Save");
            }
            else
            {
                try
                {
                    ParrelalSession = new ParrelalSession();


                    ParrelalSession.session = Convert.ToInt32(session_ParallelSession.Text);
                    ParrelalSession.parellal_session = Convert.ToInt32(parrelSession_ParallelSession.Text);
                    _db.ParrelalSessions.Add(ParrelalSession);
                    _db.SaveChanges();

                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }
                clear_Parrelal_Session();
                Load_Parrellal_session();

            }

        }

        private void delete_parrrealSection(object sender, RoutedEventArgs e)
        {
            int id = (parrelalSessionDataGrid.SelectedItem as ParrelalSession).id;
            var deletedRow = _db.ParrelalSessions.Where(r => r.id == id).Single();
            _db.ParrelalSessions.Remove(deletedRow);
            _db.SaveChanges();
            parrelalSessionDataGrid.ItemsSource = _db.ParrelalSessions.ToList();
        }

        private void btnclickViewSession_ParallelSession(object sender, RoutedEventArgs e)
        {
            if (session_ParallelSession.Text == "")
            {
                MaterialMessageBox.ShowError(@"No selected Session");
            }
            else
            {
                int id = Convert.ToInt32(session_ParallelSession.Text);
                viewSessionCommon_ParallelSession(id);
            }
        }

        private void btnclickViewParrelalSession_ParallelSession(object sender, RoutedEventArgs e)
        {
            if (parrelSession_ParallelSession.Text == "")
            {
                MaterialMessageBox.ShowError(@"No selected Parrelal Session");
            }
            else
            {
                int id = Convert.ToInt32(session_ParallelSession.Text);
                viewSessionCommon_ParallelSession(id);
            }
            

        }

        private void viewSessionGrid_parrrealSection(object sender, RoutedEventArgs e)
        {
            int id = (parrelalSessionDataGrid.SelectedItem as ParrelalSession).session;
            viewSessionCommon_ParallelSession(id);
        }

        private void ViewParellasession_Grid_parrrealSection(object sender, RoutedEventArgs e)
        {
            int id = (parrelalSessionDataGrid.SelectedItem as ParrelalSession).parellal_session;
            viewSessionCommon_ParallelSession(id);
        }

        private void viewSessionCommon_ParallelSession(int id)
        {
            try
            {
                List<LecturesSession> lecturerSessionList = new List<LecturesSession>();

                using (var ctx = new GTimeTableEntities())
                {
                    string sql = "Select LS.* " +
                                    "from Sessions S " +
                                    "INNER JOIN LecturesSessions LS ON LS.session = S.id " +
                                    "INNER JOIN Lecturers L ON LS.lecture = L.id " +
                                    "WHERE LS.session = " + id;

                    lecturerSessionList = ctx.Database.SqlQuery<LecturesSession>(sql).ToList();
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
                string message = "Session ID " + id + "\n\n\t";

                for (int i = 0; i < lecturers.Count(); i++)
                {
                    if (i == (lecturers.Count() - 1))
                    {
                        message += lecturers[i].name + "\n";
                    }
                    else
                    {
                        message += lecturers[i].name + ", ";
                    }
                }
                List<SessionDto> sessionDto = new List<SessionDto>();
                using (var ctx = new GTimeTableEntities())
                {
                    string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                                    "from Sessions S " +
                                                                    "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                    "INNER JOIN Tags T ON S.tag = T.id " +
                                                                    "INNER JOIN Student ST ON S.student = ST.id WHERE S.id ='" + id + "'";

                    sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                }

                message += "\t" + sessionDto[0].subject_name + "(" + sessionDto[0].subject_code + ")\n\t" +
                           sessionDto[0].tag + "\n\t" + sessionDto[0].groupId + "\n\t" + sessionDto[0].count + "(" + sessionDto[0].duration + ")";

                MaterialMessageBox.Show(message);
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
        }

       

        //======================================================================================================================
    }
}
