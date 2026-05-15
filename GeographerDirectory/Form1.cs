using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using GeographerDirectory.Models;
using GeographerDirectory.Storage;
using Region = GeographerDirectory.Models.Region;

namespace GeographerDirectory
{
    public partial class Form1 : Form
    {
        private TreeView treeView;
        private Panel detailsPanel;
        private Button btnAddContinent, btnAddChild, btnDelete, btnSave, btnLoad, btnShowOnMap;

        private DataManager dataManager;
        private List<Continent> continents;

        // --- РАДИКАЛЬНА ТЕМНА ТЕМА (DARK MODE) ---
        private Color bgPrimary = Color.FromArgb(30, 30, 46);     // Глибокий темний для панелі зліва
        private Color bgSecondary = Color.FromArgb(40, 42, 54);   // Трохи світліший темний для основної панелі
        private Color inputBg = Color.FromArgb(50, 52, 70);       // Фон для полів вводу тексту
        private Color textLight = Color.FromArgb(248, 248, 242);  // Основний білий текст
        private Color textMuted = Color.FromArgb(160, 160, 175);  // Сіруватий текст для підписів
        private Color accentBlue = Color.FromArgb(59, 130, 246);  // Яскравий синій для виділення

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUI();

            dataManager = new DataManager();
            continents = new List<Continent>();

            btnAddContinent.Click += (s, e) => { continents.Add(new Continent("Новий Материк")); UpdateTreeView(); };
            btnAddChild.Click += BtnAddChild_Click;
            btnDelete.Click += BtnDelete_Click;
            btnShowOnMap.Click += BtnShowOnMap_Click;
            btnSave.Click += BtnSave_Click;
            btnLoad.Click += BtnLoad_Click;

            treeView.AfterSelect += (s, e) => ShowDetails(e.Node.Tag);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON файли (*.json)|*.json|Всі файли (*.*)|*.*";
                sfd.Title = "Зберегти довідник як...";
                sfd.FileName = "Мій_Довідник.json";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    dataManager.SaveData(continents, sfd.FileName);
                    MessageBox.Show("Дані успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON файли (*.json)|*.json|Всі файли (*.*)|*.*";
                ofd.Title = "Відкрити довідник";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    continents = dataManager.LoadData(ofd.FileName);
                    UpdateTreeView();
                }
            }
        }

        private void ShowDetails(object obj)
        {
            detailsPanel.Controls.Clear();
            if (obj == null) return;

            int y = 20;

            if (obj is GeographicObject g)
            {
                Label titleLbl = new Label
                {
                    Text = g.Name,
                    Location = new Point(20, y),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 20, FontStyle.Bold),
                    ForeColor = textLight
                };
                detailsPanel.Controls.Add(titleLbl);

                Panel separator = new Panel { BackColor = Color.FromArgb(68, 71, 90), Location = new Point(20, y + 40), Size = new Size(350, 2) };
                detailsPanel.Controls.Add(separator);

                y += 65;
            }

            // Розумна функція додавання полів з ВАЛІДАЦІЄЮ
            void AddField(string labelText, string value, Func<string, bool> onUpdate)
            {
                Label lbl = new Label { Text = labelText, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = textMuted };
                TextBox txt = new TextBox
                {
                    Text = value,
                    Location = new Point(20, y + 25),
                    Width = 350,
                    Font = new Font("Segoe UI", 12),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = inputBg,
                    ForeColor = textLight
                };

                txt.TextChanged += (s, e) => {
                    // Викликаємо перевірку даних
                    bool isValid = onUpdate(txt.Text);

                    if (isValid)
                    {
                        txt.BackColor = inputBg; // Все добре -> стандартний фон
                        if (labelText.Contains("Назва") && treeView.SelectedNode != null)
                            treeView.SelectedNode.Text = txt.Text;
                    }
                    else
                    {
                        txt.BackColor = Color.FromArgb(120, 40, 40); // Помилка -> Темно-червоний фон
                    }
                };

                detailsPanel.Controls.Add(lbl); detailsPanel.Controls.Add(txt);
                y += 70;
            }

            if (obj is GeographicObject geo)
            {
                // Валідація: Назва не може бути повністю порожньою або складатися з пробілів
                AddField("Назва:", geo.Name, v => {
                    if (string.IsNullOrWhiteSpace(v)) return false;
                    geo.Name = v;
                    return true;
                });

                // Валідація: Населення має бути числом і БІЛЬШИМ АБО РІВНИМ НУЛЮ
                AddField("Населення:", geo.Population.ToString(), v => {
                    if (long.TryParse(v, out long res) && res >= 0)
                    {
                        geo.Population = res;
                        return true;
                    }
                    return false;
                });
            }

            if (obj is Continent continent)
            {
                Button btnCalc = new Button
                {
                    Text = "📊 Підрахувати населення",
                    Location = new Point(20, y + 10),
                    Size = new Size(350, 45),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(16, 185, 129),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnCalc.FlatAppearance.BorderSize = 0;
                btnCalc.MouseEnter += (s, e) => btnCalc.BackColor = Color.FromArgb(5, 150, 105);
                btnCalc.MouseLeave += (s, e) => btnCalc.BackColor = Color.FromArgb(16, 185, 129);

                btnCalc.Click += (s, e) => {
                    long totalPopulation = 0;
                    foreach (var c in continent.Countries) totalPopulation += c.Population;
                    continent.Population = totalPopulation;
                    ShowDetails(continent);
                };
                detailsPanel.Controls.Add(btnCalc);
            }
            else if (obj is Country c)
            {
                // Валідація: Площа має бути числом >= 0
                AddField("Площа (кв. км):", c.Area.ToString(), v => {
                    if (double.TryParse(v, out double res) && res >= 0)
                    {
                        c.Area = res;
                        return true;
                    }
                    return false;
                });

                AddField("Столиця:", c.Capital, v => { c.Capital = v; return true; });
                AddField("Форма правління:", c.GovernmentForm, v => { c.GovernmentForm = v; return true; });
            }
            else if (obj is Region r)
            {
                AddField("Вид (область/штат):", r.Type, v => { r.Type = v; return true; });
                AddField("Адмін. центр:", r.Capital, v => { r.Capital = v; return true; });
            }
            else if (obj is City city)
            {
                AddField("Координати (Широта, Довгота):", city.Coordinates, v => { city.Coordinates = v; return true; });
            }
        }

        private void BtnShowOnMap_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode?.Tag is City city && !string.IsNullOrEmpty(city.Coordinates))
            {
                string url = $"http://google.com/maps/place/{city.Coordinates.Replace(" ", "")}";
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
            }
            else MessageBox.Show("Оберіть місто з координатами!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void BtnAddChild_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode?.Tag is Continent cont) cont.Countries.Add(new Country("Нова Країна", 0, 0, "Правління", "Столиця"));
            else if (treeView.SelectedNode?.Tag is Country country) country.Regions.Add(new Region("Новий Регіон", 0, "Область", "Центр"));
            else if (treeView.SelectedNode?.Tag is Region region) region.Cities.Add(new City("Нове Місто", 0, "50.45, 30.52"));
            UpdateTreeView();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            TreeNode parent = treeView.SelectedNode.Parent;
            if (parent == null) continents.Remove((Continent)treeView.SelectedNode.Tag);
            else if (parent.Tag is Continent cont) cont.Countries.Remove((Country)treeView.SelectedNode.Tag);
            else if (parent.Tag is Country country) country.Regions.Remove((Region)treeView.SelectedNode.Tag);
            else if (parent.Tag is Region reg) reg.Cities.Remove((City)treeView.SelectedNode.Tag);
            UpdateTreeView();
        }

        private void UpdateTreeView()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            foreach (var cont in continents)
            {
                TreeNode contNode = new TreeNode(cont.Name) { Tag = cont };
                foreach (var country in cont.Countries)
                {
                    TreeNode countryNode = new TreeNode(country.Name) { Tag = country };
                    foreach (var reg in country.Regions)
                    {
                        TreeNode regNode = new TreeNode(reg.Name) { Tag = reg };
                        foreach (var city in reg.Cities) regNode.Nodes.Add(new TreeNode(city.Name) { Tag = city });
                        countryNode.Nodes.Add(regNode);
                    }
                    contNode.Nodes.Add(countryNode);
                }
                treeView.Nodes.Add(contNode);
            }
            treeView.ExpandAll();
            treeView.EndUpdate();
            detailsPanel.Controls.Clear();
        }

        private void InitializeCustomUI()
        {
            this.Text = "Довідник географа 2.0 (Dark Edition)";
            this.Size = new Size(1150, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = bgSecondary;

            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));

            // Дерево: темний фон, білий текст
            treeView = new TreeView { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.None, BackColor = bgPrimary, ForeColor = textLight, ItemHeight = 32 };

            // Пошук: темний фон
            TextBox txtSearch = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 12), PlaceholderText = "🔍 Пошук (назва, координати)...", BorderStyle = BorderStyle.FixedSingle, BackColor = inputBg, ForeColor = textLight };
            txtSearch.TextChanged += (s, e) => {
                string searchText = txtSearch.Text.ToLower();
                void SearchNodes(TreeNodeCollection nodes)
                {
                    foreach (TreeNode node in nodes)
                    {
                        bool match = false;
                        if (!string.IsNullOrWhiteSpace(searchText) && node.Tag is GeographicObject geo)
                        {
                            string searchData = $"{geo.Name} {geo.Population} ";
                            if (geo is Country c) searchData += $"{c.Area} {c.Capital} {c.GovernmentForm}";
                            else if (geo is Region r) searchData += $"{r.Type} {r.Capital}";
                            else if (geo is City city) searchData += $"{city.Coordinates}";
                            match = searchData.ToLower().Contains(searchText);
                        }
                        // При пошуку підсвічуємо синім
                        node.BackColor = match ? accentBlue : bgPrimary;
                        node.ForeColor = textLight;
                        if (match) node.EnsureVisible();
                        SearchNodes(node.Nodes);
                    }
                }
                treeView.BeginUpdate();
                SearchNodes(treeView.Nodes);
                treeView.EndUpdate();
            };

            Panel leftPanel = new Panel { Dock = DockStyle.Fill, BackColor = bgPrimary, Padding = new Padding(15) };
            leftPanel.Controls.Add(treeView);
            leftPanel.Controls.Add(new Panel { Height = 15, Dock = DockStyle.Top, BackColor = bgPrimary });
            leftPanel.Controls.Add(txtSearch);
            treeView.BringToFront();

            detailsPanel = new Panel { Dock = DockStyle.Fill, BackColor = bgSecondary, Padding = new Padding(40) };

            TableLayoutPanel buttonPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1, Padding = new Padding(10), BackColor = Color.FromArgb(25, 25, 35) };
            for (int i = 0; i < 6; i++) buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6F));

            void StyleBtn(Button b, Color bg, Color fg, Color hoverBg)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = bg;
                b.ForeColor = fg;
                b.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                b.Dock = DockStyle.Fill;
                b.Margin = new Padding(8);
                b.Cursor = Cursors.Hand;
                b.MouseEnter += (s, e) => b.BackColor = hoverBg;
                b.MouseLeave += (s, e) => b.BackColor = bg;
            }

            // Нижні кнопки у темному стилі
            Color btnDefaultBg = Color.FromArgb(68, 71, 90);
            Color btnDefaultHover = Color.FromArgb(98, 114, 164);

            btnAddContinent = new Button { Text = "Материк" };
            StyleBtn(btnAddContinent, btnDefaultBg, textLight, btnDefaultHover);

            btnAddChild = new Button { Text = "Додати" };
            StyleBtn(btnAddChild, btnDefaultBg, textLight, btnDefaultHover);

            btnDelete = new Button { Text = "Видалити" };
            StyleBtn(btnDelete, Color.FromArgb(220, 38, 38), Color.White, Color.FromArgb(185, 28, 28)); // Неоново червоний

            btnShowOnMap = new Button { Text = "Карта" };
            StyleBtn(btnShowOnMap, btnDefaultBg, textLight, btnDefaultHover);

            btnSave = new Button { Text = "Зберегти" };
            StyleBtn(btnSave, accentBlue, Color.White, Color.FromArgb(37, 99, 235)); // Яскраво синій

            btnLoad = new Button { Text = "Відкрити" };
            StyleBtn(btnLoad, btnDefaultBg, textLight, btnDefaultHover);

            buttonPanel.Controls.Add(btnAddContinent, 0, 0);
            buttonPanel.Controls.Add(btnAddChild, 1, 0);
            buttonPanel.Controls.Add(btnDelete, 2, 0);
            buttonPanel.Controls.Add(btnShowOnMap, 3, 0);
            buttonPanel.Controls.Add(btnSave, 4, 0);
            buttonPanel.Controls.Add(btnLoad, 5, 0);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(detailsPanel, 1, 0);
            mainLayout.Controls.Add(buttonPanel, 0, 1);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }
    }
}