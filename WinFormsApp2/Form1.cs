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
using System.Media;


namespace WinFormsApp2
{
    public class Form1 : Form
    {
        private TowerController towerController = new TowerController();

        private const int SatirSayisi = 30;
        private const int SutunSayisi = 50;
        private const int KareBoyutu = 25;

        public static string[] mapRows;
        private char[,] harita = new char[SatirSayisi, SutunSayisi];

        private Panel popupPanel;
        private int popupX, popupY;

        private Image archerImg;
        private Image cannonImg;
        private Image iceImg;
        private Image armoredImg;
        private Image flyingImg;
        private Image standardImg;
        private Image grassImg;
        private Image rockImg;
        private Image treeImg;
        private Image mainCastleImg;

        private SoundPlayer player;

        private NotificationManager notifier;

        private EnemyController enemyController;

        private System.Windows.Forms.Timer gameTimer;

        public static int money = 200;
        public static int health = 100;
        public static int wave = 1;

        // ✅ Buraya ekle:
        private List<Wave> waves;          // Dalga listesi
        private int currentWaveIndex = 0;  // Hangi dalgadayız

        // Dalga sınıfı
        public class Wave
        {
            public int ArmoredCount;
            public int FlyingCount;
            public int StandardCount;
            public int SpawnIntervalMs;
        }

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
            armoredImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "armored.png"));
            flyingImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "flying.png"));
            standardImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "standard.png"));
            treeImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "tree.png"));
            rockImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "rock.png"));
            grassImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "grass.png"));
            mainCastleImg = Image.FromFile(Path.Combine(basePath, "src", "Assets", "Images", "main_castle.png"));
            string musicPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName,"src", "Assets", "Music", "music.wav");

            player = new SoundPlayer(musicPath);
            player.Load();       // Müziği yükle
            player.PlayLooping(); // Sonsuz döngüde çal

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
                    harita[popupY, popupX] = 'C';
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
                    harita[popupY, popupX] = 'I';
                popupPanel.Visible = false;
                Invalidate();
            };
            popupPanel.Controls.Add(buzBtn);

            popupPanel.Height = buzBtn.Bottom + spacing;

            // DALGALARIN TANIMLANMASI
            waves = new List<Wave>
        {
            new Wave { ArmoredCount = 2, FlyingCount = 1, StandardCount = 3, SpawnIntervalMs = 1000 },
            new Wave { ArmoredCount = 3, FlyingCount = 2, StandardCount = 5, SpawnIntervalMs = 1000 }
            };

            StartWaves(); // Dalga mekaniklerini başlat

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
                "YYYYYYYYYYYXXXXXXXXXXXXXXXXXXXXGXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXTXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOOOOOOOOOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXGXXOXXXXXXXOXXXXXXXXXXRXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXRXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOOOOOOOOOOOOXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXTXXXXXXXOXXXXXGXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXRXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXRXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXTXXXXX",
                "XXXXXXXXXGXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXRXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXOXXXXXXXXXXXXXXXXXXOXXXXXXXGXXXXXXX",
                "SOOOOOOOOOOOOOOOXXXXTXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXRXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXTXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXOXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXRXXXXXXXXXXXXXXXXXXOOOOOOOOOOOOOOOE",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXTXXXXXXX",
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

        private async Task SpawnWaveAsync(Wave wave)
        {
            // Armored düşmanları spawn et
            for (int i = 0; i < wave.ArmoredCount; i++)
            {
                SpawnArmoredEnemy();
                await Task.Delay(wave.SpawnIntervalMs);
            }

            // Flying düşmanları spawn et
            for (int i = 0; i < wave.FlyingCount; i++)
            {
                SpawnFlyingEnemy();
                await Task.Delay(wave.SpawnIntervalMs);
            }

            // Standard düşmanları spawn et
            for (int i = 0; i < wave.StandardCount; i++)
            {
                SpawnStandardEnemy();
                await Task.Delay(wave.SpawnIntervalMs);
            }
        }

        private async void StartWaves()
        {
            while (currentWaveIndex < waves.Count)
            {
                Wave currentWave = waves[currentWaveIndex];
                wave = currentWaveIndex + 1; // HUD için dalga numarası

                await SpawnWaveAsync(currentWave);

                // Tüm düşmanlar öldükten sonra bir sonraki dalgaya geç
                while (enemyController.Enemies.Count > 0)
                {
                    await Task.Delay(500); // Küçük bekleme, game loop devam eder
                }

                currentWaveIndex++;
            }

            await Task.Delay(1000); // Son düşman animasyonu için küçük bekleme

            if (Form1.health > 0)
            {
                MessageBox.Show("Tebrikler! Oyunu kazandınız!", "Kazandın", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogsManager.Log($"OYUNU KAZANDINIZ. Kalan can: {Form1.health}, Toplam para: {Form1.money}");
            }
            else
            {
                MessageBox.Show("Oyunu kaybettiniz!", "Kaybettin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogsManager.Log($"OYUNU KAYBETTİNİZ. Kalan can: {Form1.health}, Toplam para: {Form1.money}");
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

        private void SpawnFlyingEnemy()
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

            enemyController.AddEnemy(new FlyingEnemy
            {
                X = startX,
                Y = startY
            });
        }

        private void SpawnStandardEnemy()
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

            enemyController.AddEnemy(new StandardEnemy
            {
                X = startX,
                Y = startY
            });
        }

        private int goalX;
        private int goalY;
        private bool goalFound = false;

        private void GameLoop(object sender, EventArgs e)
        {
            // 1) Hedef noktayı bul
            if (!goalFound)
            {
                for (int y = 0; y < mapRows.Length; y++)
                {
                    for (int x = 0; x < mapRows[y].Length; x++)
                    {
                        if (mapRows[y][x] == 'E')
                        {
                            goalX = x;
                            goalY = y;
                            goalFound = true;
                            break;
                        }
                    }
                    if (goalFound) break;
                }
            }

            // 2) Düşmanları ilerlet
            enemyController.UpdateAll();

            // 3) Düşmanları slow süresi için update et
            foreach (var enemy in enemyController.Enemies)
            {
                if (enemy.IsSlowed)
                {
                    enemy.SlowTimer -= 0.15f; // gameTimer.Interval = 150ms
                    if (enemy.SlowTimer <= 0)
                    {
                        enemy.IsSlowed = false;
                        enemy.Speed = enemy.OriginalSpeed;
                    }
                }
            }

            // 4) Kulelerin saldırısı
            foreach (var tower in towerController.Towers)
            {
                tower.Attack(enemyController.Enemies);

                // Cooldown update
                if (tower.FireCooldown > 0)
                    tower.FireCooldown -= 0.15f; // interval ile uyumlu
                if (tower.FireCooldown < 0) tower.FireCooldown = 0;
            }

            // 5) Düşmanları tek tek kontrol et
            foreach (var enemy in enemyController.Enemies.ToList())
            {
                // Hedefe ulaştı mı?
                double distToGoal = Math.Sqrt(Math.Pow(enemy.X - goalX, 2) + Math.Pow(enemy.Y - goalY, 2));
                if (distToGoal < 0.3)
                {
                    Form1.health -= enemy.DamageToBase;
                    notifier.ShowToast($"Base hasar aldı! (-{enemy.DamageToBase})");
                    enemyController.Enemies.Remove(enemy);
                    continue;
                }

                // Öldüyse para ver
                if (enemy.IsDead)
                {
                    Form1.money += enemy.RewardOnDeath;
                    notifier.ShowToast($"+{enemy.RewardOnDeath} para kazandın!");
                    LogsManager.Log($"{enemy.Type} düşmanı öldü! +{enemy.RewardOnDeath} para kazanıldı. Kalan can: {Form1.health}, Toplam para: {Form1.money}");
                    enemyController.Enemies.Remove(enemy);
                }
            }

            // 6) Ekranı güncelle
            Invalidate();

            // 7) Can bitti mi?
            if (Form1.health <= 0)
            {
                gameTimer.Stop();
                MessageBox.Show("Oyunu kaybettiniz!", "Kaybettin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // veya Restart logic
                return;
            }
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Harita çizimi
            for (int y = 0; y < SatirSayisi; y++)
            {
                for (int x = 0; x < SutunSayisi; x++)
                {
                    Rectangle kare = new Rectangle(x * KareBoyutu, y * KareBoyutu, KareBoyutu, KareBoyutu);

                    switch (harita[y, x])
                    {
                        case 'O': g.FillRectangle(Brushes.SandyBrown, kare); break;
                        case 'S': g.FillRectangle(Brushes.Chartreuse, kare); break;
                        case 'E': g.FillRectangle(Brushes.Crimson, kare); break;
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
                    else if (c == 'C')
                    {
                        DrawTowerImage(g, cannonImg, x, y);
                        CizMenzil(g, x, y, Color.FromArgb(80, Color.Gray));
                    }
                    else if (c == 'I')
                    {
                        DrawTowerImage(g, iceImg, x, y);
                        CizMenzil(g, x, y, Color.FromArgb(80, Color.CornflowerBlue));
                    }
                }
            }

            for (int y = 0; y < SatirSayisi; y++)
            {
                for (int x = 0; x < SutunSayisi; x++)
                {
                    

                    
                    char c = harita[y, x];
                    if (c == 'G')
                    {
                        int imgSize = KareBoyutu * 1;
                        int posX = x * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        int posY = y * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        g.DrawImage(grassImg, posX, posY, imgSize, imgSize);
                    }
                    else if (c == 'T')
                    {
                        int imgSize = (int)(KareBoyutu * 1.5);
                        int posX = x * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        int posY = y * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        g.DrawImage(treeImg, posX, posY, imgSize, imgSize);

                    }
                    else if (c == 'R')
                    {
                        int imgSize = (int)(KareBoyutu * 1.5);
                        int posX = x * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        int posY = y * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        g.DrawImage(rockImg, posX, posY, imgSize, imgSize);

                    }
                    else if (c == 'E')
                    {
                        int imgSize = (int)(KareBoyutu * 2);
                        int posX = x * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        int posY = y * KareBoyutu + (KareBoyutu - imgSize) / 2;
                        g.DrawImage(mainCastleImg, posX, posY, imgSize, imgSize);

                    }
                }
            }

            // Düşmanları çiz
            foreach (var enemy in enemyController.Enemies)
            {
                Image img = enemy switch
                {
                    ArmoredEnemy => armoredImg,
                    FlyingEnemy => flyingImg,
                    StandardEnemy => standardImg,
                    _ => null
                };

                if (img != null)
                    g.DrawImage(img, enemy.X * KareBoyutu, enemy.Y * KareBoyutu, KareBoyutu, KareBoyutu);

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
            int imgSize = KareBoyutu * 2;
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
