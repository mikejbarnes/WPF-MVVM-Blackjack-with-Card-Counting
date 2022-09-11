using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM_BlackJackWIthCardCounting.Exceptions
{
    [Serializable]
    class IncorrectAcePointValueException : Exception
    {
        public IncorrectAcePointValueException() : base("Method GetCardPointValue() cannot calculate the value of an Ace.") { }
    }
}
