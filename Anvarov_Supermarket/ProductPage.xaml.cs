using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Anvarov_Supermarket
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();

            var ProductDepartment = SupermarketEntities.GetContext().Department.Select(p => p.Department_Title).ToList();

            FiltrCmbox.Items.Add("Все");

            foreach (var Department in ProductDepartment)
            {
                FiltrCmbox.Items.Add(Department);
            }

            FiltrCmbox.SelectedIndex = 0;
            SortCmbox.SelectedIndex = 0;
            


            List<Product> currentProducts = SupermarketEntities.GetContext().Product.ToList();

            allProdCount.Text = currentProducts.Count().ToString();

            PrductListView.ItemsSource = currentProducts;

            Update();

        }

        public void Update()
        {
            var currentProducts = SupermarketEntities.GetContext().Product.ToList();
            if (FiltrCmbox.SelectedIndex != 0)
            {
                currentProducts = currentProducts.Where(p => p.Product_Department == FiltrCmbox.SelectedIndex).ToList();
            }

            switch (SortCmbox.SelectedIndex)
            {
                case 1:
                    currentProducts = currentProducts.OrderBy(p => p.Product_Cost).ToList();
                    break;
                case 2:
                    currentProducts = currentProducts.OrderByDescending(p => p.Product_Cost).ToList();
                    break;
            }

            currentProducts = currentProducts.Where(p => p.Product_Title.ToLower().Contains(productSearch.Text.ToLower())).ToList();

            currentProdCount.Text = currentProducts.Count().ToString();

            PrductListView.ItemsSource = currentProducts;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void SortCmbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void FiltrCmbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void productSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Product));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Update();
        }
    }
}
