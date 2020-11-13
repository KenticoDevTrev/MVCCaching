using System;
using System.Collections.Generic;
using System.Text;

namespace MVCCaching
{
    public interface IRepoContext
    {
        bool PreviewEnabled();
        string CurrentCulture();
    }
}
