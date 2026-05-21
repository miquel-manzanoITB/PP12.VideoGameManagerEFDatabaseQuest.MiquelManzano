using HeroEngine.Core.Data;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class DetailModel : PageModel
{
    private readonly HeroRepository _repo;

    [BindProperty(SupportsGet = true)]
    public string Name { get; set; } = "";

    public HeroDto? Hero { get; set; }

    public DetailModel(HeroRepository repo) => _repo = repo;

    public void OnGet()
    {
        Hero = _repo.LoadAll()
                    .FirstOrDefault(h => h.Name.Equals(Name,
                        StringComparison.OrdinalIgnoreCase));
    }
}
