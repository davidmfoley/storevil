using System;
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

    }
}