using GerenciadorAcademico.Models;
namespace GerenciadorAcademico.Repositories;
public class ProfessorJsonRepository : IProfessorRepository
{
    private readonly JsonFileStore<Professor> _arquivo;
    public ProfessorJsonRepository(JsonFileStore<Professor> arquivo) => _arquivo = arquivo;
    public Task<List<Professor>> ListarAsync() => _arquivo.LerAsync();
    public async Task<Professor?> ObterPorIdAsync(int id) =>
        (await ListarAsync()).FirstOrDefault(p => p.Id == id);
}
