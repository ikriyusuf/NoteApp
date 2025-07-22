using NoteApp.Entities;
using NoteApp.JWT;
using NoteApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Windows.Forms;

namespace NoteApp.Forms
{
    public partial class FormMain : Form
    {
        private int selectedNoteId = -1;
        private readonly int _userId;
        private readonly string _username;

        public FormMain(string token)
        {
            InitializeComponent();

            TokenValidator tokenValidator = new TokenValidator();
            var principal = tokenValidator.ValidateJwtToken(token);

            if (principal != null)
            {
                _username = principal.FindFirst("username")?.Value;

                string idString = principal.FindFirst("id")?.Value;
                _userId = int.TryParse(idString, out int parsedId) ? parsedId : -1;
            }
            else
            {
                MessageBox.Show("Token geçersiz!");
                this.Close();
            }
            this.Text = $"Not Uygulaması - Kullanıcı: {_username}";
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
                UserId = _userId
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
            var notes = noteService.GetAllNotes(_userId);
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
                UserId = _userId,
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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Çıkış yapmak istediğinize emin misiniz?", "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ClearFields();
                FormLogin loginForm = new FormLogin();
                loginForm.Show();
                this.Close();
            }
        }
    }
}
