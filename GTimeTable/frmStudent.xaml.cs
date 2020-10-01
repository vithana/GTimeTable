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
    /// Interaction logic for frmStudent.xaml
    /// </summary>
    public partial class frmStudent : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();
        int student_id;
        private string acadamicyear_onchange = "";
        private string semester_onchange = "";
        private string program_onchange = "";
        private string GroupNo_onchange = "";
        private string subgroupNo_onchange = "";
        private ComboBox box = new ComboBox();

        
        Student student = new Student();

        public frmStudent()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            studentDataGrid.ItemsSource = _db.Students.ToList();
            button_update_save.Visibility = Visibility.Hidden;

        }

        private void clean()
        {
            acdamicYearComboBox.Text = "";
            semesterComboBox.Text = "";
            programComboBox.Text = "";
            groupNoComboBox.Text = "";
            groupIdTextBox.Text = "";
            subGroupNoComboBox.Text = "";
            subGropIdTextBox.Text = "";


        }

        private void button_add_Student_Click(object sender, RoutedEventArgs e)
        {

            if(semesterComboBox.Text == "" || acdamicYearComboBox.Text == "" || programComboBox.Text == "" || groupNoComboBox.Text == "" || groupIdTextBox.Text == "" ||
               subGroupNoComboBox.Text == "" || subGropIdTextBox.Text == "")
            {
                MaterialMessageBox.ShowError(@"Please Enter All Fields");

            }
            else
            {
                student.acdamicYear = acdamicYearComboBox.Text;
                student.semester = semesterComboBox.Text;
                student.program = programComboBox.Text;
                student.groupNo = groupNoComboBox.Text.ToString();
                student.groupId = groupIdTextBox.Text;
                student.subGroupNo = subGroupNoComboBox.Text.ToString();
                student.subGropId = subGropIdTextBox.Text;

                _db.Students.Add(student);
                _db.SaveChanges();

                studentDataGrid.ItemsSource = _db.Students.ToList();
                clean();
            }


        }
        private void updateStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (studentDataGrid.SelectedItem as Student).id;
            Student students = (from m in _db.Students
                where m.id == Id
                select m).Single();
            acdamicYearComboBox.Text = students.acdamicYear;
            semesterComboBox.Text = students.semester;
            programComboBox.Text = students.program;
            groupNoComboBox.Text = students.groupNo;
            groupIdTextBox.Text = students.groupId;
            subGroupNoComboBox.Text = students.subGroupNo;
            subGropIdTextBox.Text = students.subGropId;
            student_id = students.id;
            button_update_save.Visibility = Visibility.Visible;
            button_add_Student.Visibility = Visibility.Hidden;

        }
        private void deleteStudentBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (studentDataGrid.SelectedItem as Student).id;
            var deleteStudent = _db.Students.Where(m => m.id == Id).Single();
            _db.Students.Remove(deleteStudent);
            _db.SaveChanges();
            studentDataGrid.ItemsSource = _db.Students.ToList();
        }

        private void button_update_save_Click(object sender, RoutedEventArgs e)
        {

            Student students = (from m in _db.Students
                where m.id == student_id
                               select m).Single();

            students.acdamicYear = acdamicYearComboBox.Text;
            students.semester = semesterComboBox.Text;
            students.program = programComboBox.Text;
            students.groupNo = groupNoComboBox.Text.ToString();
            students.groupId = groupIdTextBox.Text;
            students.subGroupNo = subGroupNoComboBox.Text.ToString();
            students.subGropId = subGropIdTextBox.Text;

            _db.SaveChanges();

            studentDataGrid.ItemsSource = _db.Students.ToList();
            clean();
            button_update_save.Visibility = Visibility.Hidden;
            button_add_Student.Visibility = Visibility.Visible;

        }

        private void GroupNocomboboxOnchange(object sender, SelectionChangedEventArgs e)
        {
            if (acdamicYearComboBox.Text != "" && semesterComboBox.Text != "" && programComboBox.Text != "")
            {
                acadamicyear_onchange = acdamicYearComboBox.Text;
                semester_onchange = semesterComboBox.Text;
                program_onchange = programComboBox.Text;
                GroupNo_onchange = (groupNoComboBox.SelectedIndex + 1).ToString();
                subGroupNoComboBox.Text = "";
                subGropIdTextBox.Text = "";
                genarateGropId();
            }
            else
            {               
                MaterialMessageBox.ShowError(@"Cannot be null values in to Academic Year , Semester or Program");
                
            }
        }

        private void SubGroupNocomboboxOnchange(object sender, SelectionChangedEventArgs e)
        {
            acadamicyear_onchange = acdamicYearComboBox.Text;
            semester_onchange = semesterComboBox.Text;
            program_onchange = programComboBox.Text;
            GroupNo_onchange = groupNoComboBox.Text;
            subgroupNo_onchange = (subGroupNoComboBox.SelectedIndex + 1).ToString();
            genarateSubGroupId();
        }
        

        private void genarateGropId()
        {
            string groupId = "";

            if (acadamicyear_onchange != "")
            {
                groupId = acadamicyear_onchange + ".";
                if (semester_onchange != "")
                {
                    groupId = groupId + semester_onchange + ".";

                    if (program_onchange != "")
                    {
                        groupId = groupId + program_onchange + ".";

                        if (GroupNo_onchange != "")
                        {
                            groupId = groupId + GroupNo_onchange ;
                        }
                    }
                }
            }

            groupIdTextBox.Text = groupId;
        }

        private void genarateSubGroupId()
        {
            string SubgroupId = "";

            if (acadamicyear_onchange != "")
            {
                SubgroupId = acadamicyear_onchange + ".";
                if (semester_onchange != "")
                {
                    SubgroupId = SubgroupId + semester_onchange + ".";

                    if (program_onchange != "")
                    {
                        SubgroupId = SubgroupId + program_onchange + ".";

                        if (GroupNo_onchange != "")
                        {
                            SubgroupId = SubgroupId + GroupNo_onchange + ".";
                            if (subgroupNo_onchange != "")
                            {
                                SubgroupId = SubgroupId + subgroupNo_onchange;
                            }
                        }
                    }
                }
            }

            subGropIdTextBox.Text = SubgroupId;
        }
    }
}
