using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public interface INumbersDataHandler
    {
        public void UpdateNumber(string label, int value);
        public void UpdateNumber(string label, float value);

        public void UpdateNumber(string label, long value);
    }
}
