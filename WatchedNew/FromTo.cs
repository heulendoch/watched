using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core {
    public class FromTo {

        
        private int m_From;

        private int m_To;

        public FromTo(int From, int To) {

            if (From >= To) {
                throw new ArgumentException();
            }
            
            this.From = From;
            this.To = To;

        }

        public int To {
            get { return m_To; }
            set { m_To = value; }
        }

        public int From {
            get { return m_From; }
            set { m_From = value; }
        }

        public IEnumerable<int> Generate() { 
            for (int Intern = From; Intern <= To; Intern++) {
                yield return Intern;
            }
        }

    }
}
