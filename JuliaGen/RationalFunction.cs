using System.Collections.Generic;
using System.Numerics;

namespace JuliaGen
{
    class Polynomial
    {
        //key = exponent, value = constant factor
        public Dictionary<int, Complex> factors = new Dictionary<int, Complex>();

        public Complex calculate(Complex input)
        {
            Complex num = 0;
            foreach (var factor in factors)
            {
                num += power(input,factor.Key) * factor.Value;
            }
            return num;

        }

        private Complex power(Complex x, int a)
        {
            Complex o = 1;
            while (a>0)
            {
                o *= x;
                --a;
            }
            return o;
        }
    }
}
