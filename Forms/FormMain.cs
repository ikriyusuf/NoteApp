using NoteApp.Entities;
using NoteApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NoteApp.Forms
{
    public partial class FormMain : Form
    {
        private int selectedNoteId = -1;
        public FormMain()
        {
            InitializeComponent();
        }
        NoteService noteService = new NoteService();
        private void FormMain_Load(object sender, EventArgs e)
        {
            timerClock.Start();
            LoadNotes();
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string priority = null;

            var title = txtTitle.Text.Trim();
            var content = txtContent.Text.Trim();

            List<RadioButton> priorityButtons = new List<RadioButton> { rdrbtnD, rdrbtnO, rdrbtnY };

            foreach (var button in priorityButtons)
            {
                if (button.Checked)
                {
                    priority = button.Text;
                    break;
                }
            }

            int userid = 1;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || priorityButtons.All(rb => !rb.Checked))
            {
                lblStatus.Text = "Başlık, içerik ve öncelik seçimi zorunludur.";
                lblStatus.ForeColor = Color.Red;
                return;
            }
            var note = new Note
            {
                Title = title,
                Content = content,
                Priority = priority,
                UserId = userid
            };
            if (noteService.CreateNote(note))
            {
                lblStatus.Text = "Not başarıyla oluşturuldu.";
                lblStatus.ForeColor = Color.Green;
                ClearFields();
            }
            else
            {
                lblStatus.Text = "Not oluşturulurken bir hata oluştu.";
                lblStatus.ForeColor = Color.Red;
            }
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            LoadNotes();
        }

        public void LoadNotes()
        {
            var notes = noteService.GetAllNotes();
            dataGridView1.DataSource = notes;
            dataGridView1.Columns["Id"].Visible = false;
        }


        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                selectedNoteId = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                txtTitle.Text = selectedRow.Cells["Title"].Value.ToString();
                txtContent.Text = selectedRow.Cells["Content"].Value.ToString();
                string priority = selectedRow.Cells["Priority"].Value.ToString();
                if (priority == "Düşük")
                {
                    rdrbtnD.Checked = true;
                }
                else if (priority == "Orta")
                {
                    rdrbtnO.Checked = true;
                }
                else if (priority == "Yüksek")
                {
                    rdrbtnY.Checked = true;
                }
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            string priority = null;

            var title = txtTitle.Text.Trim();
            var content = txtContent.Text.Trim();

            List<RadioButton> priorityButtons = new List<RadioButton> { rdrbtnD, rdrbtnO, rdrbtnY };

            foreach (var button in priorityButtons)
            {
                if (button.Checked)
                {
                    priority = button.Text;
                    break;
                }
            }

            int userid = 1;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content) || priorityButtons.All(rb => !rb.Checked))
            {
                lblStatus.Text = "Başlık, içerik ve öncelik seçimi zorunludur.";
                lblStatus.ForeColor = Color.Red;
                return;
            }
            var note = new Note
            {
                Id = selectedNoteId,
                Title = title,
                Content = content,
                Priority = priority,
                UserId = userid,
            };
            if (noteService.UpdateNote(note))
            {
                lblStatus.Text = "Not başarıyla güncellendi.";
                lblStatus.ForeColor = Color.Green;
                ClearFields();
            }
            else
            {
                lblStatus.Text = "Not güncellenirken bir hata oluştu.";
                lblStatus.ForeColor = Color.Red;
            }
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            LoadNotes();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            noteService.DeleteNote(selectedNoteId);
            lblStatus.Text = "Not başarı ile silindi.";
            lblStatus.ForeColor = Color.Green;
            ClearFields();
            LoadNotes();
        }

        public void ClearFields()
        {
            txtTitle.Clear();
            txtContent.Clear();
            rdrbtnD.Checked = false;
            rdrbtnO.Checked = false;
            rdrbtnY.Checked = false;
            selectedNoteId = -1;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            lblStatus.Text = string.Empty;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            LoadNotes();
        }
    }
}
