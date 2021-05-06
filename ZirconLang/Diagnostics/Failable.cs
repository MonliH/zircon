using System.Collections.Generic;
using System.Linq;

namespace ZirconLang.Diagnostics
{
    public class Failable
    {
        private readonly List<ErrorDisplay> _errors;

        protected Failable()
        {
            _errors = new List<ErrorDisplay>();
        }

        protected void AddError(ErrorDisplay err)
        {
            _errors.Add(err);
        }

        protected List<ErrorDisplay> GetErrors()
        {
            return _errors;
        }

        protected void RaiseErrors()
        {
            // If the list has elements, this failed
            if (_errors.Any())
            {
                throw new Errors(_errors);
            }
        }
    }
}