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

            // --- НОВИЙ КОД ДЛЯ КНОПОК ЗБЕРЕГТИ/ВІДКРИТИ ---
            btnSave.Click += BtnSave_Click;
            btnLoad.Click += BtnLoad_Click;

            treeView.AfterSelect += (s, e) => ShowDetails(e.Node.Tag);
        }

        // ВІКНО ЗБЕРЕЖЕННЯ
        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON файли (*.json)|*.json|Всі файли (*.*)|*.*";
                sfd.Title = "Зберегти довідник як...";
                sfd.FileName = "Мій_Довідник.json"; // Назва за замовчуванням

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    dataManager.SaveData(continents, sfd.FileName);
                    MessageBox.Show("Дані успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // ВІКНО ВІДКРИТТЯ
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
            void AddField(string labelText, string value, Action<string> onUpdate)
            {
                Label lbl = new Label { Text = labelText, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                TextBox txt = new TextBox { Text = value, Location = new Point(20, y + 20), Width = 280, Font = new Font("Segoe UI", 10) };
                txt.TextChanged += (s, e) => {
                    onUpdate(txt.Text);
                    if (labelText.Contains("Назва") && treeView.SelectedNode != null) treeView.SelectedNode.Text = txt.Text;
                };
                detailsPanel.Controls.Add(lbl); detailsPanel.Controls.Add(txt);
                y += 65;
            }

            if (obj is GeographicObject geo)
            {
                AddField("Назва:", geo.Name, v => geo.Name = v);
                AddField("Населення:", geo.Population.ToString(), v => { if (int.TryParse(v, out int res)) geo.Population = res; });
            }

            if (obj is Continent continent)
            {
                Button btnCalc = new Button
                {
                    Text = "📊 Підрахувати населення материка",
                    Location = new Point(20, y),
                    Size = new Size(280, 40),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.LightGreen,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                btnCalc.Click += (s, e) => {
                    long totalPopulation = 0;
                    foreach (var c in continent.Countries) totalPopulation += c.Population;
                    MessageBox.Show($"Загальне населення материка {continent.Name}: {totalPopulation} осіб.", "Статистика");
                };
                detailsPanel.Controls.Add(btnCalc);
            }
            else if (obj is Country c)
            {
                AddField("Площа (кв. км):", c.Area.ToString(), v => { if (double.TryParse(v, out double res)) c.Area = res; });
                AddField("Столиця:", c.Capital, v => c.Capital = v);
                AddField("Форма правління:", c.GovernmentForm, v => c.GovernmentForm = v);
            }
            else if (obj is Region r)
            {
                AddField("Вид (область/штат):", r.Type, v => r.Type = v);
                AddField("Адмін. центр:", r.Capital, v => r.Capital = v);
            }
            else if (obj is City city)
            {
                AddField("Координати (Широта, Довгота):", city.Coordinates, v => city.Coordinates = v);
            }
        }

        private void BtnShowOnMap_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode?.Tag is City city && !string.IsNullOrEmpty(city.Coordinates))
            {
                string url = $"http://google.com/maps/place/{city.Coordinates.Replace(" ", "")}";
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
            }
            else MessageBox.Show("Оберіть місто з координатами!");
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
            detailsPanel.Controls.Clear(); // Очищаємо панель при оновленні дерева
        }

        private void InitializeCustomUI()
        {
            this.Text = "Довідник географа 2.0";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            TableLayoutPanel mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            treeView = new TreeView { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.None };
            detailsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };

            TableLayoutPanel buttonPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1, Padding = new Padding(10), BackColor = Color.FromArgb(240, 244, 248) };
            for (int i = 0; i < 6; i++) buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6F));

            void StyleBtn(Button b, Color bg, Color fg) { b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0; b.BackColor = bg; b.ForeColor = fg; b.Font = new Font("Segoe UI", 10, FontStyle.Bold); b.Dock = DockStyle.Fill; b.Margin = new Padding(5); }

            btnAddContinent = new Button { Text = "Материк" }; StyleBtn(btnAddContinent, Color.LightGray, Color.Black);
            btnAddChild = new Button { Text = "Додати" }; StyleBtn(btnAddChild, Color.LightGray, Color.Black);
            btnDelete = new Button { Text = "Видалити" }; StyleBtn(btnDelete, Color.FromArgb(208, 56, 70), Color.White);
            btnShowOnMap = new Button { Text = "Карта" }; StyleBtn(btnShowOnMap, Color.LightGray, Color.Black);
            btnSave = new Button { Text = "Зберегти" }; StyleBtn(btnSave, Color.FromArgb(0, 120, 212), Color.White);
            btnLoad = new Button { Text = "Відкрити" }; StyleBtn(btnLoad, Color.LightGray, Color.Black);

            buttonPanel.Controls.Add(btnAddContinent, 0, 0); buttonPanel.Controls.Add(btnAddChild, 1, 0); buttonPanel.Controls.Add(btnDelete, 2, 0);
            buttonPanel.Controls.Add(btnShowOnMap, 3, 0); buttonPanel.Controls.Add(btnSave, 4, 0); buttonPanel.Controls.Add(btnLoad, 5, 0);

            mainLayout.Controls.Add(treeView, 0, 0);
            mainLayout.Controls.Add(detailsPanel, 1, 0);
            mainLayout.Controls.Add(buttonPanel, 0, 1);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }
    }
}