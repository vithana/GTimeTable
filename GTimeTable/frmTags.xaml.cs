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
    /// Interaction logic for frmTags.xaml
    /// </summary>
    public partial class frmTags : UserControl
    {
        GTimeTableEntities _db = new GTimeTableEntities();
        int tagId;
        Tag tag = new Tag();


        public frmTags()
        {
            InitializeComponent();
            Load();
        }
        
        private void Load()
        {
            tagDataGrid.ItemsSource = _db.Tags.ToList();
            button_update_save.Visibility = Visibility.Hidden;
            tag_TextBox.Focus();


        }
        private void clean()
        {
            tag_TextBox.Text = "";

        }

        private void addbtn_tags_Click(object sender, RoutedEventArgs e)
        {
            tag.tag1 = tag_TextBox.Text;

            _db.Tags.Add(tag);
            _db.SaveChanges();

            tagDataGrid.ItemsSource = _db.Tags.ToList();
            clean();
            tag_TextBox.Focus();

        }

        private void updateTagBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (tagDataGrid.SelectedItem as Tag).id;
            Tag tags = (from m in _db.Tags
                where m.id == Id
                select m).Single();
            tag_TextBox.Text = tags.tag1;
            tagId =  tags.id;
            button_update_save.Visibility = Visibility.Visible;
            addbtn_tags.Visibility = Visibility.Hidden;

        }
        private void deleteTagBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (tagDataGrid.SelectedItem as Tag).id;
            var deleteTag = _db.Tags.Where(m => m.id == Id).Single();
            _db.Tags.Remove(deleteTag);
            _db.SaveChanges();
            tagDataGrid.ItemsSource = _db.Tags.ToList();


        }

        private void button_update_save_Click(object sender, RoutedEventArgs e)
        {
            Tag tags = (from m in _db.Tags
                where m.id == tagId
                        select m).Single();

            tags.tag1 = tag_TextBox.Text;
            
            _db.SaveChanges();

            tagDataGrid.ItemsSource = _db.Tags.ToList();
            clean();
            button_update_save.Visibility = Visibility.Hidden;
            addbtn_tags.Visibility = Visibility.Visible;
        }
    }
}
