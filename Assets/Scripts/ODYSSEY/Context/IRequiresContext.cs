using System;
using System.Collections.Generic;


namespace Odyssey
{
    public interface IRequiresContext
    {
        public void Init(IMomentumContext context);
    }
}
