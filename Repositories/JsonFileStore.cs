using System.Text.Json;

namespace GerenciadorAcademico.Repositories;

public class JsonFileStore<T>
{
    // O lock é comum a todas as requisições deste processo.
    private static readonly SemaphoreSlim Escrita = new(1, 1);
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    private readonly string _caminho;

    public JsonFileStore(IWebHostEnvironment ambiente)
    {
        string nome = typeof(T).Name switch
        {
            "Projeto" => "projetos.json",
            "Professor" => "professores.json",
            _ => throw new NotSupportedException("Tipo de arquivo não configurado.")
        };
        _caminho = Path.Combine(ambiente.ContentRootPath, "Data", nome);
    }

    public async Task<List<T>> LerAsync()
    {
        if (!File.Exists(_caminho))
            throw new FileNotFoundException($"Arquivo de dados não encontrado: {_caminho}", _caminho);
        var texto = await File.ReadAllTextAsync(_caminho);
        if (string.IsNullOrWhiteSpace(texto)) return new List<T>();
        try
        {
            return JsonSerializer.Deserialize<List<T>>(texto, Opcoes)
                ?? throw new InvalidDataException($"O arquivo {_caminho} não contém uma lista JSON válida.");
        }
        catch (JsonException erro)
        {
            throw new InvalidDataException($"JSON inválido no arquivo {_caminho}. O arquivo original foi preservado.", erro);
        }
    }

    // A operação inteira é protegida: ler, modificar e gravar.
    public async Task<TResult> AlterarAsync<TResult>(Func<List<T>, (bool Gravar, TResult Resultado)> alterar)
    {
        await Escrita.WaitAsync();
        try
        {
            var itens = await LerAsync();
            var (gravar, resultado) = alterar(itens);
            if (gravar) await GravarAsync(itens);
            return resultado;
        }
        finally { Escrita.Release(); }
    }

    private async Task GravarAsync(List<T> itens)
    {
        string temporario = _caminho + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await File.WriteAllTextAsync(temporario, JsonSerializer.Serialize(itens, Opcoes));
            File.Move(temporario, _caminho, overwrite: true);
        }
        finally { if (File.Exists(temporario)) File.Delete(temporario); }
    }
}
