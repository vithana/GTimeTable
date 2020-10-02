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
    /// Interaction logic for Staticstic.xaml
    /// </summary>
    public partial class Staticstic : UserControl
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


        public Staticstic()
        {
            InitializeComponent();
            lecturerCount();
        }

        private void lecturerCount()
        {
            List<Building> buildings = new List<Building>();
            buildings = _db.Buildings.ToList();

            List<Room> rooms = new List<Room>();
            rooms = _db.Rooms.ToList();

            List<Subject> subjects = new List<Subject>();
            subjects = _db.Subjects.ToList();

            List<Session> sessions = new List<Session>();
            sessions = _db.Sessions.ToList();

            List<Student> groups = new List<Student>();
            groups = _db.Students.ToList();

        



            var lecCount =0;
            var subCount = 0;
            var roomCount = 0;
            var sessCount = 0;
            var groupCount = 0;
            var buildingCount = 0;


            foreach (var building in buildings)
            {
                lecCount++;
            }

        

            foreach (var room in rooms)
            {
                roomCount++;
            }

          

            foreach (var subject in subjects)
            {
                subCount++;
            }

            foreach (var session in sessions)
            {
                sessCount++;
            }

            foreach (var building in buildings)
            {
                buildingCount++;
            }

            foreach (var group in groups)
            {
                groupCount++;
            }



            numOfLecTxtBx.Text = lecCount.ToString();
            numOfSubTxtBx.Text = subCount.ToString();
            numOfRoomsTxtBx.Text = roomCount.ToString();
            numOfSessionTxtBx.Text = sessCount.ToString();
            numOfBuildingTxtBx.Text = buildingCount.ToString();
            numOfGroupsTxtBx.Text = groupCount.ToString();


        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
