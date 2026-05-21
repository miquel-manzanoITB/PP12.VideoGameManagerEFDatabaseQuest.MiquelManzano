using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class CreateModel : PageModel
{
    private readonly HeroEngineContext _context;

    public CreateModel(HeroEngineContext context) => _context = context;

    [BindProperty]
    public HeroInputModel Input { get; set; } = new();

    // Tasca 8.2: SelectList de HeroClass
    public SelectList HeroClassList { get; set; } = default!;

    public async Task OnGetAsync()
    {
        HeroClassList = new SelectList(
            await _context.HeroClasses.ToListAsync(), "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            HeroClassList = new SelectList(
                await _context.HeroClasses.ToListAsync(), "Id", "Name");
            return Page();
        }

        var hero = new HeroEntity
        {
            Name = Input.Name,
            Level = Input.Level,
            MaxHp = 100 + (Input.Level - 1) * 20,
            Armor = Input.Armor,
            Biography = Input.Biography,
            HeroClassId = Input.HeroClassId
        };

        _context.Heroes.Add(hero);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Heroes/Heroes");
    }
}

public class HeroInputModel
{
    [Required(ErrorMessage = "El nom és obligatori")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "El nom ha de tenir entre 2 i 80 caràcters")]
    public string Name { get; set; } = "";

    [Range(1, 100, ErrorMessage = "El nivell ha de ser entre 1 i 100")]
    public int Level { get; set; } = 1;

    [Range(0, 999)]
    public int Armor { get; set; } = 0;

    public string? Biography { get; set; }

    [Required(ErrorMessage = "La classe és obligatòria")]
    public int HeroClassId { get; set; }
}
