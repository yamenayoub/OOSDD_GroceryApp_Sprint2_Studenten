using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    [QueryProperty(nameof(GroceryList), nameof(GroceryList))]
    public partial class GroceryListItemsViewModel : BaseViewModel
    {
        private readonly IGroceryListItemsService _groceryListItemsService;
        private readonly IProductService _productService;
        public ObservableCollection<GroceryListItem> MyGroceryListItems { get; set; } = [];
        public ObservableCollection<Product> AvailableProducts { get; set; } = [];

        [ObservableProperty]
        GroceryList groceryList = new(0, "None", DateOnly.MinValue, "", 0);

        public GroceryListItemsViewModel(IGroceryListItemsService groceryListItemsService, IProductService productService)
        {
            _groceryListItemsService = groceryListItemsService;
            _productService = productService;
            Load(groceryList.Id);
        }

        private void Load(int id)
        {
            MyGroceryListItems.Clear();
            foreach (var item in _groceryListItemsService.GetAllOnGroceryListId(id)) MyGroceryListItems.Add(item);
            GetAvailableProducts();
        }

        private void GetAvailableProducts()
        {
            // Maak de lijst AvailableProducts leeg
            AvailableProducts.Clear();

            // Haal de lijst met producten op
            var products = _productService?.GetAll();

            // Controleer of het product al op de boodschappenlijst staat, zo niet zet het in de AvailableProducts lijst
            if (products == null || products.Count == 0) return;

            // Maak een HashSet van de bestaande ProductIds in MyGroceryListItems voor snelle lookup, HashSet doet dit efficiënter dan List.Contains
            var existingIds = MyGroceryListItems.Select(i => i.ProductId).ToHashSet();

            // Voeg alleen producten toe die niet in existingIds zitten en waarvan de voorraad (Stock) groter is dan 0
            foreach (var product in products.Where(p => p != null && p.Stock > 0 && !existingIds.Contains(p.Id)))
            {
                // Voeg het product toe aan de lijst AvailableProducts
                AvailableProducts.Add(product);
            }
        }

        partial void OnGroceryListChanged(GroceryList value)
        {
            Load(value.Id);
        }

        [RelayCommand]
        public async Task ChangeColor()
        {
            Dictionary<string, object> paramater = new() { { nameof(GroceryList), GroceryList } };
            await Shell.Current.GoToAsync($"{nameof(ChangeColorView)}?Name={GroceryList.Name}", true, paramater);
        }
        [RelayCommand]
        public void AddProduct(Product product)
        {
            //Controleer of het product bestaat en dat de Id > 0
            //Maak een GroceryListItem met Id 0 en vul de juiste productid en grocerylistid
            //Voeg het GroceryListItem toe aan de dataset middels de _groceryListItemsService
            //Werk de voorraad (Stock) van het product bij en zorg dat deze wordt vastgelegd (middels _productService)
            //Werk de lijst AvailableProducts bij, want dit product is niet meer beschikbaar
            //call OnGroceryListChanged(GroceryList);

            // Controleer of het product bestaat en dat de Id > 0
            if (product == null || product.Id <= 0) return;

            // Maak een GroceryListItem met Id 0 en vul de juiste productid en grocerylistid
            var groceryListItem = new GroceryListItem(0, GroceryList.Id, product.Id, 1);

            // Voeg het GroceryListItem toe aan de dataset middels de _groceryListItemsService
            _groceryListItemsService.Add(groceryListItem);

            // Werk de voorraad (Stock) van het product bij en zorg dat deze wordt vastgelegd (middels _productService)
            product.Stock--;
            _productService.Update(product);

            // Werk de lijst AvailableProducts bij, want dit product is niet meer beschikbaar
            AvailableProducts.Remove(product);

            // call OnGroceryListChanged(GroceryList)
            OnGroceryListChanged(GroceryList);
        }
    }
}
