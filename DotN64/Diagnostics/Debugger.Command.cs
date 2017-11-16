using System;
using System.Collections.Generic;

namespace DotN64.Diagnostics
{
    public partial class Debugger
    {
        private struct Command
        {
            #region Properties
            public IEnumerable<string> Names { get; }

            public string Description { get; }

            public Action<string[]> Action { get; }
            #endregion

            #region Constructors
            public Command(IEnumerable<string> names, string description, Action<string[]> action)
            {
                Names = names;
                Description = description;
                Action = action;
            }
            #endregion
        }
    }
}
