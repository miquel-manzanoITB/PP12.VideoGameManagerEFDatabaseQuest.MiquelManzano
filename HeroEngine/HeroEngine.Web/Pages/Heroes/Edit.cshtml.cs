using HeroEngine.Core.Data;
using HeroEngine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class EditModel : PageModel
{
    private readonly HeroEngineContext _context;

    public EditModel(HeroEngineContext context) => _context = context;

    [BindProperty]
    public HeroEntity Hero { get; set; } = default!;

    public SelectList HeroClassList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Hero = await _context.Heroes.FindAsync(id)
               ?? throw new InvalidOperationException("Heroi no trobat");

        HeroClassList = new SelectList(
            await _context.HeroClasses.ToListAsync(), "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            HeroClassList = new SelectList(
                await _context.HeroClasses.ToListAsync(), "Id", "Name");
            return Page();
        }

        _context.Attach(Hero).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return RedirectToPage("/Heroes/Heroes");
    }
}
