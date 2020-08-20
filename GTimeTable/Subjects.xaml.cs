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
    /// Interaction logic for Subjects.xaml
    /// </summary>
    public partial class Subjects : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();

        int subject_id;
        Subject subject = new Subject();
        public Subjects()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            subjectDataGrid.ItemsSource = _db.Subjects.ToList();
            edit_subject_btn.Visibility = Visibility.Hidden;
        }

        private void clean()
        {
            nameTextBox.Text = "";
            codeTextBox.Text = "";
            off_yearTextBox.Text = "";
            off_semTextBox.Text = "";
            lec_hrsTextBox.Text = "";
            tute_hrsTextBox.Text = "";
            lab_hrsTextBox.Text = "";
            eval_hrsTextBox.Text = "";
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

        private void add_subject_btn_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text == "" || codeTextBox.Text == "" || off_yearTextBox.Text == ""
                || off_semTextBox.Text == "" || off_semTextBox.Text == "" || lec_hrsTextBox.Text == ""
                || tute_hrsTextBox.Text == "" || lab_hrsTextBox.Text == "" || eval_hrsTextBox.Text == ""
            )
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
            {
                subject.name = nameTextBox.Text;
                subject.code = codeTextBox.Text;
                subject.off_year = int.Parse(off_yearTextBox.Text);
                subject.off_sem = int.Parse(off_semTextBox.Text);
                subject.lec_hrs = int.Parse(lec_hrsTextBox.Text);
                subject.tute_hrs = int.Parse(tute_hrsTextBox.Text);
                subject.lab_hrs = int.Parse(lab_hrsTextBox.Text);
                subject.eval_hrs = int.Parse(eval_hrsTextBox.Text);

                _db.Subjects.Add(subject);
                _db.SaveChanges();

                subjectDataGrid.ItemsSource = _db.Subjects.ToList();
                clean();
            }
        }

        private void updateSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (subjectDataGrid.SelectedItem as Subject).id;
            Subject subject = (from m in _db.Subjects
                                 where m.id == Id
                                 select m).Single();


            nameTextBox.Text = subject.name;
            codeTextBox.Text = subject.code;
            off_yearTextBox.Text = subject.off_year.ToString();
            off_semTextBox.Text = subject.off_sem.ToString();
            lec_hrsTextBox.Text = subject.lec_hrs.ToString();
            tute_hrsTextBox.Text = subject.tute_hrs.ToString();
            lab_hrsTextBox.Text = subject.lab_hrs.ToString();
            eval_hrsTextBox.Text = subject.eval_hrs.ToString();

            subject_id = subject.id;

            edit_subject_btn.Visibility = Visibility.Visible;
            add_subject_btn.Visibility = Visibility.Hidden;
        }
        private void deleteSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            //int Id = (workingDaysPerWeekDataGrid.SelectedItem as Day).id;
            int Id = (subjectDataGrid.SelectedItem as Subject).id;
            var deletedsubject = _db.Subjects.Where(m => m.id == Id).Single();
            _db.Subjects.Remove(deletedsubject);
            _db.SaveChanges();
            subjectDataGrid.ItemsSource = _db.Subjects.ToList();

        }

        private void edit_subject_btn_Click(object sender, RoutedEventArgs e)
        {
            Subject subject = (from m in _db.Subjects
                                 where m.id == subject_id
                                 select m).Single();


            if (nameTextBox.Text == "" || codeTextBox.Text == "" || off_yearTextBox.Text == ""
                || off_semTextBox.Text == "" || off_semTextBox.Text == "" || lec_hrsTextBox.Text == ""
                || tute_hrsTextBox.Text == "" || lab_hrsTextBox.Text == "" || eval_hrsTextBox.Text == ""
            )
            {
                MaterialMessageBox.ShowError(@"Please Enter All the fields");
            }
            else
            {
                subject.name = nameTextBox.Text;
                subject.code = codeTextBox.Text;
                subject.off_year = int.Parse(off_yearTextBox.Text);
                subject.off_sem = int.Parse(off_semTextBox.Text);
                subject.lec_hrs = int.Parse(lec_hrsTextBox.Text);
                subject.tute_hrs = int.Parse(tute_hrsTextBox.Text);
                subject.lab_hrs = int.Parse(lab_hrsTextBox.Text);
                subject.eval_hrs = int.Parse(eval_hrsTextBox.Text);

                _db.SaveChanges();
                subjectDataGrid.ItemsSource = _db.Subjects.ToList();
                clean();

                edit_subject_btn.Visibility = Visibility.Hidden;
                add_subject_btn.Visibility = Visibility.Visible;
            }
            
        }
    }
}
