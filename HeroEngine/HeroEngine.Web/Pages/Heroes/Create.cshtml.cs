using HeroEngine.Core.Data;
using HeroEngine.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public class CreateModel : PageModel
{
    private readonly HeroRepository _repo;

    public CreateModel(HeroRepository repo) => _repo = repo;

    [BindProperty]
    public HeroInputModel Input { get; set; } = new();

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        _repo.Add(new HeroDto
        {
            Name = Input.Name,
            Type = Input.Type,
            Level = Input.Level,
            MaxHp = 100 + (Input.Level - 1) * 20,
            Armor = Input.Type == "Warrior" ? 10 + (Input.Level - 1) * 2 : 0,
            Abilities = new List<AbilityDto>()
        });

        return RedirectToPage("/Heroes/Heroes");
    }
}

public class HeroInputModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 30 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Class is required.")]
    public string Type { get; set; } = "Warrior";

    [Range(1, 20, ErrorMessage = "Level must be between 1 and 20.")]
    public int Level { get; set; } = 1;
}
