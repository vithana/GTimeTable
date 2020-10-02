using BespokeFusion;
using GTimeTable.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
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

        //------- Add  Buildng  Tab-----------------
        int buildingId;
        int roomId;
        Building building = new Building();

        //------- Add  Rooms  Tab-----------------
        Room room;

        //-------- Suitable Rooms for tab -------------
        SutiableTabforRoom suitableRoomsforTag;
        SuitableRoomsforSubjectTag SuitableRoomsforSubjectTag;
        SuitableRoomsforLecturer SuitableRoomsforLecturer;
        SuiableRoomsforSession SuiableRoomsforSession;
        SuitableRoomsforGroup SuitableRoomsforGroup;

        public Locations()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            
            BuildingLoad();
            RoomsLoad();
           
        }
        //=============== Main clear Function ==================================

         private void clearMainFunction()
        {
            suitableRoomsforTagsClearText();
            SuitableRoomsforSubjectandTagsClearText();
            SuitableRoomsforLecturersClear();
            SuitableRoomsforLecturersClear();
            SuitableRoomsforGroupClear();
        }

        private void BuildingLoad()
        {
            clearMainFunction();
            //Showing Values in the Building Tab
            buildingDataGrid.ItemsSource = _db.Buildings.ToList();
            saveBuildingName.Visibility = Visibility.Hidden;
        }

        // ================================= Rooms Tab Function  ============================================================
        private void RoomsLoad()
        {
            clearMainFunction();
            List<RoomsDto> roomDto = new List<RoomsDto>();
            
            using (var ctx = new GTimeTableEntities())
            {
               roomDto = ctx.Database.SqlQuery<RoomsDto>("SELECT R.id , R.capacity, R.roomId, R.roomType, B.name As building " +
                                                                "FROM Rooms R INNER JOIN Buildings B ON R.[building] = B.id   ").ToList();
            }

            roomDataGrid.ItemsSource = roomDto;
                

            List<Building> buildings = new List<Building>();
            buildings = _db.Buildings.ToList();


            foreach (var building in buildings)
            {
                if (building != null)
                {
                    buildingComboBox.Items.Add(building.name);
                }
            }

            roomTypeComboBox.Items.Add("Lecture Hall");
            roomTypeComboBox.Items.Add("Laboratory");
         
        }

        private void CleanRoom()
        {
            capacityTextbox.Text = "";
            roomIdTextBox.Text = "";
        }
        private void AddRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            //Add Function
            if (addRoomBtn.Content.Equals("Add"))
            {
                room = new Room();

                Building building = (from m in _db.Buildings
                                     where m.name.Equals(buildingComboBox.Text)
                                     select m).Single();

                room.capacity = int.Parse(capacityTextbox.Text);
                room.roomId = roomIdTextBox.Text;
                room.building = building.id;
                room.roomType = roomTypeComboBox.Text;

                _db.Rooms.Add(room);
                _db.SaveChanges();
                using (var ctx = new GTimeTableEntities())
                {
                    roomDataGrid.ItemsSource = ctx.Database.SqlQuery<RoomsDto>("SELECT R.id , R.capacity, R.roomId, R.roomType, B.name As building " +
                                                                     "FROM Rooms R INNER JOIN Buildings B ON R.[building] = B.id   ").ToList();
                }
                CleanRoom();
            }
            //update a room
            else
            {
                Room room = (from m in _db.Rooms
                             where m.id == roomId
                             select m).Single();

                Building building = (from m in _db.Buildings
                                     where m.name.Equals(buildingComboBox.Text)
                                     select m).Single();

                room.capacity = int.Parse(capacityTextbox.Text);
                room.roomId = roomIdTextBox.Text;
                room.building = building.id;
                room.roomType = roomTypeComboBox.Text;

                _db.SaveChanges();
                using (var ctx = new GTimeTableEntities())
                {
                    roomDataGrid.ItemsSource = ctx.Database.SqlQuery<RoomsDto>("SELECT R.id , R.capacity, R.roomId, R.roomType, B.name As building " +
                                                                     "FROM Rooms R INNER JOIN Buildings B ON R.[building] = B.id   ").ToList();
                }
                CleanRoom();
            }

        }

        private void updateRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            addRoomBtn.Content = "Save";

            int Id = (roomDataGrid.SelectedItem as RoomsDto).id;

            Room room = (from m in _db.Rooms
                         where m.id == Id
                         select m).Single();

            Building building = (from m in _db.Buildings
                                 where m.id == room.building
                                 select m).Single();

            buildingComboBox.Text = building.name;
            roomIdTextBox.Text = room.roomId;
            capacityTextbox.Text = room.capacity.ToString();
            roomTypeComboBox.Text = room.roomType;
            roomId = room.id;
        }

        private void deleteRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (roomDataGrid.SelectedItem as RoomsDto).id;
            var deletedRoom = _db.Rooms.Where(r => r.id == Id).Single();
            _db.Rooms.Remove(deletedRoom);
            _db.SaveChanges();

            using (var ctx = new GTimeTableEntities())
            {
                roomDataGrid.ItemsSource = ctx.Database.SqlQuery<RoomsDto>("SELECT R.id , R.capacity, R.roomId, R.roomType, B.name As building " +
                                                                 "FROM Rooms R INNER JOIN Buildings B ON R.[building] = B.id   ").ToList();
            }


        }

        //===============================================================================================================================

        // ================================= Suitable Rooms For Tags Tab Function  ============================================================

        // --------------- Main Load Function -----------------------------------
        private void TBSuitableRoomsforTags(object sender, MouseButtonEventArgs e)
        {
            clearMainFunction();
            Load_TBSuitableRoomsForTags();
            List<suitableroomsforTagDto> suitableRoomsforTags = new List<suitableroomsforTagDto>();
            using (var ctx = new GTimeTableEntities())
            {
                suitableRoomsforTags = ctx.Database.SqlQuery<suitableroomsforTagDto>("select s.id, (select r.roomId from Rooms r where r.id = s.room) as  room , " +
                    "(select t.tag from Tags t where t.id = s.tag) as tag from SutiableTabforRoom s  ").ToList();
            }

            sutiableTabforRoomDataGrid.ItemsSource = suitableRoomsforTags;



        }

        private void Load_TBSuitableRoomsForTags()
        {
            List<Tag> LstTags = new List<Tag>();
            LstTags = _db.Tags.ToList();

            List<Room> LstRooms = new List<Room>();
            LstRooms = _db.Rooms.ToList();

            foreach (var tags in LstTags)
            {
                if (tags != null)
                {
                    comboBox_Tags_TBSuitableRoomsForTags.Items.Add(tags.tag1);
                }
            }

            foreach (var room in LstRooms)
            {
                if (room != null)
                {
                    comboBox_Rooms_TBSuitableRoomsForTags.Items.Add(room.roomId);
                }
            }
        }

        //----------------- Add Button click Function ---------------------
        private void addClickSuitableRoomsfoTag_TBSuitableRoomsForTags(object sender, RoutedEventArgs e)
            {

                if (comboBox_Tags_TBSuitableRoomsForTags.Text == "")
                {
                    MaterialMessageBox.ShowError(@"Please Select Tag Befor Save");
                }
                else if (comboBox_Rooms_TBSuitableRoomsForTags.Text == "")
                {
                    MaterialMessageBox.ShowError(@"Please Select Room Befor Save");
                }
                else
                {
                    try
                    {
                        suitableRoomsforTag = new SutiableTabforRoom();
                        Tag tag = (from m in _db.Tags
                                   where m.tag1.Equals(comboBox_Tags_TBSuitableRoomsForTags.Text)
                                   select m).Single();

                        Room room = (from m in _db.Rooms
                                     where m.roomId.Equals(comboBox_Rooms_TBSuitableRoomsForTags.Text)
                                     select m).Single();

                        suitableRoomsforTag.room = room.id;
                        suitableRoomsforTag.tag = tag.id;
                        _db.SutiableTabforRooms.Add(suitableRoomsforTag);
                        _db.SaveChanges();
                        using (var ctx = new GTimeTableEntities())
                        {
                                sutiableTabforRoomDataGrid.ItemsSource = ctx.Database.SqlQuery<suitableroomsforTagDto>("select s.id, (select r.roomId from Rooms r where r.id = s.room) as  room , " +
                                "(select t.tag from Tags t where t.id = s.tag) as tag from SutiableTabforRoom s  ").ToList();
                        }
                }
                    catch (Exception exe)
                    {
                        MessageBox.Show(exe.ToString());
                    }

                    suitableRoomsforTagsClearText();
                    Load_TBSuitableRoomsForTags();

            }


            }
        //-------------- Delete Function in Grid ------------------------------------

        private void deleteRoomBtnClick_TBSuitableRoomsForTags(object sender, RoutedEventArgs e)
        {
            int Id = (sutiableTabforRoomDataGrid.SelectedItem as suitableroomsforTagDto).id;
            var deletedRoom = _db.SutiableTabforRooms.Where(r => r.id == Id).Single();
            _db.SutiableTabforRooms.Remove(deletedRoom);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                sutiableTabforRoomDataGrid.ItemsSource = ctx.Database.SqlQuery<suitableroomsforTagDto>("select s.id, (select r.roomId from Rooms r where r.id = s.room) as  room , " +
                "(select t.tag from Tags t where t.id = s.tag) as tag from SutiableTabforRoom s  ").ToList();
            }

        }

        //-------- Clear Data Function ------------
        private void suitableRoomsforTagsClearText()
            {
                comboBox_Tags_TBSuitableRoomsForTags.Text = "";
                comboBox_Rooms_TBSuitableRoomsForTags.Text = "";
                comboBox_Tags_TBSuitableRoomsForTags.Items.Clear();
                comboBox_Rooms_TBSuitableRoomsForTags.Items.Clear();

            }


        //=============================================================================================================================================



        // ================================= Suitable Roooms For Lectures And  Tags  ============================================================
        private void TBSuitableRoomsforSubjectandTags(object sender, MouseButtonEventArgs e)
        {
            clearMainFunction();
            SuitableRoomsforSubjectandTagsClearText();
            Load_SuitableRoomsforSubjectandTags();

            List<SuitableRoomsForSubjectandTagsscs> SuitableRoomsForSubjectandTagsscs = new List<SuitableRoomsForSubjectandTagsscs>();
            using (var ctx = new GTimeTableEntities())
            {
                SuitableRoomsForSubjectandTagsscs = ctx.Database.SqlQuery<SuitableRoomsForSubjectandTagsscs>("select s.id , (select sb.name from Subjects sb where sb.id = s.subject) as subject , (select t.tag from Tags t where t.id = s.tag) as tag ," +
                    "(select r.roomId from Rooms r where r.id = s.room) as room  from SuitableRoomsforSubjectTag s").ToList();
            }

            suitableRoomsforSubjectTagDataGrid.ItemsSource = SuitableRoomsForSubjectandTagsscs;


        }
        private void Load_SuitableRoomsforSubjectandTags()
        {
            List<Tag> LstTags_SuitableRoomsforSubjectandTags = new List<Tag>();
            LstTags_SuitableRoomsforSubjectandTags = _db.Tags.ToList();


            List<Subject> LstSubjects_SuitableRoomsforSubjectandTags = new List<Subject>();
            LstSubjects_SuitableRoomsforSubjectandTags = _db.Subjects.ToList();

            foreach (var tags in LstTags_SuitableRoomsforSubjectandTags)
            {
                if (tags != null)
                {
                    combobox_tags_SuitableRoomsforSubjectandTags.Items.Add(tags.tag1);
                }
            }

            foreach (var subject in LstSubjects_SuitableRoomsforSubjectandTags)
            {
                if (subject != null)
                {
                    combobox_subject_SuitableRoomsforSubjectandTags.Items.Add(subject.name);
                }
            }

        }

        //---------------------------------------------- Tags Combo Box Onchange Function ------------------------------------------------------
        private void Combobox_tags_SuitableRoomsforSubjectandTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Room> LstRooms_SuitableRoomsforSubjectandTags = new List<Room>();
            combobox_rooms_SuitableRoomsforSubjectandTags.Items.Clear();
            string Tag = "";
            Tag = (combobox_tags_SuitableRoomsforSubjectandTags.SelectedItem != null) ? combobox_tags_SuitableRoomsforSubjectandTags.SelectedItem.ToString() : "";
            
            if (combobox_tags_SuitableRoomsforSubjectandTags.Text != null)
            {                
                using (var ctx = new GTimeTableEntities())
                {
                    LstRooms_SuitableRoomsforSubjectandTags = ctx.Database.SqlQuery<Room>("select r.id , r.capacity , r.roomId , r.roomType , r.building " +
                                                                "from SutiableTabforRoom s , Tags t , Rooms r where t.id = s.tag AND s.room = r.id AND t.tag ='"+ Tag + "'" ).ToList();
                }

            }
            foreach (var room in LstRooms_SuitableRoomsforSubjectandTags)
            {
                if (room != null)
                {
                    combobox_rooms_SuitableRoomsforSubjectandTags.Items.Add(room.roomId);
                }
            }
        }
        //-------- Add Click Function ------------
        private void btnAddClick_SuitableRoomsforSubjectandTags(object sender, RoutedEventArgs e)
        {
            if (combobox_subject_SuitableRoomsforSubjectandTags.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Subject Befor Save");
            }
            else if (combobox_tags_SuitableRoomsforSubjectandTags.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Tasg Befor Save");
            }
            else if (combobox_rooms_SuitableRoomsforSubjectandTags.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Room Befor Save");
            }
            else
            {
                try
                {
                    SuitableRoomsforSubjectTag = new SuitableRoomsforSubjectTag();

                    Subject subject = (from m in _db.Subjects
                                       where m.name.Equals(combobox_subject_SuitableRoomsforSubjectandTags.Text)
                                       select m).Single();

                    Tag tag = (from m in _db.Tags
                               where m.tag1.Equals(combobox_tags_SuitableRoomsforSubjectandTags.Text)
                               select m).Single();

                    Room room = (from m in _db.Rooms
                                 where m.roomId.Equals(combobox_rooms_SuitableRoomsforSubjectandTags.Text)
                                 select m).Single();

                    SuitableRoomsforSubjectTag.subject = subject.id;
                    SuitableRoomsforSubjectTag.tag = tag.id;
                    SuitableRoomsforSubjectTag.room = room.id;
                    _db.SuitableRoomsforSubjectTags.Add(SuitableRoomsforSubjectTag);
                    _db.SaveChanges();
                    using (var ctx = new GTimeTableEntities())
                    {
                        suitableRoomsforSubjectTagDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForSubjectandTagsscs>("select s.id , (select sb.name from Subjects sb where sb.id = s.subject) as subject , (select t.tag from Tags t where t.id = s.tag) as tag ," +
                            "(select r.roomId from Rooms r where r.id = s.room) as room  from SuitableRoomsforSubjectTag s").ToList();
                    }

                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }

                SuitableRoomsforSubjectandTagsClearText();                
                Load_SuitableRoomsforSubjectandTags();
            }

        }

        //------------- Delete Click ----------------------------------------------
        private void deleteSubitableSubjClick_SuitableRoomsforSubjectandTags(object sender, RoutedEventArgs e)
        {
            int Id = (suitableRoomsforSubjectTagDataGrid.SelectedItem as SuitableRoomsForSubjectandTagsscs).id;
            var deletedRoom = _db.SuitableRoomsforSubjectTags.Where(r => r.id == Id).Single();
            _db.SuitableRoomsforSubjectTags.Remove(deletedRoom);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                suitableRoomsforSubjectTagDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForSubjectandTagsscs>("select s.id , (select sb.name from Subjects sb where sb.id = s.subject) as subject , (select t.tag from Tags t where t.id = s.tag) as tag ," +
                    "(select r.roomId from Rooms r where r.id = s.room) as room  from SuitableRoomsforSubjectTag s").ToList();
            }

        }

        //-------- Clear Data Function ------------
        private void SuitableRoomsforSubjectandTagsClearText()
        {
            combobox_tags_SuitableRoomsforSubjectandTags.Text = "";
            combobox_subject_SuitableRoomsforSubjectandTags.Text = "";
            combobox_rooms_SuitableRoomsforSubjectandTags.Text = "";
            combobox_tags_SuitableRoomsforSubjectandTags.Items.Clear();
            combobox_subject_SuitableRoomsforSubjectandTags.Items.Clear();
            combobox_rooms_SuitableRoomsforSubjectandTags.Items.Clear();

        }
        //=========================================================================================================================================     



        // ================================= Suitable Roooms For Lecturers ============================================================
        private void TBSuitableRoomsforLecturers(object sender, MouseButtonEventArgs e)
        {

            clearMainFunction();
            SuitableRoomsforLecturersClear();
            Load_SuitableRoomsforLecturers();

            using (var ctx = new GTimeTableEntities())
            {
                suitableRoomsforLecturerDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForLecturers>("select sl.id as id , (select sb.name from Lecturers sb where sb.id = sl.lecturer) as lecturer , (select r.roomId from Rooms r where r.id = sl.room) as room from SuitableroomsforLecturer sl;").ToList();
            }

        }

        //------------- Load Function -----------------------------
        public void Load_SuitableRoomsforLecturers()
        {
            List<Lecturer> LstLecturer_SuitableRoomsforLecturers = new List<Lecturer>();
            LstLecturer_SuitableRoomsforLecturers = _db.Lecturers.ToList();


            List<Room> LstRoom_SuitableRoomsforLecturers = new List<Room>();
            LstRoom_SuitableRoomsforLecturers = _db.Rooms.ToList();

            foreach (var lecturer in LstLecturer_SuitableRoomsforLecturers)
            {
                if (lecturer != null)
                {
                    comboBox_Lecturer_suitableRoomsforLecturer.Items.Add(lecturer.name);
                }
            }

            foreach (var rooms in LstRoom_SuitableRoomsforLecturers)
            {
                if (rooms != null)
                {
                    comboBox1_room_suitableRoomsforLecturer.Items.Add(rooms.roomId);
                }
            }

        }
        //-------------------- Add Function -----------------------------------------------
        private void btnClicEventAdd_suitableRoomsforLecturer(object sender, RoutedEventArgs e)
        {
            if (comboBox_Lecturer_suitableRoomsforLecturer.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Lecturer Befor Save");
            }
            else if (comboBox1_room_suitableRoomsforLecturer.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Room Befor Save");
            }
            else
            {

                try
                {
                    SuitableRoomsforLecturer = new SuitableRoomsforLecturer();

                    Lecturer lecturer = (from m in _db.Lecturers
                                         where m.name.Equals(comboBox_Lecturer_suitableRoomsforLecturer.Text)
                                         select m).Single();

                    Room room = (from m in _db.Rooms
                                 where m.roomId.Equals(comboBox1_room_suitableRoomsforLecturer.Text)
                                 select m).Single();

                    SuitableRoomsforLecturer.lecturer = lecturer.id;
                    SuitableRoomsforLecturer.room = room.id;
                    _db.SuitableRoomsforLecturers.Add(SuitableRoomsforLecturer);
                    _db.SaveChanges();
                    using (var ctx = new GTimeTableEntities())
                    {
                        suitableRoomsforLecturerDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForLecturers>("select sl.id as id , (select sb.name from Lecturers sb where sb.id = sl.lecturer) as lecturer , (select r.roomId from Rooms r where r.id = sl.room) as room from SuitableroomsforLecturer sl;").ToList();
                    }

                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }

            }
            SuitableRoomsforLecturersClear();
            Load_SuitableRoomsforLecturers();
        }

        //-------------------------------- Delete Function ------------------------------------------
        private void deleteSubitableSubjClick_suitableRoomsforLecturer(object sender, RoutedEventArgs e)
        {
            int Id = (suitableRoomsforLecturerDataGrid.SelectedItem as SuitableRoomsForLecturers).id;
            var deletedRow = _db.SuitableRoomsforLecturers.Where(r => r.id == Id).Single();
            _db.SuitableRoomsforLecturers.Remove(deletedRow);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                suitableRoomsforLecturerDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForLecturers>("select sl.id as id , (select sb.name from Lecturers sb where sb.id = sl.lecturer) as lecturer , (select r.roomId from Rooms r where r.id = sl.room) as room from SuitableroomsforLecturer sl;").ToList();
            }

        }

            //--------- Clear Function------------
            private void SuitableRoomsforLecturersClear()
        {
            comboBox_session_SuitableRoomsforSession.Text = "";
            comboBox_room_SuitableRoomsforSession.Text = "";
            comboBox_session_SuitableRoomsforSession.Items.Clear();
            comboBox_room_SuitableRoomsforSession.Items.Clear();            

        }

        //==========================================================================================================================



        // ================================= Suitable Roooms For Sessions ============================================================
        private void TBSuitableRoomsforSession(object sender, MouseButtonEventArgs e)
        {
            clearMainFunction();
            SuitableRoomsforSessionsClear();
            Load_SuitableRoomsforSession();

            using (var ctx = new GTimeTableEntities())
            {
                suiableRoomsforSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForSessionDto>("select S.id as id, S.session as session,  (select R.roomId from Rooms R where R.id = S.room) room    from SuiableRoomsforSession S;").ToList();
            }

        }
        //----------------- Load Function --------------------------------------------------------
        private void Load_SuitableRoomsforSession()
        {
            List<Session> LstSession_SuitableRoomsforSession = new List<Session>();
            LstSession_SuitableRoomsforSession = _db.Sessions.ToList();


            List<Room> LstRoom_SuitableRoomsforSession = new List<Room>();
            LstRoom_SuitableRoomsforSession = _db.Rooms.ToList();

            foreach (var session in LstSession_SuitableRoomsforSession)
            {
                if (session != null)
                {
                    comboBox_session_SuitableRoomsforSession.Items.Add(session.id);
                }
            }

            foreach (var rooms in LstRoom_SuitableRoomsforSession)
            {
                if (rooms != null)
                {
                    comboBox_room_SuitableRoomsforSession.Items.Add(rooms.roomId);
                }
            }

        }

        //----------------------------- View Session Function ---------------------------------
        private void btnclickViewSession_SuitableRoomsforSession(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(comboBox_session_SuitableRoomsforSession.Text);
            viewSessionCommon_SuitableRoomsforSession(id);

        }

        //------------------- Grid View Session Button Click ------------------------------------------
        private void ViewSesssionSubitableSessionClick_SuitableRoomsforSession(object sender, RoutedEventArgs e)
        {
            int id = (suiableRoomsforSessionDataGrid.SelectedItem as SuitableRoomsForSessionDto).session;
            viewSessionCommon_SuitableRoomsforSession(id);
        }

        //------------------------ View Session Common Function -------------------------------------------
        private void viewSessionCommon_SuitableRoomsforSession(int id)
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

        // ---------------------- Add Function -----------------------------------------------
        private void btnclickAddClick_SuitableRoomsforSession(object sender, RoutedEventArgs e)
        {
            if (comboBox_session_SuitableRoomsforSession.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Session Befor Save");
            }
            else if (comboBox_room_SuitableRoomsforSession.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Room Befor Save");
            }
            else
            {
                try
                {
                    SuiableRoomsforSession = new SuiableRoomsforSession();                   

                    Room room = (from m in _db.Rooms
                                 where m.roomId.Equals(comboBox_room_SuitableRoomsforSession.Text.ToString())
                                 select m).Single();

                    SuiableRoomsforSession.session = Convert.ToInt32(comboBox_session_SuitableRoomsforSession.Text);
                    SuiableRoomsforSession.room = room.id;
                    _db.SuiableRoomsforSessions.Add(SuiableRoomsforSession);
                    _db.SaveChanges();
                    using (var ctx = new GTimeTableEntities())
                    {
                        suiableRoomsforSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForSessionDto>("select S.id as id, S.session as session,  (select R.roomId from Rooms R where R.id = S.room) room    from SuiableRoomsforSession S;").ToList();
                    }

                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }

            }

        }
        //------------------- Delete Button Click ------------------------------------------
        private void deleteSubitableSessionClick_SuitableRoomsforSession(object sender, RoutedEventArgs e)
        {
            int id = (suiableRoomsforSessionDataGrid.SelectedItem as SuitableRoomsForSessionDto).id;
            var deletedRow = _db.SuiableRoomsforSessions.Where(r => r.id == id).Single();
            _db.SuiableRoomsforSessions.Remove(deletedRow);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                suiableRoomsforSessionDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsForSessionDto>("select S.id as id, S.session as session,  (select R.roomId from Rooms R where R.id = S.room) room    from SuiableRoomsforSession S;").ToList();
            }

        }
        


        //------------------- Clear Function --------------------------------------
        private void SuitableRoomsforSessionsClear()
        {
            comboBox_session_SuitableRoomsforSession.Text = "";
            comboBox_room_SuitableRoomsforSession.Text = "";
            comboBox_session_SuitableRoomsforSession.Items.Clear();
            comboBox_room_SuitableRoomsforSession.Items.Clear();

        }       

        //======================================================================================================================

        // ================================= Suitable Roooms For Group ============================================================
        private void TBSuitableRoomsforGroup(object sender, MouseButtonEventArgs e)
        {
            clearMainFunction();
            SuitableRoomsforGroupClear();
            Load_SuitableRoomsforGroup();

            using (var ctx = new GTimeTableEntities())
            {
                suitableRoomsforGroupDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsforGroupDto>("select S.id as id, (select st.groupId from Student st where st.id = S.group_id) as group_id , (select R.roomId from Rooms R where R.id = S.room) as room from SuitableRoomsforGroup S").ToList();
            }

        }

        //----------------------- Load Function ------------------------
        private void Load_SuitableRoomsforGroup()
        {
            List<Student> LstGroup_SuitableRoomsforGroup = new List<Student>();
            LstGroup_SuitableRoomsforGroup = _db.Students.ToList();


            List<Room> LstRoom_SuitableRoomsforGroup = new List<Room>();
            LstRoom_SuitableRoomsforGroup = _db.Rooms.ToList();

            foreach (var group in LstGroup_SuitableRoomsforGroup)
            {
                if (group != null)
                {
                    comboBox_group_SuitableRoomsforGroup.Items.Add(group.groupId);
                }
            }

            foreach (var rooms in LstRoom_SuitableRoomsforGroup)
            {
                if (rooms != null)
                {
                    comboBox_room_SuitableRoomsforGroup.Items.Add(rooms.roomId);
                }
            }

        }

        //------------------------- Clear Function ---------------------------
        private void SuitableRoomsforGroupClear()
        {
            comboBox_group_SuitableRoomsforGroup.Text = "";
            comboBox_room_SuitableRoomsforGroup.Text = "";
            comboBox_group_SuitableRoomsforGroup.Items.Clear();
            comboBox_room_SuitableRoomsforGroup.Items.Clear();
        }

        //------------------------ Addd Click Button ------------------------------------
        private void Button_add_SuitableRoomsforGroup_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_group_SuitableRoomsforGroup.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Group Befor Save");
            }
            else if (comboBox_room_SuitableRoomsforGroup.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Select Room Befor Save");
            }
            else
            {
                try
                {
                    SuitableRoomsforGroup = new SuitableRoomsforGroup();

                    Student student = (from m in _db.Students
                                 where m.groupId.Equals(comboBox_group_SuitableRoomsforGroup.Text)
                                 select m).Single();

                    Room room = (from m in _db.Rooms
                                 where m.roomId.Equals(comboBox_room_SuitableRoomsforGroup.Text)
                                 select m).Single();

                    SuitableRoomsforGroup.group_id = student.id;
                    SuitableRoomsforGroup.room = room.id;
                    _db.SuitableRoomsforGroups.Add(SuitableRoomsforGroup);
                    _db.SaveChanges();

                    using (var ctx = new GTimeTableEntities())
                    {
                        suitableRoomsforGroupDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsforGroupDto>("select S.id as id, (select st.groupId from Student st where st.id = S.group_id) as group_id , (select R.roomId from Rooms R where R.id = S.room) as room from SuitableRoomsforGroup S").ToList();
                    }

                }
                catch (Exception exe)
                {
                    MessageBox.Show(exe.ToString());
                }

            }
            SuitableRoomsforGroupClear();
            Load_SuitableRoomsforGroup();

        }

        //------------------- Delete Button Click ------------------------------------------
        private void deleteSuitableRoomforGroupLClick_SuitableRoomsforGroup(object sender, RoutedEventArgs e)
        {
            int id = (suitableRoomsforGroupDataGrid.SelectedItem as SuitableRoomsforGroupDto).id;
            var deletedRow = _db.SuitableRoomsforGroups.Where(r => r.id == id).Single();
            _db.SuitableRoomsforGroups.Remove(deletedRow);
            _db.SaveChanges();
            using (var ctx = new GTimeTableEntities())
            {
                suitableRoomsforGroupDataGrid.ItemsSource = ctx.Database.SqlQuery<SuitableRoomsforGroupDto>("select S.id as id, (select st.groupId from Student st where st.id = S.group_id) as group_id , (select R.roomId from Rooms R where R.id = S.room) as room from SuitableRoomsforGroup S").ToList();
            }

        }


        //=============================================================================================================================



        private void clean()
        {
            buildingNameTextBox.Text = "";
            capacityTextbox.Text = "";
            roomIdTextBox.Text = "";
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
            addBuildingName.Visibility = Visibility.Hidden;
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
            if(buildingNameTextBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Enter Building Name");
            }
            else
            {
                building.name = buildingNameTextBox.Text;
                _db.Buildings.Add(building);
                _db.SaveChanges();
                buildingDataGrid.ItemsSource = _db.Buildings.ToList();
                clean();
            }
            
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
            addBuildingName.Visibility = Visibility.Visible;
        }

       
    }
}
