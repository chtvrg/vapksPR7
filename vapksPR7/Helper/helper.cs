using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vapksPR7.Entity;

namespace vapksPR7.Helper
{
    internal class helper
    {
        private static vapksPR7Entities _context;

        public static vapksPR7Entities GetContext()
        {
            if (_context == null)
                _context = new vapksPR7Entities();
            return _context;
        }
    }
}
