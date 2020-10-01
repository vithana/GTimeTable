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
    /// Interaction logic for Sessions.xaml
    /// </summary>
    public partial class Sessions : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();

        int session_id;
        Session session = new Session();
        LecturesSession lecture_session = new LecturesSession();
        public Sessions()
        {
            InitializeComponent();
            Load();

        }

        private void Load()
        {
            List<SessionDto> sessionDto = new List<SessionDto>();
            using (var ctx = new GTimeTableEntities())
            {
                string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                                "from Sessions S "+
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                sessionDto = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
            }


            sessionDataGrid.ItemsSource = sessionDto;
            edit_session_btn.Visibility = Visibility.Hidden;

            List<Lecturer> lecturers = new List<Lecturer>();
            lecturers = _db.Lecturers.ToList();
            foreach (var lecturer in lecturers)
            {
                if (lecturer != null)
                {
                    lectureTextBox.Items.Add(lecturer.name);
                }
            }

            List<Subject> subjects = new List<Subject>();
            subjects = _db.Subjects.ToList();
            foreach (var subject in subjects)
            {
                if (subject != null)
                {
                    subjectTextBox.Items.Add(subject.name);
                }
            }

            List<Tag> tags = new List<Tag>();
            tags = _db.Tags.ToList();
            foreach (var tag in tags)
            {
                if (tag != null)
                {
                    tagTextBox.Items.Add(tag.tag1);
                }
            }

            List<Student> students = new List<Student>();
            students = _db.Students.ToList();
            foreach (var student in students)
            {
                if (student != null)
                {
                    groupTextBox.Items.Add(student.groupId);
                }
            }

        }

        private void clean()
        {
            subjectTextBox.Text = "";
            tagTextBox.Text = "";
            groupTextBox.Text = "";
            stdCountTextBox.Text = "";
            durationTextBox.Text = "";
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
        private void add_session_btn_Click(object sender, RoutedEventArgs e)
        {
            if (subjectTextBox.Text == "" || tagTextBox.Text == "" || groupTextBox.Text == ""
                || stdCountTextBox.Text == "" || durationTextBox.Text == ""
            )
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
            {
                Subject subject = (from m in _db.Subjects
                                   where m.name.Equals(subjectTextBox.Text)
                                   select m).Single();

                Tag tag = (from m in _db.Tags
                           where m.tag1.Equals(tagTextBox.Text)
                           select m).Single();

                Student student = (from m in _db.Students
                                   where m.groupId.Equals(groupTextBox.Text)
                                   select m).Single();

                session.subject = subject.id;
                session.tag = tag.id;
                session.student = student.id;
                session.count = int.Parse(stdCountTextBox.Text);
                session.duration = durationTextBox.Text;

                Session new_session = _db.Sessions.Add(session);
                _db.SaveChanges();

                foreach (var item in lectureTextBox.SelectedItems)
                {
                    Lecturer lecture = (from m in _db.Lecturers
                                       where m.name.Equals(item.ToString())
                                       select m).Single();

                    lecture_session.session = new_session.id;
                    lecture_session.lecture = lecture.id;

                    _db.LecturesSessions.Add(lecture_session);
                    _db.SaveChanges();
                }

                using (var ctx = new GTimeTableEntities())
                {
                    string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                                "from Sessions S " +
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                    sessionDataGrid.ItemsSource = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                }


                clean();
            }
        }
        private void updateSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            int id = (sessionDataGrid.SelectedItem as SessionDto).id;

            Session session = (from m in _db.Sessions
                                 where m.id == id
                                 select m).Single();

            Subject subject = (from m in _db.Subjects
                               where m.id == session.subject
                               select m).Single();

            Tag tag = (from m in _db.Tags
                       where m.id == session.tag
                       select m).Single();

            Student student = (from m in _db.Students
                               where m.id == session.student
                               select m).Single();

            List<Lecturer> lecturerList = new List<Lecturer>();

            using (var ctx = new GTimeTableEntities())
            { 
                string sql = "Select L.* " +
                                "from Sessions S " +
                                "INNER JOIN LecturesSessions LS ON LS.session = S.id " +
                                "INNER JOIN Lecturers L ON LS.lecture = L.id " +
                                "WHERE LS.session = "+ id;

               lecturerList = ctx.Database.SqlQuery<Lecturer>(sql).ToList();
            }


            subjectTextBox.Text = subject.name;
            tagTextBox.Text = tag.tag1;
            groupTextBox.Text = student.groupId;
            stdCountTextBox.Text = session.count.ToString();
            durationTextBox.Text = session.duration;
            
            foreach (var item in lecturerList)
            {
                lectureTextBox.SelectedItems.Add(item.name);
            }
            
            session_id = session.id;


            edit_session_btn.Visibility = Visibility.Visible;
            add_session_btn.Visibility = Visibility.Hidden;
        }

        private void deleteSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            int id = (sessionDataGrid.SelectedItem as SessionDto).id;

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

            foreach (var item in lecturerSessionList)
            {
                var deletedLectureSession = _db.LecturesSessions.Where(m => m.id == item.id).Single();

                _db.LecturesSessions.Remove(deletedLectureSession);
                _db.SaveChanges();
            }

            var deletedSession = _db.Sessions.Where(m => m.id == id).Single();
            _db.Sessions.Remove(deletedSession);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                                "from Sessions S " +
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                sessionDataGrid.ItemsSource = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
            }

        }
        private void edit_session_btn_Click(object sender, RoutedEventArgs e)
        {
            Session session = (from m in _db.Sessions
                                 where m.id == session_id
                               select m).Single();

            if (subjectTextBox.Text == "" || tagTextBox.Text == "" || groupTextBox.Text == ""
                || stdCountTextBox.Text == "" || durationTextBox.Text == ""
            )
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
            {
                Subject subject = (from m in _db.Subjects
                                   where m.name.Equals(subjectTextBox.Text)
                                   select m).Single();

                Tag tag = (from m in _db.Tags
                           where m.tag1.Equals(tagTextBox.Text)
                           select m).Single();

                Student student = (from m in _db.Students
                                   where m.groupId.Equals(groupTextBox.Text)
                                   select m).Single();

                session.subject = subject.id;
                session.tag = tag.id;
                session.student = student.id;
                session.count = int.Parse(stdCountTextBox.Text);
                session.duration = durationTextBox.Text;

                List<LecturesSession> lecturerSessionList = new List<LecturesSession>();

                using (var ctx = new GTimeTableEntities())
                {
                    string sql = "Select LS.* " +
                                    "from Sessions S " +
                                    "INNER JOIN LecturesSessions LS ON LS.session = S.id " +
                                    "INNER JOIN Lecturers L ON LS.lecture = L.id " +
                                    "WHERE LS.session = " + session_id;

                    lecturerSessionList = ctx.Database.SqlQuery<LecturesSession>(sql).ToList();
                }

                foreach (var item in lecturerSessionList)
                {
                    var deletedLectureSession = _db.LecturesSessions.Where(m => m.id == item.id).Single();

                    _db.LecturesSessions.Remove(deletedLectureSession);
                    _db.SaveChanges();
                }

                _db.SaveChanges();

                foreach (var item in lectureTextBox.SelectedItems)
                {
                    Lecturer lecture = (from m in _db.Lecturers
                                        where m.name.Equals(item.ToString())
                                        select m).Single();

                    lecture_session.session = session_id;
                    lecture_session.lecture = lecture.id;

                    _db.LecturesSessions.Add(lecture_session);
                    _db.SaveChanges();
                }

                using (var ctx = new GTimeTableEntities())
                {
                    string sql = "Select S.id, SU.name AS subject_name, SU.code AS subject_code, T.tag AS tag, ST.groupId AS groupId, S.count AS count, S.duration AS duration " +
                                                                "from Sessions S " +
                                                                "INNER JOIN Subjects SU ON S.subject = SU.id " +
                                                                "INNER JOIN Tags T ON S.tag = T.id " +
                                                                "INNER JOIN Student ST ON S.student = ST.id ";

                    sessionDataGrid.ItemsSource = ctx.Database.SqlQuery<SessionDto>(sql).ToList();
                }


                clean();

                edit_session_btn.Visibility = Visibility.Hidden;
                add_session_btn.Visibility = Visibility.Visible;
            }


        }
        private void viewSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            int id = (sessionDataGrid.SelectedItem as SessionDto).id;

            SessionDto SessionDTO = (sessionDataGrid.SelectedItem as SessionDto);

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
            string message = "Session ID "+ id + "\n\n\t";

            for (int i = 0; i < lecturers.Count(); i++) {
                if (i == (lecturers.Count() -1))
                {
                    message += lecturers[i].name + "\n";
                }
                else
                {
                    message += lecturers[i].name + ", ";
                }
            }

            message += "\t"+SessionDTO.subject_name+"("+SessionDTO.subject_code+ ")\n\t" +
                       SessionDTO.tag+ "\n\t" + SessionDTO.groupId+ "\n\t" + SessionDTO.count+"("+SessionDTO.duration+")";

            MaterialMessageBox.Show(message);
        }
        
    }
}
