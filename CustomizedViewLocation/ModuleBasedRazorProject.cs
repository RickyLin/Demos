using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Razor.Language;

namespace CustomizedViewLocation
{
    public class ModuleBasedRazorProject : FileProviderRazorProject
    {
        public ModuleBasedRazorProject(IRazorViewEngineFileProviderAccessor accessor)
            : base(accessor)
        {

        }

        public override IEnumerable<RazorProjectItem> FindHierarchicalItems(string basePath, string path, string fileName)
        {
            IEnumerable<RazorProjectItem> items = base.FindHierarchicalItems(basePath, path, fileName);

            // the items are in the order of closest first, furthest last, therefore we append our item to be the last item.
            return items.Append(GetItem("/Shared/Views/" + fileName));
        }
    }
}