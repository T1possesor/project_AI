using System;
using System.Drawing;
using System.Windows.Forms;

namespace project_AI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgvMain;
        private RichTextBox rtbChat;
        private TextBox txtQuery;
        private Button btnSend;

        private Button btnModeBooks;
        private Button btnModeAuthors;
        private Button btnModeCategories;

        private Button btnShowBooksBySelectedAuthor;
        private Button btnShowBooksBySelectedCategory;

        private Label lblMode;
        private Label lblInfo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvMain = new DataGridView();
            this.rtbChat = new RichTextBox();
            this.txtQuery = new TextBox();
            this.btnSend = new Button();

            this.btnModeBooks = new Button();
            this.btnModeAuthors = new Button();
            this.btnModeCategories = new Button();

            this.btnShowBooksBySelectedAuthor = new Button();
            this.btnShowBooksBySelectedCategory = new Button();

            this.lblMode = new Label();
            this.lblInfo = new Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.SuspendLayout();

            // ===== Form =====
            this.ClientSize = new Size(1100, 650);
            this.MinimumSize = new Size(1000, 600);
            this.Text = "AI Pattern Book Search (Browse + Chat)";

            // ===== lblMode =====
            this.lblMode.Location = new Point(12, 10);
            this.lblMode.Size = new Size(500, 20);
            this.lblMode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblMode.Text = "CHẾ ĐỘ: SÁCH";

            // ===== lblInfo =====
            this.lblInfo.Location = new Point(520, 10);
            this.lblInfo.Size = new Size(560, 20);
            this.lblInfo.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.lblInfo.TextAlign = ContentAlignment.MiddleRight;
            this.lblInfo.Text = "";

            // ===== Mode buttons =====
            this.btnModeBooks.Location = new Point(12, 40);
            this.btnModeBooks.Size = new Size(110, 30);
            this.btnModeBooks.Text = "Sách";
            this.btnModeBooks.Click += new EventHandler(this.btnModeBooks_Click);

            this.btnModeAuthors.Location = new Point(130, 40);
            this.btnModeAuthors.Size = new Size(110, 30);
            this.btnModeAuthors.Text = "Tác giả";
            this.btnModeAuthors.Click += new EventHandler(this.btnModeAuthors_Click);

            this.btnModeCategories.Location = new Point(248, 40);
            this.btnModeCategories.Size = new Size(110, 30);
            this.btnModeCategories.Text = "Thể loại";
            this.btnModeCategories.Click += new EventHandler(this.btnModeCategories_Click);

            // ===== Action buttons =====
            this.btnShowBooksBySelectedAuthor.Location = new Point(380, 40);
            this.btnShowBooksBySelectedAuthor.Size = new Size(250, 30);
            this.btnShowBooksBySelectedAuthor.Text = "Xem sách của tác giả đã chọn";
            this.btnShowBooksBySelectedAuthor.Click += new EventHandler(this.btnShowBooksBySelectedAuthor_Click);

            this.btnShowBooksBySelectedCategory.Location = new Point(640, 40);
            this.btnShowBooksBySelectedCategory.Size = new Size(270, 30);
            this.btnShowBooksBySelectedCategory.Text = "Xem sách theo thể loại đã chọn";
            this.btnShowBooksBySelectedCategory.Click += new EventHandler(this.btnShowBooksBySelectedCategory_Click);

            // ===== dgvMain =====
            this.dgvMain.Location = new Point(12, 80);
            this.dgvMain.Size = new Size(1068, 380);
            this.dgvMain.ReadOnly = true;
            this.dgvMain.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvMain.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvMain_CellDoubleClick);

            // ===== rtbChat =====
            this.rtbChat.Location = new Point(12, 470);
            this.rtbChat.Size = new Size(1068, 120);
            this.rtbChat.ReadOnly = true;
            this.rtbChat.Font = new Font("Consolas", 10F);
            this.rtbChat.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // ===== txtQuery =====
            this.txtQuery.Location = new Point(12, 600);
            this.txtQuery.Size = new Size(920, 27);
            this.txtQuery.Font = new Font("Segoe UI", 11F);
            this.txtQuery.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // ===== btnSend =====
            this.btnSend.Location = new Point(945, 600);
            this.btnSend.Size = new Size(135, 27);
            this.btnSend.Text = "Gửi";
            this.btnSend.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnSend.Click += new EventHandler(this.btnSend_Click);

            // ===== add controls =====
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.lblInfo);

            this.Controls.Add(this.btnModeBooks);
            this.Controls.Add(this.btnModeAuthors);
            this.Controls.Add(this.btnModeCategories);

            this.Controls.Add(this.btnShowBooksBySelectedAuthor);
            this.Controls.Add(this.btnShowBooksBySelectedCategory);

            this.Controls.Add(this.dgvMain);
            this.Controls.Add(this.rtbChat);
            this.Controls.Add(this.txtQuery);
            this.Controls.Add(this.btnSend);

            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
