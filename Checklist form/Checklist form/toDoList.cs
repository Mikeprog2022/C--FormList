using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checklist_form
{
    public partial class ToDoList : Form
    {
        public ToDoList()
        {
            InitializeComponent();
        }
            
        DataTable todoList = new DataTable();
        bool isEditing = false;

        private async void ToDoList_Load(object sender, EventArgs e)
        {
            //Create Columns
            todoList.Columns.Add("Title");
            todoList.Columns.Add("Description");

            //Point datagridview to datasource
            toDoListView.DataSource = todoList;

            await checkData();
        }

        private async void toDoListView_Click(object sender, EventArgs e)
        {
            await checkData();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                if (string.IsNullOrEmpty(titleTextBox.Text) || string.IsNullOrEmpty(descriptionTextBox.Text))
                {
                    MessageBox.Show("Cant Save empty data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                todoList.Rows[toDoListView.CurrentCell.RowIndex]["Title"] = titleTextBox.Text;
                todoList.Rows[toDoListView.CurrentCell.RowIndex]["Description"] = descriptionTextBox.Text;
            }
            else
            {
                if (string.IsNullOrEmpty(titleTextBox.Text) || string.IsNullOrEmpty(descriptionTextBox.Text))
                {
                    MessageBox.Show("Cant Save empty data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                todoList.Rows.Add(titleTextBox.Text, descriptionTextBox.Text);
                MessageBox.Show("Data saved successfully!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            titleTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;
            isEditing = false;
        }

        private async void newButton_Click(object sender, EventArgs e)
        {
            var dataCheck = await checkData();

            if (!string.IsNullOrEmpty(titleTextBox.Text) || !string.IsNullOrEmpty(descriptionTextBox.Text))
            {
                var confirmation = await discardItem(false);
                if (!confirmation) { return; }
            }

            titleTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;
        }

        private async void editButton_Click(object sender, EventArgs e)
        {
            var dataCheck = await checkData();

            if (!dataCheck)
            {
                return;
            }

            isEditing = true;

            //Fill text fields wiht table data
            titleTextBox.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[0].ToString();
            descriptionTextBox.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[1].ToString(); ;
        }

        private async void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var confirmation = await discardItem(true);
                if (!confirmation) {  return; }
                todoList.Rows[toDoListView.CurrentCell.RowIndex].Delete();

                var dataCheck = await checkData();

                if (!dataCheck)
                {
                    return;
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task<bool> checkData()
        {
            var data = todoList.Rows.Count;

            if (data > 0)
            {
                deleteButton.Visible = true;
                editButton.Visible = true;
                return true;
            }
            else
            {
                deleteButton.Visible = false;
                editButton.Visible = false;
                return false;
            }
        }

        private async Task<bool> discardItem(bool x)
        {
            DialogResult result = new DialogResult();

            if (x)
            {
                result = MessageBox.Show("Confirm deletion", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                result = MessageBox.Show("This will discard your changes?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;                
            }

        }
    }
}
