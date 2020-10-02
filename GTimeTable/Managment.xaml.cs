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
            NotAvailbleSessionsLoad();
            notAvailableGroupLoad();
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


        //Start of not availble time of session
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
                sessionComboBox.Items.Add(session.subject_code + "-" + session.tag + "-" +  session.groupId);
            }

            foreach (var timeSlot in WorkingDaysAndHours.TimeSlots)
            {
                sessionTimeSlotComboBox.Items.Add(timeSlot);
            }

            List<NotAvailableTimeOfSessionDto> notAvailableTimeOfSessionDtos = new List<NotAvailableTimeOfSessionDto>();

            using (var ctx = new GTimeTableEntities())
            {
                notAvailbeleTimesOfSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvailableTimeOfSessionDto>("Select N.id as id ,S.id as session , N.timeSlot as timeSlot from NotAvailbeleTimesOfSessions N INNER JOIN Sessions S ON N.[session] = S.id" ).ToList();
            }

            


        }

        private void Not_Avilable_TIme_Session_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sessionComboBox.Text == "" || sessionTimeSlotComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
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
                                   where (m.subject == subject.id && m.tag == tag.id && m.student == student.id )
                                   select m).Single();



                NotAvailbeleTimesOfSession notAvailbeleTimesOfSession = new NotAvailbeleTimesOfSession();

                notAvailbeleTimesOfSession.session = session.id;
                notAvailbeleTimesOfSession.timeSlot = sessionTimeSlotComboBox.Text;

                _db.NotAvailbeleTimesOfSessions.Add(notAvailbeleTimesOfSession);
                _db.SaveChanges();

                using (var ctx = new GTimeTableEntities())
                {
                    notAvailbeleTimesOfSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<NotAvailableTimeOfSessionDto>("Select N.id as id ,S.id as session , N.timeSlot as timeSlot from NotAvailbeleTimesOfSessions N INNER JOIN Sessions S ON N.[session] = S.id").ToList();
                }


                //Lecturer lecturer = (from m in _db.Sessions
                //                     where m..Equals(sessionComboBox.Text)
                //                     select m).Single();
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
        //End of not availble time of session


        //Start of not available time of groups and sub groups

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
                groupComboBox.Items.Add(student.subGropId);
            }

            //List<NotAvailbleTimeOfGroupDto> notAvailbleTimeOfGroupDtos = new List<NotAvailbleTimeOfGroupDto>();

            //using (var ctx = new GTimeTableEntities())
            //{
            //    notAvailbleTimeOfGroupDtos = ctx.Database.SqlQuery<NotAvailbleTimeOfGroupDto>("Select NL.id ,L.name as lecturer, NL.timeSlot  " +
            //                                                    "from NotAvailbleTimesOfLecturers NL INNER JOIN Lecturers L ON NL.[lecturer] = L.id   ").ToList();
            //}

            ////.ItemsSource = notAvailbleTimeOfGroupDtos;
        }

        private void not_avilable_group_deleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        //Start of Concecutive Session
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
                
                if (session.consecutive_session_id != null) {
                    string session1 = session.subject_code + "-" + session.tag + "-" + session.groupId;

                    using (var ctx = new GTimeTableEntities())
                    {
                        string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration, S.consecutive_session_id AS consecutive_session_id  " +
                                                                        "from Sessions S " +
                                                                        "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                        "INNER JOIN Tags T ON S.tag = T.id " +
                                                                        "INNER JOIN Student ST ON S.student = ST.id "+
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

        private void concecutiveSession1Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (concecutiveSession1Combobox.SelectedItem != null) {
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
                                                                    "INNER JOIN Student ST ON S.student = ST.id "+
                                                                    "where S.subject = " + subject.id +
                                                                    " and S.student = " + student.id;

                    sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                }

                foreach (var session in sessionDto)
                {
                    if(selected_session.id != session.id)
                    {
                        concecutiveSession2ComboBox.Items.Add(session.subject_code + "-" + session.tag + "-" + session.groupId);
                    }
                }

            } 
        }

        private void concecutiveSession_Add_Button_Click(object sender, RoutedEventArgs e)
        {

            if (concecutiveSession1Combobox.Text == "" || concecutiveSession2ComboBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
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
                                                                            "INNER JOIN Student ST ON S.student = ST.id "+
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
        }
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
        private void cleanConcecutiveSession()
        {
            concecutiveSession1Combobox.SelectedIndex = -1;
            concecutiveSession2ComboBox.SelectedIndex = -1;
        }
    }
}
