using System;

namespace Licensing.Exceptions
{
    public class PemFileInvalidException : Exception
    {
        public PemFileInvalidException() : base("License public key in PEM file is invalid") { }
    }
}
