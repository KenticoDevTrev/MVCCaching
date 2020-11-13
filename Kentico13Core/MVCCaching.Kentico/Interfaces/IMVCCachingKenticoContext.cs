using System;
using System.Collections.Generic;
using System.Text;

namespace MVCCaching.Interfaces
{
    public interface IMVCCachingKenticoContext
    {
        bool PreviewEnabled();
        string CurrentCulture();
    }
}
