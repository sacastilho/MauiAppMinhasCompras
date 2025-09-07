using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    // coleção com todos os produtos, fornecido pela IA nas pesquisas
    private List<Produto> allProdutos = new(); 
    
    // coleção filtrada (ligada ao Listview), adaptado pela IA nas pesquisas
    private ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

	protected async override void OnAppearing()
	{
        base.OnAppearing();
        try
        {
            // alteração sugerida pela IA para guardar todos em allProdutos
            allProdutos = await App.Db.GetAll();

            // sugerido pela IA para preencher a ObservableCollection com todos
            AtualizarLista(allProdutos);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }



    }

    // novo método sugerido pela IA para auxiliar a atualização da ObservableCollection
    private void AtualizarLista (IEnumerable<Produto> produto)
    {
        lista.Clear();
        foreach (var p in produto)
            lista.Add(p);
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		} catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }
    private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            // alteração sugerida pela IA para filtra a lista em memória
            string q = e.NewTextValue?.ToLower() ?? "";

            var filtrados = allProdutos
                .Where(p => p.Descricao.ToLower().Contains(q)
                         || p.Preco.ToString().Contains(q)
                         || p.Quantidade.ToString().Contains(q));


            AtualizarLista(filtrados);
        } 
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
        
    }

    

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var produto = menuItem?.BindingContext as Produto;

        if (produto != null)
        {
            bool confirma = await DisplayAlert("Remover", $"Deseja Remover {produto.Descricao}?", "Sim", "Não");

            if (confirma)
            {
                try
                {
                    await App.Db.Delete(produto.Id);

                    // sugerido pela IA para remover também da lista completa
                    allProdutos.Remove(produto);
                    lista.Remove(produto);
                }
                catch (Exception ex)
                {
                    DisplayAlert("Ops", ex.Message, "OK");

                }
            }
        }
    }

    private void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");

        }

    }
}