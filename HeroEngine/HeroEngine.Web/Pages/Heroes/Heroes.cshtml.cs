using HeroEngine.Core.Data;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class HeroesModel : PageModel
{
    private readonly HeroRepository _repo;
    public List<HeroDto> Heroes { get; set; } = new();

    public HeroesModel(HeroRepository repo) => _repo = repo;

    public void OnGet()
    {
        Heroes = _repo.LoadAll();
    }

    public IActionResult OnPostDelete(string name)
    {
        _repo.Delete(name);
        return RedirectToPage();
    }
}
