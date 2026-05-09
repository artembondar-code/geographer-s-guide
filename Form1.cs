using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GeographerGuide
{
    public partial class Form1 : Form
    {
        // Список, у якому зберігатимуться всі міста
        private List<City> cities = new List<City>();

        // Оголошення елементів форми (додано = null!, щоб прибрати попередження)
        private ListBox listBoxCities = null!;
        private TextBox txtCityName = null!;
        private TextBox txtPopulation = null!;
        private TextBox txtCoordinates = null!;
        private Button btnAdd = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private Label lblCity = null!;
        private Label lblPopulation = null!;
        private Label lblCoordinates = null!;

        public Form1()
        {
            InitializeCustomComponent();
        }

        private void InitializeCustomComponent()
        {
            this.Text = "Довідник географа";
            this.Size = new Size(550, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Налаштування списку міст
            listBoxCities = new ListBox { Location = new Point(15, 15), Size = new Size(220, 300) };
            listBoxCities.SelectedIndexChanged += ListBoxCities_SelectedIndexChanged;

            // Налаштування текстових полів
            lblCity = new Label { Text = "Назва міста:", Location = new Point(260, 20) };
            txtCityName = new TextBox { Location = new Point(260, 45), Size = new Size(220, 25) };

            lblPopulation = new Label { Text = "Населення:", Location = new Point(260, 80) };
            txtPopulation = new TextBox { Location = new Point(260, 105), Size = new Size(220, 25) };

            lblCoordinates = new Label { Text = "Координати:", Location = new Point(260, 140) };
            txtCoordinates = new TextBox { Location = new Point(260, 165), Size = new Size(220, 25) };

            // Кнопка Додати
            btnAdd = new Button { Text = "Додати", Location = new Point(260, 210), Size = new Size(220, 35) };
            btnAdd.Click += BtnAdd_Click;

            // Кнопка Редагувати
            btnEdit = new Button { Text = "Редагувати", Location = new Point(260, 255), Size = new Size(220, 35) };
            btnEdit.Click += BtnEdit_Click;

            // Кнопка Видалити
            btnDelete = new Button { 
                Text = "Видалити", 
                Location = new Point(260, 300), 
                Size = new Size(220, 35), 
                BackColor = Color.LightCoral 
            };
            btnDelete.Click += BtnDelete_Click;

            // Додаємо все на форму
            this.Controls.AddRange(new Control[] { 
                listBoxCities, lblCity, txtCityName, lblPopulation, 
                txtPopulation, lblCoordinates, txtCoordinates, btnAdd, btnEdit, btnDelete 
            });
        }

        // Функція для оновлення візуального списку
        private void RefreshCities()
        {
            listBoxCities.DataSource = null;
            listBoxCities.DataSource = cities;
            listBoxCities.DisplayMember = "Name";
        }

        // Функція для очищення полів після введення
        private void ClearInputs()
        {
            txtCityName.Clear();
            txtPopulation.Clear();
            txtCoordinates.Clear();
        }

        // МЕТОДИ ОБРОБКИ ПОДІЙ (Додано object? sender)
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCityName.Text))
            {
                MessageBox.Show("Будь ласка, введіть назву міста.");
                return;
            }

            City newCity = new City
            {
                Name = txtCityName.Text,
                Population = txtPopulation.Text,
                Coordinates = txtCoordinates.Text
            };

            cities.Add(newCity);
            RefreshCities();
            ClearInputs();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (listBoxCities.SelectedItem is City selectedCity)
            {
                selectedCity.Name = txtCityName.Text;
                selectedCity.Population = txtPopulation.Text;
                selectedCity.Coordinates = txtCoordinates.Text;
                RefreshCities();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (listBoxCities.SelectedItem is City selectedCity)
            {
                cities.Remove(selectedCity);
                RefreshCities();
                ClearInputs();
            }
        }

        private void ListBoxCities_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBoxCities.SelectedItem is City selectedCity)
            {
                txtCityName.Text = selectedCity.Name;
                txtPopulation.Text = selectedCity.Population;
                txtCoordinates.Text = selectedCity.Coordinates;
            }
        }
    }
}