using System.Collections.Generic;
using System.Linq;

namespace ZirconLang.Diagnostics
{
    public class Failable
    {
        private readonly List<Error> _errors;

        protected Failable()
        {
            _errors = new List<Error>();
        }

        protected void AddError(Error err)
        {
            _errors.Add(err);
        }

        protected List<Error> GetErrors()
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