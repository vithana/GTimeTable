using BespokeFusion;
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
    /// Interaction logic for Lecturers.xaml
    /// </summary>
    public partial class Lecturers : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();

        int lecturer_id;
        Lecturer lecturer = new Lecturer();

        public Lecturers()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            lecturerDataGrid.ItemsSource = _db.Lecturers.ToList();
            edit_lecture_btn.Visibility = Visibility.Hidden;
        }
        private void clean()
        {
            nameTextBox.Text = "";
            emp_idTextBox.Text = "";
            facultyTextBox.Text = "";
            deptTextBox.Text = "";
            centerTextBox.Text = "";
            buildingTextBox.Text = "";
            lvlTextBox.Text = "";
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

        private void add_lecture_btn_Click(object sender, RoutedEventArgs e)
        {
            int level;

            switch (lvlTextBox.Text) {
                case "Professor":
                    level = 1;
                    break;
                case "Assistant Professor":
                    level = 2;
                    break;
                case "Senior Lecturer(HG)":
                    level = 3;
                    break;
                case "Senior Lecturer":
                    level = 4;
                    break;
                case "Lecturer":
                    level = 5;
                    break;
                case "Assistant Lecturer":
                    level = 6;
                    break;
                case "Instructors":
                    level = 7;
                    break;
                default:
                    level = 1;
                    break;
            }

            if (nameTextBox.Text == "" || emp_idTextBox.Text== "" || facultyTextBox.Text == "" 
                || deptTextBox.Text == "" || centerTextBox.Text == "" || buildingTextBox.Text == "" 
                || lvlTextBox.Text == ""
            )
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else {
                if (emp_idTextBox.Text.Length != 6) {
                    MaterialMessageBox.ShowError(@"Employee ID should have 6 digits");
                }
                else { 
                    lecturer.name = nameTextBox.Text;
                    lecturer.emp_id = emp_idTextBox.Text;
                    lecturer.faculty = facultyTextBox.Text;
                    lecturer.dept = deptTextBox.Text;
                    lecturer.center = centerTextBox.Text;
                    lecturer.building = buildingTextBox.Text;
                    lecturer.lvl = level;
                    lecturer.rank = level + "."+ emp_idTextBox.Text;

                    _db.Lecturers.Add(lecturer);
                    _db.SaveChanges();

                    lecturerDataGrid.ItemsSource = _db.Lecturers.ToList();
                    clean();
                }
            }
        }

        //get details of working days of week
        private void updateLecBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (lecturerDataGrid.SelectedItem as Lecturer).id;
            Lecturer lecturer = (from m in _db.Lecturers
                                             where m.id == Id
                                             select m).Single();

            string level;

            switch (lecturer.lvl)
            {
                case 1:
                    level = "Professor";
                    break;
                case 2:
                    level = "Assistant Professor";
                    break;
                case 3:
                    level = "Senior Lecturer(HG)";
                    break;
                case 4:
                    level ="Senior Lecturer";
                    break;
                case 5:
                    level = "Lecturer";
                    break;
                case 6:
                    level = "Assistant Lecturer";
                    break;
                case 7:
                    level = "Instructors";
                    break;
                default:
                    level = "Professor";
                    break;
            }
            nameTextBox.Text = lecturer.name;
            emp_idTextBox.Text = lecturer.emp_id;
            facultyTextBox.Text = lecturer.faculty;
            deptTextBox.Text = lecturer.dept;
            centerTextBox.Text = lecturer.center;
            buildingTextBox.Text = lecturer.building;
            lvlTextBox.Text = level;

            lecturer_id = lecturer.id;

            edit_lecture_btn.Visibility = Visibility.Visible;
            add_lecture_btn.Visibility = Visibility.Hidden;
        }

        private void deleteLecBtn_Click(object sender, RoutedEventArgs e)
        {
            //int Id = (workingDaysPerWeekDataGrid.SelectedItem as Day).id;
            int Id = (lecturerDataGrid.SelectedItem as Lecturer).id;
            var deletedLecturer = _db.Lecturers.Where(m => m.id == Id).Single();
            _db.Lecturers.Remove(deletedLecturer);
            _db.SaveChanges();
            lecturerDataGrid.ItemsSource = _db.Lecturers.ToList();

        }
        private void edit_lecture_btn_Click(object sender, RoutedEventArgs e)
        {
            Lecturer lecturer = (from m in _db.Lecturers
                                             where m.id == lecturer_id
                                             select m).Single();

            int level;

            switch (lvlTextBox.Text)
            {
                case "Professor":
                    level = 1;
                    break;
                case "Assistant Professor":
                    level = 2;
                    break;
                case "Senior Lecturer(HG)":
                    level = 3;
                    break;
                case "Senior Lecturer":
                    level = 4;
                    break;
                case "Lecturer":
                    level = 5;
                    break;
                case "Assistant Lecturer":
                    level = 6;
                    break;
                case "Instructors":
                    level = 7;
                    break;
                default:
                    level = 1;
                    break;
            }

            if (nameTextBox.Text == "" || emp_idTextBox.Text == "" || facultyTextBox.Text == ""
                || deptTextBox.Text == "" || centerTextBox.Text == "" || buildingTextBox.Text == ""
                || lvlTextBox.Text == ""
            )
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
            {
                if (emp_idTextBox.Text.Length != 6)
                {
                    MaterialMessageBox.ShowError(@"Employee ID should have 6 digits");
                }
                else
                {
                    lecturer.name = nameTextBox.Text;
                    lecturer.emp_id = emp_idTextBox.Text;
                    lecturer.faculty = facultyTextBox.Text;
                    lecturer.dept = deptTextBox.Text;
                    lecturer.center = centerTextBox.Text;
                    lecturer.building = buildingTextBox.Text;
                    lecturer.lvl = level;
                    lecturer.rank = level + "." + emp_idTextBox.Text;

                    _db.SaveChanges();
                    lecturerDataGrid.ItemsSource = _db.Lecturers.ToList();
                    clean();

                    edit_lecture_btn.Visibility = Visibility.Hidden;
                    add_lecture_btn.Visibility = Visibility.Visible;
                }
            }

            
        }
    }
}
