using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    // cole��o com todos os produtos, fornecido pela IA nas pesquisas
    private List<Produto> allProdutos = new(); 
    
    // cole��o filtrada (ligada ao Listview), adaptado pela IA nas pesquisas
    private ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

	protected async override void OnAppearing()
	{
        base.OnAppearing();

        // altera��o sugerida pela IA para guardar todos em allProdutos
        allProdutos = await App.Db.GetAll();

        // sugerido pela IA para preencher a ObservableCollection com todos
        AtualizarLista(allProdutos);

    }

    // novo m�todo sugerido pela IA para auxiliar a atualiza��o da ObservableCollection
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
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        // altera��o sugerida pela IA para filtra a lista em mem�ria
        string q = e.NewTextValue?.ToLower() ?? "";

        var filtrados = allProdutos
            .Where(p => p.Descricao.ToLower().Contains(q)
                     || p.Preco.ToString().Contains(q)
                     || p.Quantidade.ToString().Contains(q));


        AtualizarLista(filtrados);
    }

    

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        var menuItem = sender as MenuItem;
        var produto = menuItem?.BindingContext as Produto;

        if (produto != null)
        {
            bool confirma = await DisplayAlert("Remover", $"Deseja Remover {produto.Descricao}?", "Sim", "N�o");

            if (confirma)
            {
                await App.Db.Delete(produto.Id);

                // sugerido pela IA para remover tamb�m da lista completa
                allProdutos.Remove(produto);
                lista.Remove(produto);
            }
        }
    }

    private void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total � {soma:C}";

        DisplayAlert("Total dos Produtos", msg, "OK");
    }
}