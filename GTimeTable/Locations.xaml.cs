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
                List<suitableroomsforTagDto> suitableRoomsforTags = new List<suitableroomsforTagDto>();

                using (var ctx = new GTimeTableEntities())
                {
                    suitableRoomsforTags = ctx.Database.SqlQuery<suitableroomsforTagDto>("select s.id, (select r.roomId from Rooms r where r.id = s.room) as  room , " +
                        "(select t.tag from Tags t where t.id = s.tag) as tag from SutiableTabforRoom s  ").ToList();
                }

                sutiableTabforRoomDataGrid.ItemsSource = suitableRoomsforTags;

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

                if (comboBox_Rooms_TBSuitableRoomsForTags.Text == "")
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

        private void clean() {
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
