using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Razor.Language;

namespace CustomizedViewLocation
{
    public class ModuleRazorTemplateEngine : MvcRazorTemplateEngine
    {
        public ModuleRazorTemplateEngine(RazorEngine engine, RazorProject project) : base(engine, project)
        {
        }

        public override IEnumerable<RazorProjectItem> GetImportItems(RazorProjectItem projectItem)
        {
            IEnumerable<RazorProjectItem> importItems = base.GetImportItems(projectItem);
            return importItems.Append(Project.GetItem($"/Shared/Views/{Options.ImportsFileName}"));
        }
    }
}