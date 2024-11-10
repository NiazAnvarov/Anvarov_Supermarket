using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {

        private Product currentProduct = new Product();
        public bool check = false;

        public AddEditPage(Product SelectedProduct)
        {
            InitializeComponent();

            var ProductDepartment = SupermarketEntities.GetContext().Department.Select(p => p.Department_Title).ToList();

            foreach (var Department in ProductDepartment)
            {
                DepCombox.Items.Add(Department);
            }

            if (SelectedProduct != null)
            {
                DeleteBtn.Visibility = Visibility.Visible;
                check = true;
                currentProduct = SelectedProduct;
                DepCombox.SelectedIndex = currentProduct.Product_Department - 1;
                ProdCost.Text = currentProduct.Product_Cost.ToString();
            }
            else
            {
                ProdCost.Text = "0";
                DepCombox.SelectedIndex = 0;
                DeleteBtn.Visibility = Visibility.Hidden;
            }

            DataContext = currentProduct;
            
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentProduct.Product_Photo = myOpenFileDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            decimal cost;
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentProduct.Product_Title))
                errors.AppendLine("Введите название товара!");
            if (string.IsNullOrWhiteSpace(ProdCost.Text) || !decimal.TryParse(ProdCost.Text, out cost))
            {
                errors.AppendLine("Не верная цена товара!");
            }
            else if(cost <= 0)
            {
                errors.AppendLine("Не верная цена товара!");
            }
                
            if (string.IsNullOrWhiteSpace(currentProduct.Product_CountryOfOrigin))
                errors.AppendLine("Введите страну производителя");
            //if (string.IsNullOrWhiteSpace(currentProduct.Product_ShelfLife))
            //    errors.AppendLine("Введите срок годности");
            //if (string.IsNullOrWhiteSpace(currentProduct.Product_StorageConditions))
            //    errors.AppendLine("Введите условия хранения");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if(string.IsNullOrWhiteSpace(currentProduct.Product_ShelfLife))
            {
                currentProduct.Product_ShelfLife = null;
            }
            if(string.IsNullOrWhiteSpace(currentProduct.Product_StorageConditions))
            {
                currentProduct.Product_StorageConditions = "Нет";
            }

            currentProduct.Product_Cost = decimal.Parse(ProdCost.Text);
            currentProduct.Product_Department = DepCombox.SelectedIndex + 1;

            var allProduct = SupermarketEntities.GetContext().Product.ToList();
            allProduct = allProduct.Where(p => p.Product_Title == currentProduct.Product_Title && p.Product_CountryOfOrigin == currentProduct.Product_CountryOfOrigin).ToList();

            if (allProduct.Count == 0 || check == true)
            {
                if (currentProduct.Product_Number == 0)
                    SupermarketEntities.GetContext().Product.Add(currentProduct);
                try
                {
                    SupermarketEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
                MessageBox.Show("Такой товар уже существует!");


        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentProduct = (sender as Button).DataContext as Product;

            var currentProductList = SupermarketEntities.GetContext().ProductSale.ToList();
            currentProductList = currentProductList.Where(p => p.ProductSale_Number == currentProduct.Product_Number).ToList();

            if(currentProductList.Count != 0)
            {
                MessageBox.Show("Невозможно выполнить удаление!");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        SupermarketEntities.GetContext().Product.Remove(currentProduct);
                        SupermarketEntities.GetContext().SaveChanges();
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }


        }
    }
}
