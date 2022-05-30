using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public class LogServiceFactory
    {
        public static ILogService CreateDefault()
        {
            return new OddLogService();
        }
    }
}
