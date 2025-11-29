using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormsApp2.src.Controllers;
using WinFormsApp2.src.Services;
using WinFormsApp2.src.Models;
using WinFormsApp2.src.Models.Towers;
using WinFormsApp2.src.Models.Enemies;
using WinFormsApp2.src.Interfaces;

namespace WinFormsApp2
{
    public class Form1 : Form
    {
        private TowerController towerController = new TowerController();

        private const int SatirSayisi = 30;
        private const int SutunSayisi = 50;
        private const int KareBoyutu = 20;

        public static string[] mapRows;
        private char[,] harita = new char[SatirSayisi, SutunSayisi];

        private Panel popupPanel;
        private int popupX, popupY;

        private Image archerImg;
        private Image cannonImg;
        private Image iceImg;

        private NotificationManager notifier;

        private EnemyController enemyController;

        private System.Windows.Forms.Timer gameTimer;

        public static int money = 100;
        public static int health = 100;
        public static int wave = 1;

        public Form1()
        {
            this.Width = SutunSayisi * KareBoyutu + 40;
            this.Height = SatirSayisi * KareBoyutu + 60;
            this.Text = "30x50 Harita - Kule Seçimi";
            this.DoubleBuffered = true;

            this.Paint += Form1_Paint;
            this.MouseClick += Form1_MouseClick;

            notifier = new NotificationManager(this);

            HaritaOlustur();

            enemyController = new EnemyController(mapRows);

            // PNG'leri yükle
            string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            archerImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "archer.png"));
            cannonImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "cannon.png"));
            iceImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "ice_tower.png"));

            // Pop-up panel oluştur
            popupPanel = new Panel();
            popupPanel.Size = new Size(100, 140);
            popupPanel.BackColor = Color.LightGray;
            popupPanel.BorderStyle = BorderStyle.FixedSingle;
            popupPanel.Visible = false;
            this.Controls.Add(popupPanel);

            // Başlık
            Label title = new Label();
            title.Text = "Kule Ekle";
            title.Font = new Font("Arial", 10, FontStyle.Bold);
            title.AutoSize = false;
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Size = new Size(popupPanel.Width, 25);
            title.Location = new Point(0, 0);
            popupPanel.Controls.Add(title);

            int btnWidth = 80;
            int btnHeight = 30;
            int spacing = 5;

            // Okçu buton
            Button okcuBtn = new Button();
            okcuBtn.Text = "Okçu";
            okcuBtn.BackColor = Color.Black;
            okcuBtn.ForeColor = Color.White;
            okcuBtn.Size = new Size(btnWidth, btnHeight);
            okcuBtn.Location = new Point((popupPanel.Width - btnWidth) / 2, title.Bottom + spacing);
            okcuBtn.Click += (s, e) =>
            {
                Response response = towerController.AddTower(new ArcherTower(popupX, popupY));
                notifier.ShowToast(response.message);
                if (response.success)
                    harita[popupY, popupX] = 'A';
                popupPanel.Visible = false;
                Invalidate();
            };
            popupPanel.Controls.Add(okcuBtn);

            // Topçu buton
            Button topcuBtn = new Button();
            topcuBtn.Text = "Topçu";
            topcuBtn.BackColor = Color.Gray;
            topcuBtn.ForeColor = Color.White;
            topcuBtn.Size = new Size(btnWidth, btnHeight);
            topcuBtn.Location = new Point((popupPanel.Width - btnWidth) / 2, okcuBtn.Bottom + spacing);
            topcuBtn.Click += (s, e) =>
            {
                Response response = towerController.AddTower(new CannonTower(popupX, popupY));
                notifier.ShowToast(response.message);
                if (response.success)
                    harita[popupY, popupX] = 'T';
                popupPanel.Visible = false;
                Invalidate();
            };
            popupPanel.Controls.Add(topcuBtn);

            // Buz buton
            Button buzBtn = new Button();
            buzBtn.Text = "Buz";
            buzBtn.BackColor = Color.CornflowerBlue;
            buzBtn.ForeColor = Color.White;
            buzBtn.Size = new Size(btnWidth, btnHeight);
            buzBtn.Location = new Point((popupPanel.Width - btnWidth) / 2, topcuBtn.Bottom + spacing);
            buzBtn.Click += (s, e) =>
            {
                Response response = towerController.AddTower(new IceTower(popupX, popupY));
                notifier.ShowToast(response.message);
                if (response.success)
                    harita[popupY, popupX] = 'B';
                popupPanel.Visible = false;
                Invalidate();
            };
            popupPanel.Controls.Add(buzBtn);

            popupPanel.Height = buzBtn.Bottom + spacing;

            // Başlangıç düşmanı spawn
            SpawnArmoredEnemy();

            // Timer başlat
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 150; // Hızı ayarla
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void HaritaOlustur()
        {
            mapRows = new string[]
            {
                "YYYYYYYYYYYXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "YYYYYYYYYYYXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "YYYYYYYYYYYXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOOOOOOOOOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOOOOOOOOOOOOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "SOOOOOOOOOOOOOOOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOOOOOOOOOOOOOOO9",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
            };

            for (int y = 0; y < SatirSayisi; y++)
                for (int x = 0; x < SutunSayisi; x++)
                    harita[y, x] = mapRows[y][x];
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / KareBoyutu;
            int y = e.Y / KareBoyutu;

            if (x >= 0 && x < SutunSayisi && y >= 0 && y < SatirSayisi)
            {
                if (harita[y, x] == 'X') // Sadece yeşil kareler
                {
                    popupX = x;
                    popupY = y;
                    popupPanel.Location = new Point(x * KareBoyutu, y * KareBoyutu);
                    popupPanel.Visible = true;
                }
                else popupPanel.Visible = false;
            }
        }

        private void SpawnArmoredEnemy()
        {
            int startX = 0;
            int startY = 0;

            for (int y = 0; y < mapRows.Length; y++)
            {
                for (int x = 0; x < mapRows[y].Length; x++)
                {
                    if (mapRows[y][x] == 'S')
                    {
                        startX = x;
                        startY = y;
                        break;
                    }
                }
            }

            enemyController.AddEnemy(new ArmoredEnemy
            {
                X = startX,
                Y = startY
            });
        }

        private void GameLoop(object sender, EventArgs e)
        {
            foreach (var enemy in enemyController.Enemies.ToList())
            {
                foreach (var tower in towerController.Towers)
                {
                    int dx = enemy.X - tower.X;
                    int dy = enemy.Y - tower.Y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    if (distance <= tower.Range)
                    {
                        enemyController.TakeDamage(enemy, tower.Damage);
                    }
                }

                if (enemy.IsDead)
                {
                    enemyController.Enemies.Remove(enemy);
                }
            }

            enemyController.UpdateAll();

            Invalidate();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Harita çizimi
            for (int y = 0; y < SatirSayisi; y++)
            {
                for (int x = 0; x < SutunSayisi; x++)
                {
                    if (harita[y, x] == 'Y') continue;
                    Rectangle kare = new Rectangle(x * KareBoyutu, y * KareBoyutu, KareBoyutu, KareBoyutu);
                    switch (harita[y, x])
                    {
                        case 'O': g.FillRectangle(Brushes.SandyBrown, kare); break;
                        default: g.FillRectangle(Brushes.LightGreen, kare); break;
                    }
                    g.DrawRectangle(Pens.Black, kare);
                }
            }

            // Kuleleri PNG ile çiz
            for (int y = 0; y < SatirSayisi; y++)
            {
                for (int x = 0; x < SutunSayisi; x++)
                {
                    char c = harita[y, x];
                    if (c == 'A')
                    {
                        DrawTowerImage(g, archerImg, x, y);
                        CizMenzil(g, x, y, Color.FromArgb(80, Color.Red));
                    }
                    else if (c == 'T')
                    {
                        DrawTowerImage(g, cannonImg, x, y);
                        CizMenzil(g, x, y, Color.FromArgb(80, Color.Gray));
                    }
                    else if (c == 'B')
                    {
                        DrawTowerImage(g, iceImg, x, y);
                        CizMenzil(g, x, y, Color.FromArgb(80, Color.CornflowerBlue));
                    }
                }
            }

            // Düşmanları çiz
            foreach (var enemy in enemyController.Enemies)
            {
                g.FillEllipse(Brushes.DarkGray, enemy.X * KareBoyutu, enemy.Y * KareBoyutu, KareBoyutu, KareBoyutu);
                g.DrawString(enemy.Health.ToString("0"), new Font("Arial", 8), Brushes.White, enemy.X * KareBoyutu, enemy.Y * KareBoyutu - 12);
            }

            // HUD
            int hudWidth = 11 * KareBoyutu;
            int hudHeight = 3 * KareBoyutu;
            g.FillRectangle(Brushes.LightGray, new Rectangle(0, 0, hudWidth, hudHeight));
            Font hudFont = new Font("Arial", 10, FontStyle.Bold);
            Brush hudBrush = Brushes.Black;
            g.DrawString($"Para: {money}", hudFont, hudBrush, 2, 2);
            g.DrawString($"Can: {health}", hudFont, hudBrush, 2, 18);
            g.DrawString($"Dalga: {wave}", hudFont, hudBrush, 2, 34);
        }

        private void DrawTowerImage(Graphics g, Image img, int x, int y)
        {
            int imgSize = KareBoyutu * 3;
            int posX = x * KareBoyutu + (KareBoyutu - imgSize) / 2;
            int posY = y * KareBoyutu + (KareBoyutu - imgSize) / 2;

            g.DrawImage(img, posX, posY, imgSize, imgSize);
        }

        private void CizMenzil(Graphics g, int kuleX, int kuleY, Color renk)
        {
            int radius = 4 * KareBoyutu;
            int centerX = kuleX * KareBoyutu + KareBoyutu / 2;
            int centerY = kuleY * KareBoyutu + KareBoyutu / 2;
            SolidBrush brush = new SolidBrush(renk);
            g.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
        }
    }
}
