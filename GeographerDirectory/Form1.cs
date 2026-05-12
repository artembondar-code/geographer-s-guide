using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics; // Потрібно для відкриття браузера
using GeographerDirectory.Models;
using GeographerDirectory.Storage;
using Region = GeographerDirectory.Models.Region;

namespace GeographerDirectory
{
    public partial class Form1 : Form
    {
        private TreeView treeView;
        private PropertyGrid propertyGrid;
        private Button btnAddContinent;
        private Button btnAddChild;
        private Button btnDelete;
        private Button btnSave;
        private Button btnLoad;
        private Button btnShowOnMap; // Нова кнопка

        private DataManager dataManager;
        private List<Continent> continents;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomUI();

            dataManager = new DataManager();
            continents = new List<Continent>();

            btnAddContinent.Click += BtnAddContinent_Click;
            btnAddChild.Click += BtnAddChild_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnLoad.Click += BtnLoad_Click;
            btnShowOnMap.Click += BtnShowOnMap_Click; // Прив'язка нової кнопки

            treeView.AfterSelect += (s, e) => propertyGrid.SelectedObject = e.Node.Tag;

            propertyGrid.PropertyValueChanged += (s, e) =>
            {
                if (e.ChangedItem.PropertyDescriptor.Name == "Name" && treeView.SelectedNode != null)
                    treeView.SelectedNode.Text = e.ChangedItem.Value?.ToString() ?? "Без назви";
            };
        }

        // --- ЛОГІКА КНОПКИ КАРТИ ---
        private void BtnShowOnMap_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode?.Tag is City city)
            {
                if (string.IsNullOrWhiteSpace(city.Coordinates) || city.Coordinates == "0.0, 0.0")
                {
                    MessageBox.Show("Спочатку введіть коректні координати міста у форматі: Широта, Довгота");
                    return;
                }

                // Формуємо посилання на Google Maps
                string url = $"https://www.google.com/maps/search/?api=1&query={city.Coordinates.Replace(" ", "")}";

                try
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не вдалося відкрити браузер: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, оберіть МІСТО у дереві зліва.");
            }
        }

        private void BtnAddContinent_Click(object sender, EventArgs e)
        {
            continents.Add(new Continent("Новий Материк"));
            UpdateTreeView();
        }

        private void BtnAddChild_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;

            object selected = treeView.SelectedNode.Tag;

            if (selected is Continent cont)
                cont.Countries.Add(new Country("Нова Країна", 0, 0.0, "Не вказано", "Не вказано"));
            else if (selected is Country country)
                country.Regions.Add(new Region("Новий Регіон", 0, "Не вказано", "Не вказано"));
            else if (selected is Region region)
                region.Cities.Add(new City("Нове Місто", 0, "50.00, 36.23")); // Приклад координат (Харків)

            UpdateTreeView();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;
            object selected = treeView.SelectedNode.Tag;
            TreeNode parentNode = treeView.SelectedNode.Parent;

            if (parentNode == null) continents.Remove((Continent)selected);
            else
            {
                object parent = parentNode.Tag;
                if (parent is Continent cont) cont.Countries.Remove((Country)selected);
                else if (parent is Country country) country.Regions.Remove((Region)selected);
                else if (parent is Region region) region.Cities.Remove((City)selected);
            }
            UpdateTreeView();
        }

        private void BtnSave_Click(object sender, EventArgs e) => dataManager.SaveData(continents);

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            continents = dataManager.LoadData();
            UpdateTreeView();
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            foreach (var continent in continents)
            {
                TreeNode contNode = new TreeNode(continent.Name) { Tag = continent };
                foreach (var country in continent.Countries)
                {
                    TreeNode countryNode = new TreeNode(country.Name) { Tag = country };
                    foreach (var region in country.Regions)
                    {
                        TreeNode regionNode = new TreeNode(region.Name) { Tag = region };
                        foreach (var city in region.Cities)
                            regionNode.Nodes.Add(new TreeNode(city.Name) { Tag = city });
                        countryNode.Nodes.Add(regionNode);
                    }
                    contNode.Nodes.Add(countryNode);
                }
                treeView.Nodes.Add(contNode);
            }
            treeView.ExpandAll();
        }

        private void InitializeCustomUI()
        {
            this.Text = "Довідник географа";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10, 5, 10, 5) };
            this.Controls.Add(buttonPanel);

            treeView = new TreeView { Dock = DockStyle.Left, Width = 300, Font = new Font("Segoe UI", 10), HideSelection = false };
            this.Controls.Add(treeView);

            propertyGrid = new PropertyGrid { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(propertyGrid);

            btnAddContinent = new Button { Text = "Додати Материк", Width = 130, Height = 35 };
            btnAddChild = new Button { Text = "Додати об'єкт", Width = 130, Height = 35 };
            btnDelete = new Button { Text = "Видалити", Width = 90, Height = 35 };
            btnShowOnMap = new Button { Text = "На карті 🌐", Width = 110, Height = 35, BackColor = Color.LightBlue };
            btnSave = new Button { Text = "Зберегти", Width = 90, Height = 35 };
            btnLoad = new Button { Text = "Завантажити", Width = 110, Height = 35 };

            buttonPanel.Controls.Add(btnAddContinent);
            buttonPanel.Controls.Add(btnAddChild);
            buttonPanel.Controls.Add(btnDelete);
            buttonPanel.Controls.Add(btnShowOnMap);
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnLoad);
        }
    }
}