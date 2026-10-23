using GerenciadorAcademico.Models;
namespace GerenciadorAcademico.Repositories;
public class ProjetoJsonRepository : IProjetoRepository
{
    private readonly JsonFileStore<Projeto> _arquivo;
    public ProjetoJsonRepository(JsonFileStore<Projeto> arquivo) => _arquivo = arquivo;
    public Task<List<Projeto>> ListarAsync() => _arquivo.LerAsync();
    public async Task<Projeto?> ObterPorIdAsync(int id) =>
        (await ListarAsync()).FirstOrDefault(p => p.Id == id);
    public async Task AdicionarAsync(Projeto projeto)
    {
        await _arquivo.AlterarAsync(itens =>
        {
            projeto.Id = itens.Count == 0 ? 1 : itens.Max(p => p.Id) + 1;
            itens.Add(projeto);
            return (true, true);
        });
    }
    public async Task AtualizarAsync(Projeto projeto)
    {
        bool achou = await _arquivo.AlterarAsync(itens =>
        {
            var atual = itens.FirstOrDefault(p => p.Id == projeto.Id);
            if (atual is null) return (false, false);
            atual.Titulo = projeto.Titulo;
            atual.Descricao = projeto.Descricao;
            atual.CargaHoraria = projeto.CargaHoraria;
            atual.ProfessorId = projeto.ProfessorId;
            return (true, true);
        });
        if (!achou) throw new KeyNotFoundException("Projeto não encontrado.");
    }
    public Task<bool> RemoverAsync(int id) => _arquivo.AlterarAsync(itens =>
    {
        var atual = itens.FirstOrDefault(p => p.Id == id);
        if (atual is null) return (false, false);
        itens.Remove(atual);
        return (true, true);
    });
}
