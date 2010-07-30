using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil;

namespace Tutorial
{
    [Context]
    public class CustomParameterConversionContext
    {
        private float _float;


        public void we_can_parse_floats_like(float f)
        {   
            _float = f;
        }

        public void And_the_result_should_be_between_a_and_b(decimal a, decimal b)
        {
            Assert.That(a, Is.LessThan(_float));
            Assert.That(_float, Is.LessThan(b));
            
        }

    }
}